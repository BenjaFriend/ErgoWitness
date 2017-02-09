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
    public Text alertText;
    private int alertCount;
    // This will be used to randomly position the computers within this area. 
    // The max range will be whatever number you put in, and the min will be 
    // the negative of that. So if you enter 100,100,100, then a random
    // position will be within -100 and 100 on each axis.
    public Vector3 boundries;
    public float positionScalar = 1f;
    private Vector3 tempPosition;
    private float timeSinceStart;
    private Dictionary<string, GameObject> computersDict; // A dictionary of all the computers I have
    #endregion

    void Start ()
    {
        alertCount = 0;
        computersDict = new Dictionary<string, GameObject>();
        timeSinceStart = Time.timeSinceLevelLoad / 60f;
    }

    public IEnumerator CheckIpEnum(Source jsonSourceData)
    {
        // Make sure that we have something in the array
        if(jsonSourceData == null)
        {
            yield break;
        }

        if(jsonSourceData.alert == "true")
        {
            alertCount++;
            alertText.text = "Alerts: " + alertCount.ToString();
        }

        // Make sure that we are not null
        if (jsonSourceData.id_orig_h == null ||
            jsonSourceData.id_orig_h == "Null")
        {
            yield break;
        }

        yield return null;

        // If my dictionary contains the IP address of this JSON info...
        if (jsonSourceData.id_orig_h != null && computersDict.ContainsKey(jsonSourceData.id_orig_h))
        {
            // I want to check if there is a connection that I should add
            StartCoroutine(CheckConnectionsEnum(jsonSourceData,
                computersDict[jsonSourceData.id_orig_h]));
        }
        else
        {
            // If I do NOT have this IP in my dictionary, then make a new computer        
            StartCoroutine(NewComputerEnum(jsonSourceData));
        }
        yield return null;

    }

    /// <summary>
    /// Author: Ben Hoffman
    /// We have NOT seen this IP before, and we want to make
    /// a new one
    /// </summary>
    private IEnumerator NewComputerEnum(Source jsonSourceData)
    {
        if (jsonSourceData.id_orig_h != null)
        {
            yield return null;
            GameObject obj = ObjectPooler.current.GetPooledObject();

            if (obj == null) yield return null;

            timeSinceStart = Time.timeSinceLevelLoad / 60f;

            tempPosition = Random.onUnitSphere * timeSinceStart * positionScalar;

            yield return null;

            obj.transform.position = tempPosition;
            obj.transform.rotation = Quaternion.identity;

            // Set the DATA on this gameobject to the data from the JSON data
            obj.GetComponent<Computer>().SetData(jsonSourceData);
            obj.SetActive(true);
            yield return null;

            computersDict.Add(jsonSourceData.id_orig_h, obj);
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// This is for when there is a computer that we already have the SOURCE
    /// IP for no the network. This will check if we already have the DESTINATION
    /// IP of that object or not. 
    /// </summary>
    /// <param name="data">The data of that commputer with the same source IP so I can check the dest.</param>
    /// <param name="checkMe">The game object that I already have, from my dicionary</param>
    private IEnumerator CheckConnectionsEnum(Source broMessage, GameObject checkMe)
    {
        yield return null;
        // I need to check if my destination is a source  address, which would mean that it is in my dictionary already
        if (broMessage.id_orig_h == null || broMessage.id_orig_h == "Null")
        {
            yield return null;
        }

        // If we do already have the destination on the network, then connect them
        if (computersDict.ContainsKey(broMessage.id_orig_h))
        {
            // We have this IP on our network already, add the connection to each PC
            yield return null;

            checkMe.GetComponent<Computer>().AddConnectedPC(computersDict[broMessage.id_orig_h]);
            computersDict[broMessage.id_orig_h].GetComponent<Computer>().AddConnectedPC(checkMe);
        }
        else
        {
            // Here is the problem... I am making a new computer with the same data
            // as the one already on the network so it is showing up as 2 of the same
            // This new computer that I am making needs to have some altered things...
            // Like the fact that it is the source, and that we don't know the destination

            broMessage.id_orig_h = broMessage.id_resp_h;
            broMessage.id_resp_h = "Null";

            broMessage.id_orig_p = broMessage.id_resp_p;
            broMessage.id_resp_p = -1;
            yield return null;

            StartCoroutine(NewComputerEnum(broMessage));
        }
        yield return null;

    }

}
