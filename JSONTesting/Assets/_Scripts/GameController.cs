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
    public GameObject computerPrefab;  // The computer prefab

    // This will be used to randomly position the computers within this area. 
    // The max range will be whatever number you put in, and the min will be 
    // the negative of that. So if you enter 100,100,100, then a random
    // position will be within -100 and 100 on each axis.
    public Vector3 boundries;

    private Vector3 tempPosition;
    public Dictionary<string, GameObject> computersDict; // A dictionary of all the computers I have
    private GameObject temp; // Use this for making new computers, so I can edit them
    #endregion

    void Start ()
    {
        // Load in my prefab from the resources folder
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
        // Make a random transform within the paramters of this space
        tempPosition = new Vector3(
            Random.Range(-boundries.x, boundries.x),
            Random.Range(-boundries.y, boundries.y),
            Random.Range(-boundries.z, boundries.z));

        // Instantiate a new computer object
        temp = (GameObject)Instantiate(computerPrefab, tempPosition, Quaternion.identity);
        // Set the DATA on this gameobject to the data from the JSON data
        temp.GetComponent<Computer>().ComputerSourceInfo = data;
        // Actually add it to my list of computers
        computersDict.Add(ipAddr, temp);

        // Check the connections to this PC
        CheckConnections(data, temp);
        // Release from memory
        temp = null;
    }


    /// <summary>
    /// Author: Ben Hoffman
    /// We have seen this IP before, and we want to see if the DESTINATION IP
    /// is NEW. IF IT IS, then make a new computer, and ADD THAT COMPUTER to the
    /// list of connected computers to both
    /// </summary>
    /// <param name="data">The data of that commputer</param>
    /// <param name="checkMe">The new computer that we just made</param>
    private void CheckConnections(Source data, GameObject checkMe)
    {
        // I need to check if my destination is a source  address, which would mean that it is in my dictionary already
        if(data.dest.ip== null)
        {
            return;
        }
        // There IS a connection if one of the PC's on the network has a DEST IP of the given PC's IP
        if (computersDict.ContainsKey(data.dest.ip))
        {
            // We have this IP on our network already, add the connection to the given pc
            checkMe.GetComponent<Computer>().AddConnectedPC(computersDict[data.dest.ip]);
        }
        else
        {
            NewComputer(data.dest.ip, data);
            // Add the connected IP's together on each computer
        }

    }
	
}
