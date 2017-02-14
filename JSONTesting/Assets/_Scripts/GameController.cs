using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Ben Hoffman
/// This class will do a lot of important things. 
/// It will have a way to STORE all my current 'computers' on the network
/// It will have a method to check if a
/// </summary>
public class GameController : MonoBehaviour {

    #region Fields
    public Text alertText;              // How many alerts have we had?
    public Text deviceCountText;        // How many devices are there currently?
    public ObjectPooler computerPooler; // The object pooler for the computer prefab

    private int alertCount;             // The count of how many alerts we have had
    // This will be used to randomly position the computers within this area.   
    // The max range will be whatever number you put in, and the min will be 
    // the negative of that. So if you enter 100,100,100, then a random
    // position will be within -100 and 100 on each axis.
    public Vector3 boundries;
    public float positionScalar = 1f;   // How far we want to scale the time by when positioning things
    private Vector3 tempPosition;       // Used for position calculations
    private float timeSinceStart;       // How long it has been since start 

    private Dictionary<string, GameObject> computersDict; // A dictionary of all the computers I have
    #endregion

    void Start ()
    {
        alertCount = 0;
        computersDict = new Dictionary<string, GameObject>();
        timeSinceStart = Time.timeSinceLevelLoad / 60f;
        deviceCountText.text = "Devices: 0";
    }

    public IEnumerator CheckIpEnum(Source jsonSourceData)
    {
        // Make sure that we have something in the array
        if(jsonSourceData == null)
        {
            yield break;
        }

        // Make sure that the IP's are not null, this happens some times with ELK stack
        if (jsonSourceData.id_orig_h == null ||
            jsonSourceData.id_orig_h == "Null")
        {
            yield break;
        }

        // If this is an alert from snort then add to the alerts count
        if (jsonSourceData.alert == "true")
        {
            alertCount++;
            alertText.text = "Alerts: " + alertCount.ToString();
        }


        // If we know of the source IP already...
        if (computersDict.ContainsKey(jsonSourceData.id_orig_h))
        {
            // I want to check if there is a connection that I should add
            CheckConnectionsEnum(jsonSourceData, computersDict[jsonSourceData.id_orig_h]);
        }
        else
        {
            // If I do NOT have this IP in my dictionary, then make a new computer        
            NewComputerEnum(jsonSourceData);
        }

    }

    /// <summary>
    /// We have NOT seen this IP before, and we want to make
    /// a new one
    /// Author: Ben Hoffman
    /// </summary>
    public void NewComputerEnum(Source jsonSourceData)
    {
        // If this is null or we already have the IP somehow...
        if(jsonSourceData.id_orig_h == null || computersDict.ContainsKey(jsonSourceData.id_orig_h))
        {
            return;
        }

        // Get a temporary object from the object pooler
        GameObject obj = computerPooler.GetPooledObject();

        // If the object is null then break out of this
        if (obj == null) return;

        // Get the start time
        timeSinceStart = Time.timeSinceLevelLoad / 60f;

        // Scale the radius of sphere by the time since start, and then get a random point on it
        tempPosition = Random.onUnitSphere * timeSinceStart * positionScalar;

        // Set the position and rotation of the object
        obj.transform.position = tempPosition;
        obj.transform.rotation = Quaternion.identity;

        // Set the DATA on this gameobject to the data from the JSON data
        obj.GetComponent<Computer>().SourceInfo = jsonSourceData;
        obj.SetActive(true);

        // Add the object to the dictionary
        computersDict.Add(jsonSourceData.id_orig_h, obj);

        // Update the UI that tells us how many devices there are
        deviceCountText.text = "Devices: " + computersDict.Count.ToString();

        // Check the connections to this
        CheckConnectionsEnum(jsonSourceData, computersDict[jsonSourceData.id_orig_h]);

    }

    /// <summary>
    /// Author: Ben Hoffman
    /// This is for when there is a computer that we already have the SOURCE
    /// IP for no the network. This will check if we already have the DESTINATION
    /// IP of that object or not. 
    /// </summary>
    /// <param name="data">The data of that commputer with the same source IP so I can check the dest.</param>
    /// <param name="checkMe">The game object that I already have, from my dicionary</param>
    private void CheckConnectionsEnum(Source broMessage, GameObject checkMe)
    {
        // I need to check if my destination is a source  address, which would mean that it is in my dictionary already
        if (broMessage.id_resp_h == null || broMessage.id_resp_h == "Null" || checkMe == null)
        {
            return;
        }

        // If we do already have the destination on the network, then connect them
        if (computersDict.ContainsKey(broMessage.id_resp_h))
        {
            // We have this IP on our network already, add the connection to the thing we are checking
            checkMe.GetComponent<Computer>().AddConnectedPC(computersDict[broMessage.id_resp_h]);
            // Add the connection to the other computer
            computersDict[broMessage.id_orig_h].GetComponent<Computer>().AddConnectedPC(checkMe);
        }
        else
        {
            // We DO NOT have the responding IP on our network, so add it.
            // Change up the data so that it represents the repsonding IP
            broMessage.id_orig_h = broMessage.id_resp_h;
            broMessage.id_resp_h = "Null";

            broMessage.id_orig_p = broMessage.id_resp_p;
            broMessage.id_resp_p = -1;
            // Add this new computer to the network
            NewComputerEnum(broMessage);
        }
    }

    /// <summary>
    /// Simply get the transform of the given IP
    /// </summary>
    /// <param name="IP">The IP of the computer that we want to find</param>
    /// <returns>The transform of the computer</returns>
    public Transform GetTransform(string IP)
    {
        if (IP == null || IP == "null")
        {
            return null;
        }

        if(computersDict.ContainsKey(IP))
            return computersDict[IP].transform;

        return null;
    }

    /// <summary>
    /// Check if our dictionary comtaints this IP
    /// </summary>
    /// <param name="IP">The IP that we want to know if we have</param>
    /// <returns>True if we have this key</returns>
    public bool CheckDictionary(string IP)
    {
        if(IP == null || IP == "null")
        {
            return false;
        }

        return computersDict.ContainsKey(IP);
    }
}
