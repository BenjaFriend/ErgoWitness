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
    public void CheckIP(Data jsonData)
    {
        // Check all the given info from the json data with the info that I already have
        for (int i = 0; i < jsonData.hits.hits.Length; i++)
        {
            // If my dictionary contains the IP address of this JSON info...
            if (computersDict.ContainsKey(jsonData.hits.hits[i]._source.ip))
            {
                // Then I know that I have had this before, and I need to check the client IP
                // If the client IP is the same then do NOTHING, but if it is different, then 
                // create a new computer on the network with that IP
            }else
            {
                // If I do NOT have this IP in my dictionary, then make a new computer and add
                // it to the dictionary
            }
        }

    }

    /// <summary>
    /// Author: Ben Hoffman
    /// We have NOT seen this IP before, and we want to make
    /// a new one
    /// </summary>
    private void NewComputer(string ipAddr, Source newSource)
    {
        // Instantiate a new computer object
        GameObject temp = (GameObject)Instantiate(computerPrefab, transform);
        // Actually add it to my list of computers

    }


    /// <summary>
    /// Author: Ben Hoffman
    /// We have seen this IP before, and 
    /// </summary>
    private void AddConnection()
    {


    }
	
}
