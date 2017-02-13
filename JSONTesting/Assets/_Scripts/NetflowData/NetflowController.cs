using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will manage the netflow data objects that we have
/// and handle the comparisons and what not that we have
/// </summary>
public class NetflowController : MonoBehaviour {

    public ObjectPooler netflowObjectPooler;

    private GameController gameControllerObj;
    private GameObject obj; // This is better for memory
    private NetflowObject tempNet; // Temp object for when we alter stuff

    void Awake()
    {
        gameControllerObj = FindObjectOfType<GameController>(); 
    }


    public void CheckPacketbeatData()
    {
        // Do we have this object in our dictionary? 
        if (gameControllerObj.CheckDictionary(""))
        {
            // Set up my data
        }
    }


    /// <summary>
    /// Do the setup for the netflow object
    /// </summary>
    /// <param name="sourceIP">The source IP</param>
    /// <param name="destIP">The destination IP</param>
    /// <param name="protocol">the protocol of the object</param>
    private void SendFlow(string sourceIP, string destIP, string protocol)
    {
        // Grab a pooled object
        GameObject obj = netflowObjectPooler.GetPooledObject();
        // Get the netflow component of that
        tempNet = obj.GetComponent<NetflowObject>();
        // As long as that component is not null...
        if(tempNet != null)
        {
            // Set the source of the netflow 
            tempNet.Source = gameControllerObj.GetTransform(sourceIP);
            // Set the destination of the netflow obj       
            tempNet.Destination = gameControllerObj.GetTransform(destIP);
            // Set the protocol of the netflow 
            tempNet.Protocol = protocol;

            // Set it to active in the hierachy
            obj.SetActive(true);
        }
        
    }
}
