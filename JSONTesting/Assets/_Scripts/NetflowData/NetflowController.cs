using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will manage the netflow data objects that we have
/// and handle the comparisons and what not that we have
/// </summary>
public class NetflowController : MonoBehaviour {

    public ObjectPooler netflowObjectPooler;    // The object pooler for the netflow object

    private GameController gameControllerObj;
    private GameObject obj; // This is better for memory
    private NetflowObject tempNet; // Temp object for when we alter stuff

    /// <summary>
    /// Grab the game controler object
    /// </summary>
    void Awake()
    {
        // Get the game controller object in the sceen
        gameControllerObj = FindObjectOfType<GameController>(); 
    }

    /// <summary>
    /// If we have this computer, then send the visual aspect of the flow data
    /// Otherwise, add it to the network
    /// </summary>
    /// <param name="packetbeatSource">The netflow source data</param>
    public IEnumerator CheckPacketbeatData(Source packetbeatSource)
    {
        // Break out if something is null
        if (packetbeatSource.packet_source.ip == null || packetbeatSource.dest.ip == null)
        {
            yield break;
        }

        if (packetbeatSource.packet_source.ip == "" || packetbeatSource.dest.ip == "")
        {
            yield break;
        }

        // If the source and destination IP's are known...
        if (gameControllerObj.CheckDictionary(packetbeatSource.packet_source.ip) &&
            gameControllerObj.CheckDictionary(packetbeatSource.dest.ip))
        {
            // Then we can continue on and send out flow data out      
            SendFlow(packetbeatSource.packet_source.ip, packetbeatSource.dest.ip, packetbeatSource.transport);
        }
        else
        {
            // Set up the source data to properlly represent a computer that we don't yet have
            packetbeatSource.id_orig_h = packetbeatSource.packet_source.ip;
            packetbeatSource.id_orig_p = packetbeatSource.packet_source.port;

            packetbeatSource.id_resp_h = packetbeatSource.dest.ip;
            packetbeatSource.id_resp_p = packetbeatSource.dest.port;

            packetbeatSource.proto = packetbeatSource.transport;

            // Add them to the network, and wait for that to finish:
            gameControllerObj.NewComputerEnum(packetbeatSource);
            // Now send the data since we know about it:
            SendFlow(packetbeatSource.packet_source.ip, packetbeatSource.dest.ip, packetbeatSource.transport);
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
            // Set the protocol of the netflow 
            tempNet.Protocol = protocol;
            // Set the destination of the netflow obj, which also start the movement 
            tempNet.Destination = gameControllerObj.GetTransform(destIP);
        }
        
    }
}
