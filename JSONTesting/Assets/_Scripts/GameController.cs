using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// This class will do a lot of important things. 
/// It will have a way to STORE all my current 'computers' on the network
/// It will have a method to check if a
/// </summary>
public class GameController : MonoBehaviour {

    #region Fields

    // This will be used to randomly position the computers within this area. 
    // The max range will be whatever number you put in, and the min will be 
    // the negative of that. So if you enter 100,100,100, then a random
    // position will be within -100 and 100 on each axis.
    public Vector3 boundries;

    private Vector3 tempPosition;
    public Dictionary<string, GameObject> computersDict; // A dictionary of all the computers I have
    #endregion

    void Start ()
    {
        computersDict = new Dictionary<string, GameObject>();     
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// This will take in an IP address, and CHECK if we have
    /// seen it before. 
    /// </summary>
    /// <param name="jsonData">The data for me to check what PC's are on the network</param>
    public void CheckIP(Json_Data jsonData)
    {
        // Check all the given info from the json data with the info that I already have
        for (int i = 0; i < jsonData.hits.hits.Length; i++)
        {
            // Make sure that we are not null
            if(jsonData.hits.hits[i]._source.source.ip == null)
            {
                break;
            }

            // If my dictionary contains the IP address of this JSON info...
            if (computersDict.ContainsKey(jsonData.hits.hits[i]._source.source.ip))
            {
                // I want to check if there is a connection that I should add
                CheckConnections(jsonData.hits.hits[i]._source,
                    computersDict[jsonData.hits.hits[i]._source.source.ip]);
            }
            else
            {
                // If I do NOT have this IP in my dictionary, then make a new computer        
                NewComputer(jsonData.hits.hits[i]._source.source.ip, jsonData.hits.hits[i]._source);
            }
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// We have NOT seen this IP before, and we want to make
    /// a new one
    /// </summary>
    private void NewComputer(string ipAddr, Source data)
    {
        GameObject obj = ObjectPooler.current.GetPooledObject();

        if (obj == null) return;

        // Make a random transform within the paramters of this space
        tempPosition = new Vector3(
            Random.Range(-boundries.x, boundries.x),
            Random.Range(-boundries.y, boundries.y),
            Random.Range(-boundries.z, boundries.z));

        obj.transform.position = tempPosition;
        obj.transform.rotation = Quaternion.identity;

        // Set the DATA on this gameobject to the data from the JSON data
        obj.GetComponent<Computer>().SetData(data);
        obj.SetActive(true);
        computersDict.Add(ipAddr, obj);

    }

    /// <summary>
    /// Author: Ben Hoffman
    /// This is for when there is a computer that we already have the SOURCE
    /// IP for no the network. This will check if we already have the DESTINATION
    /// IP of that object or not. 
    /// </summary>
    /// <param name="data">The data of that commputer with the same source IP so I can check the dest.</param>
    /// <param name="checkMe">The game object that I already have, from my dicionary</param>
    private void CheckConnections(Source data, GameObject checkMe)
    {
        // I need to check if my destination is a source  address, which would mean that it is in my dictionary already
        if(data.dest.ip== null)
        {
            return;
        }

        // If we do already have the destination on the network, then connect them
        if (computersDict.ContainsKey(data.dest.ip))
        {
            // We have this IP on our network already, add the connection to each PC
            checkMe.GetComponent<Computer>().AddConnectedPC(computersDict[data.dest.ip]);
            computersDict[data.dest.ip].GetComponent<Computer>().AddConnectedPC(checkMe);
        }
        else
        {
            // Here is the problem... I am making a new computer with the same data
            // as the one already on the network so it is showing up as 2 of the same
            // This new computer that I am making needs to have some altered things...
            // Like the fact that it is the source, and that we don't know the destination
            
            data.source.ip = data.dest.ip;
            data.dest.ip = "Null";

            data.source.port = data.dest.port;
            data.dest.port = -1;
            
            NewComputer(data.source.ip, data);
        }

    }
	
}
