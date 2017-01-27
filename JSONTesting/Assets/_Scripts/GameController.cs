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
    public GameObject computerPrefab;

    private Dictionary<string, GameObject> computersDict;
    private GameObject temp;
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
            if(jsonData.hits.hits[i]._source.ip == null)
            {
                break;
            }

            // If my dictionary contains the IP address of this JSON info...
            if (computersDict.ContainsKey(jsonData.hits.hits[i]._source.ip))
            {
                // Then I know that I have had this before, and I need to check the client IP
                // If the client IP is the same then do NOTHING, but if it is different, then 
                // create a new computer on the network with that IP
                CheckClient(jsonData.hits.hits[i]._source);
            }
            else
            {
                // If I do NOT have this IP in my dictionary, then make a new computer and add
                // it to the dictionary                
                NewComputer(jsonData.hits.hits[i]._source.ip, jsonData.hits.hits[i]._source);
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
        // Instantiate a new computer object
        temp = (GameObject)Instantiate(computerPrefab, transform);
        // Set the DATA on this gameobject to the data from the JSON data
        temp.GetComponent<Computer>().ComputerSourceInfo = data;
        // Actually add it to my list of computers
        computersDict.Add(ipAddr, temp);
    }


    /// <summary>
    /// Author: Ben Hoffman
    /// We have seen this IP before, and we want to see if the DESTINATION IP
    /// is NEW. IF IT IS, then make a new computer, and ADD THAT COMPUTER to the
    /// list of connected computers to both
    /// </summary>
    private void CheckClient(Source data)
    {
        if(data.client_ip== null)
        {
            return;
        }

        if (computersDict.ContainsKey(data.client_ip))
        {
            // We have this computer on the network, CONNECT IT to the client on the network

        }
        else
        {
            NewComputer(data.client_ip, data);
            // Add the connected IP's together on each computer
        }

    }
	
}
