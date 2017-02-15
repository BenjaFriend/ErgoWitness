using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will manage the netflow data objects that we have
/// and handle the comparisons and what not that we have
/// </summary>
public class NetflowController : MonoBehaviour {

    public static NetflowController currentNetflowController;

    public ObjectPooler netflowObjectPooler;    // The object pooler for the netflow object

    private GameObject obj; // This is better for memory
    private NetflowObject tempNet; // Temp object for when we alter stuff

    /// <summary>
    /// Set the static reference to this
    /// </summary>
    private void Awake()
    {
        currentNetflowController = this;
    }

    /// <summary>
    /// If we have this computer, then send the visual aspect of the flow data
    /// Otherwise, add it to the network
    /// </summary>
    /// <param name="packetbeatSource">The netflow source data</param>
    public void CheckPacketbeatData(Source packetbeatSource)
    {
        // Break out if something is null
        if (packetbeatSource.packet_source.ip == null || packetbeatSource.dest.ip == null)
        {
            return;
        }

        if (packetbeatSource.packet_source.ip == "" || packetbeatSource.dest.ip == "")
        {
            return;
        }

        // If the source and destination IP's are known...
        if (GameController.currentGameController.CheckDictionary(packetbeatSource.packet_source.ip) &&
            GameController.currentGameController.CheckDictionary(packetbeatSource.dest.ip))
        {
            // Then we can continue on and send out flow data out      
            SendFlow(packetbeatSource.packet_source.ip, packetbeatSource.dest.ip, packetbeatSource.transport);
        }
        else
        {
            // Set up the source data to properlly represent a computer that we don't yet have
            packetbeatSource.id_orig_h = packetbeatSource.packet_source.ip;
            packetbeatSource.id_orig_p = packetbeatSource.packet_source.port;

            // Set the destination data so that the game controller can read it
            packetbeatSource.id_resp_h = packetbeatSource.dest.ip;
            packetbeatSource.id_resp_p = packetbeatSource.dest.port;

            // Set the protocol so that the game controller can read it
            packetbeatSource.proto = packetbeatSource.transport;

            // Add them to the network, and wait for that to finish:
            GameController.currentGameController.NewComputerEnum(packetbeatSource);

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
        obj = netflowObjectPooler.GetPooledObject();
        // Get the netflow component of that
        tempNet = obj.GetComponent<NetflowObject>();

        // Return if we are null
        if(tempNet == null)
            return;
 

        // Set the source of the netflow 
        tempNet.Source = GameController.currentGameController.GetTransform(sourceIP);
        // Set the protocol of the netflow 
        tempNet.Protocol = protocol;
        // Set the destination of the netflow obj, which also start the movement 
        tempNet.Destination = GameController.currentGameController.GetTransform(destIP);

    }
}
