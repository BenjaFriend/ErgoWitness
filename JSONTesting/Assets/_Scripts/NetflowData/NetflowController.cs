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
    public void CheckPacketbeatData(Source_Packet packetbeatSource)
    {
        if(packetbeatSource == null)
        {
            return;
        }
        // Break out if something is null
        if (packetbeatSource.packet_source.ip == null || packetbeatSource.dest.ip == null)
        {
            return;
        }

        if (packetbeatSource.packet_source.ip == null || packetbeatSource.dest.ip == null)
        {
            return;
        }
        
        if(packetbeatSource.sourceIpInt == 0 || packetbeatSource.destIpInt == 0)
        {
            return;
        }

        // If the source and destination IP's are known:
        if (GameController.currentGameController.CheckDictionary(packetbeatSource.sourceIpInt) &&
            GameController.currentGameController.CheckDictionary(packetbeatSource.destIpInt))
        {
            // Increase the emmision of the computer here, because we
            // obviously see some activity with it if we are checking
            GameController.ComputersDict[packetbeatSource.sourceIpInt].GetComponent<IncreaseEmission>().AddHit();

            // Then we can continue on and send out flow data out      
            SendFlow(packetbeatSource.sourceIpInt, packetbeatSource.destIpInt, packetbeatSource.transport);
        }
        else
        {
            Source newSource = new Source();

            // Set up the source data to properlly represent a computer that we don't yet have
            newSource.id_orig_h = packetbeatSource.packet_source.ip;
            newSource.id_orig_p = packetbeatSource.packet_source.port;

            // Set the destination data so that the game controller can read it
            newSource.id_resp_h = packetbeatSource.dest.ip;

            // Set the destiation port data
            newSource.id_resp_p = packetbeatSource.dest.port;

            // Set the protocol so that the game controller can read it
            packetbeatSource.proto = packetbeatSource.transport;

            //Set the integer values for this object
            NetworkMonitor.currentNetworkMonitor.SetIntegerValues(newSource);

            // Add them to the network, and wait for that to finish:
            GameController.currentGameController.CheckIp(newSource);

            // Now send the data since we know about it:
            SendFlow(packetbeatSource.sourceIpInt, packetbeatSource.destIpInt, packetbeatSource.transport);
        }
    }


    /// <summary>
    /// Do the setup for the netflow object
    /// </summary>
    /// <param name="sourceIP">The source IP</param>
    /// <param name="destIP">The destination IP</param>
    /// <param name="protocol">the protocol of the object</param>
    private void SendFlow(int sourceIP, int destIP, string protocol)
    {
        // Grab a pooled object
        obj = netflowObjectPooler.GetPooledObject();

        // Return if the object is null
        if (obj == null)
            return;

        // Get the netflow component of that
        tempNet = obj.GetComponent<NetflowObject>();

        // Return if we are null
        if(tempNet == null)
            return;

        if(GameController.currentGameController.GetTransform(sourceIP) == null ||
           GameController.currentGameController.GetTransform(destIP) == null)
        {
            return;
        }

        // Set the source of the netflow 
        tempNet.SourcePos = GameController.currentGameController.GetTransform(sourceIP);

        // Set the protocol of the netflow 
        tempNet.Protocol = protocol;

        // Set the destination of the netflow obj, which also start the movement 
        tempNet.DestinationPos = GameController.currentGameController.GetTransform(destIP);

        obj.SetActive(true);

        // Connect the computers, because now they have talked to each other
        GameController.currentGameController.ConnectComputers(sourceIP, destIP);
    }
}
