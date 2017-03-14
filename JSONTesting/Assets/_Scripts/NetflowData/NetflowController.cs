using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will manage the netflow data objects that we have
/// and handle the comparisons and what not that we have
/// </summary>
public class NetflowController : MonoBehaviour
{
    #region Fields
    public static NetflowController currentNetflowController;
    public StreamingInfo_UI streamingUI;

    // The different colors for the protocols
    public Material tcpMat;
    public Material udpColor;
    public Material httpColor;
    public Material defaultColor;

    public Gradient tcpTrailColor;
    public Gradient udpTrailColor;
    public Gradient httpTrailColor;
    public Gradient defaultTrailColor;


    public ObjectPooler netflowObjectPooler;    // The object pooler for the netflow object

    private GameObject obj; // This is better for memory
    private NetflowObject tempNet; // Temp object for when we alter stuff

    #endregion

    /// <summary>
    /// Set the static reference to this
    /// </summary>
    private void Awake()
    {
        if (currentNetflowController == null)
        {
            currentNetflowController = this;
        }
        else if (currentNetflowController != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
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
        if (DeviceManager.currentDeviceManager.CheckDictionary(packetbeatSource.sourceIpInt) &&
            DeviceManager.currentDeviceManager.CheckDictionary(packetbeatSource.destIpInt))
        {
            // Increase the emmision of the computer here, because we
            // obviously see some activity with it if we are checking
            DeviceManager.ComputersDict[packetbeatSource.sourceIpInt].GetComponent<IncreaseEmission>().AddHit();

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
            ManageMonitors.currentMonitors.SetIntegerValues(newSource);

            // Add them to the network, and wait for that to finish:
            DeviceManager.currentDeviceManager.CheckIp(newSource);

            // Now send the data since we know about it:
            SendFlow(packetbeatSource.sourceIpInt, packetbeatSource.destIpInt, packetbeatSource.transport);

            // Tell the streaming UI about this
            streamingUI.AddInfo(packetbeatSource);
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

        if(DeviceManager.currentDeviceManager.GetTransform(sourceIP) == null ||
           DeviceManager.currentDeviceManager.GetTransform(destIP) == null)
        {
            return;
        }

        // Set the source of the netflow 
        tempNet.SourcePos = DeviceManager.currentDeviceManager.GetTransform(sourceIP);

        // Set the protocol of the netflow 
        tempNet.Protocol = protocol;

        // Set the color of the temp net object
        SetColor(tempNet);

        // Set the destination of the netflow obj, which also start the movement 
        tempNet.DestinationPos = DeviceManager.currentDeviceManager.GetTransform(destIP);

        // Set the object as active in the hierachy, so that the object pooler
        // Knows not to 
        obj.SetActive(true);

        // Connect the computers, because now they have talked to each other
        DeviceManager.currentDeviceManager.ConnectComputers(sourceIP, destIP);
    }

    /// <summary>
    /// Set the trail material for the given object
    /// </summary>
    /// <param name="objToSet"></param>
    private void SetColor(NetflowObject objToSet)
    {
        // Change to the proper material
        switch (objToSet.Protocol)
        {
            case ("tcp"):
                objToSet.ProtoMaterial = tcpMat;
                objToSet.SetColor(tcpTrailColor);
                objToSet.LineDrawColor = Color.red ;
                break;
            case ("udp"):
                objToSet.ProtoMaterial = udpColor;
                objToSet.SetColor(udpTrailColor);
                objToSet.LineDrawColor = Color.cyan;

                break;
            case ("http"):
                objToSet.ProtoMaterial = httpColor;
                objToSet.SetColor(httpTrailColor);
                objToSet.LineDrawColor = Color.green;

                break;
            default:
                // Set the material of the single node/head of the particle system
                objToSet.ProtoMaterial = defaultColor;
                // Set the Trail particles color
                objToSet.SetColor(defaultTrailColor);
                objToSet.LineDrawColor = Color.gray;


                break;
        }
    }
}
