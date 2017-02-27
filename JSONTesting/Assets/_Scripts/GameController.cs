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
    public static GameController currentGameController;

    public Text alertText;              // How many alerts have we had?
    public Text deviceCountText;        // How many devices are there currently?
    public ObjectPooler computerPooler; // The object pooler for the computer prefab

    private int alertCount;             // The count of how many alerts we have had
    private GameObject obj;             // Use this as a temp calculation variable for better memory
    private Dictionary<string, GameObject> computersDict; // A dictionary of all the computers I have

    public Dictionary<string, GameObject> ComputersDict { get { return computersDict; } }
    #endregion

    void Awake ()
    {
        // Set the static reference
        currentGameController = this;
        // Set the alert count
        alertCount = 0;
        // Initialize the dictionary
        computersDict = new Dictionary<string, GameObject>();

        // Set the text
        deviceCountText.text = "0";
        alertText.text = "0";
    }

    public void CheckIpEnum(Source jsonSourceData)
    {
        // Make sure that we have something in the array
        if (jsonSourceData == null)
        { 
            return;
        }

        // Make sure that the IP's are not null, this happens some times with ELK stack
        if (jsonSourceData.id_orig_h == null ||
            jsonSourceData.id_orig_h == "Null")
        {
            return;
        }

        // If this is an alert from snort then add to the alerts count
    /*    if (jsonSourceData.alert)
        {
            alertCount++;
            alertText.text = alertCount.ToString();
        }
        */

        // If we know of the source IP already...
        if (computersDict.ContainsKey(jsonSourceData.id_orig_h))
        {
            // I want to check if there is a connection that I should add
            CheckConnectionsEnum(jsonSourceData);
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
        obj = computerPooler.GetPooledObject();

        // If the object is null then break out of this
        if (obj == null) return;

        // Set the DATA on this gameobject to the data from the JSON data
        obj.GetComponent<Computer>().SourceInfo = jsonSourceData;

        // Set this object as active in the hierachy so that you can actually see it
        obj.SetActive(true);

        // Add the object to the dictionary
        computersDict.Add(jsonSourceData.id_orig_h, obj);

        // Update the UI that tells us how many devices there are
        deviceCountText.text = computersDict.Count.ToString();

        // Check the connections to this
        CheckConnectionsEnum(jsonSourceData);

        // Check if we can add it to a group
        IPGroupManager.currentIpGroups.CheckGroups(jsonSourceData.id_orig_h);
    }

    /// <summary>
    /// Check if the destination IP of this source data is in our dictionary.
    /// If it is, then add the connection to each computer's list.
    /// If not, then add it as a new computer. 
    /// </summary>
    /// <param name="source">This source data has a SOURCE IP that we know
    /// of already, and we want to check the destination to see if we know
    /// of that or not.</param>
    private void CheckConnectionsEnum(Source source)
    {
        // I need to check if my destination is a source  address, which would mean that it is in my dictionary already
        if (source.id_resp_h == null || source.id_resp_h == "Null")
        {
            return;
        }

        // If we do already have the destination on the network, then connect them
        if (computersDict.ContainsKey(source.id_resp_h))
        {
            // Add a connection from source to destination
            computersDict[source.id_orig_h].GetComponent<Computer>().AddConnectedPC(computersDict[source.id_resp_h]);
            // Add a connection from destination to source
            computersDict[source.id_resp_h].GetComponent<Computer>().AddConnectedPC(computersDict[source.id_orig_h]);
        }
        else
        {
            // We DO NOT have the responding IP on our network, so add it.
            // Make a new source object
            Source newSource = new Source();

            // Set the NEW source's original IP to the response IP of the other one
            newSource.id_orig_h = source.id_resp_h;
            // Set the NEW source's orig. Port to the response port of the other one
            newSource.id_orig_p = source.id_resp_p;
            // Set that transport to the same thing
            //newSource.transport = source.transport;
            // I got this information from the same log file, so the source is the same
            //newSource.source = source.source;
            // Add this new computer to the network
            NewComputerEnum(newSource);
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
