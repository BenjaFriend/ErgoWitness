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

    // Particle head materials =========
    public Material tcpMat;  // Just take in one material and use the colors to generate them
    public Material udpMat;
    public Material httpMat;
    public Material dnsMat;
    public Material defaultMat;
    public Material AttackingBlueTeam_Material;

    // Particle system colors ============
    public Gradient tcpTrailColor;
    public Gradient udpTrailColor;
    public Gradient httpTrailColor;
    public Gradient defaultTrailColor;
    public Gradient dnsTrailColor;
    public Gradient AttackingBlueTeam_Gradient;

    // Line colors ======================
    public Color tcpColor;
    public Color udpColor;
    public Color httpColor;
    public Color dnsColor;
    public Color defaultColor;
    public Color AttackingBlueTeam_Color;

    public ObjectPooler netflowObjectPooler;    // The object pooler for the netflow object

    private GameObject obj; // This is better for memory
    private NetflowObject tempNet; // Temp object for when we alter stuff

    #endregion

    /// <summary>
    /// Set the static reference to this
    /// </summary>
    private void Awake()
    {
        // If there is not another controller in the scene...
        if (currentNetflowController == null)
        {
            // Set the static reference to this
            currentNetflowController = this;
        }
        // If there is another controller in scene...
        else if (currentNetflowController != this)
        {
            // Destroy this 
            Destroy(gameObject);
        }
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

        // If the source and destination IP's are known:
        if (!DeviceManager.currentDeviceManager.CheckDictionary(packetbeatSource.sourceIpInt) ||
            !DeviceManager.currentDeviceManager.CheckDictionary(packetbeatSource.destIpInt))
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
            newSource.proto = packetbeatSource.transport;

            //Set the integer values for this object
            ManageMonitors.currentMonitors.SetIntegerValues(newSource);

            // Add them to the network, and wait for that to finish:
            DeviceManager.currentDeviceManager.CheckIp(newSource);
        }


        // Then we can continue on and send out flow data out      
		SendFlow(packetbeatSource.sourceIpInt, packetbeatSource.destIpInt, packetbeatSource.transport);
    }


    /// <summary>
    /// This method will just take in the source and IP integers of something,
    /// and the transport. This will Make it so that you don't need packetbeat
    /// data to send connections. Since packetbeat in only netflow data(can't tell if
    /// something if DNS, HTTP, or SSH), this will allow you to send netflow like data
    /// with a bro log or snort. 
    /// </summary>
    /// <param name="sourceIP">The source IP int</param>
    /// <param name="destIP">the DEST IP int</param>
    /// <param name="transport">The type of transport (tcp, udp, http, ssh)</param>
    public void CheckPacketbeatData(int sourceIP, int destIP, string transport)
    {
        // If the source and destination IP's are known:
        if (!DeviceManager.currentDeviceManager.CheckDictionary(sourceIP) ||
            !DeviceManager.currentDeviceManager.CheckDictionary(destIP))
        {
            Source newSource = new Source();

            // Set up the source data to properlly represent a computer that we don't yet have
            newSource.sourceIpInt = sourceIP;

            // Set the destination data so that the game controller can read it
            newSource.destIpInt = destIP;

            // Set the protocol so that the game controller can read it
            newSource.proto = transport;

            //Set the integer values for this object
            //ManageMonitors.currentMonitors.SetIntegerValues(newSource);

            // Add them to the network, and wait for that to finish:
            DeviceManager.currentDeviceManager.CheckIp(newSource);
        }
    
        // Actually send the flow
        SendFlow(sourceIP, destIP, transport);

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
        //DeviceManager.currentDeviceManager.ConnectComputers(sourceIP, destIP);
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
                objToSet.HeadParticleMaterial = tcpMat;
                objToSet.SetColor(tcpTrailColor);
                objToSet.LineDrawColor = tcpColor;
                break;

            case ("udp"):
                objToSet.HeadParticleMaterial = udpMat;
                objToSet.SetColor(udpTrailColor);
                objToSet.LineDrawColor = udpColor;
                break;

            case ("http"):
                objToSet.HeadParticleMaterial = httpMat;
                objToSet.SetColor(httpTrailColor);
                objToSet.LineDrawColor = httpColor;
                break;

            case ("dns"):
                objToSet.HeadParticleMaterial = dnsMat;
                objToSet.SetColor(dnsTrailColor);
                objToSet.LineDrawColor = dnsColor;
                break;

            default:
                // Set the material of the single node/head of the particle system
                objToSet.HeadParticleMaterial = defaultMat;
                // Set the Trail particles color
                objToSet.SetColor(defaultTrailColor);
                objToSet.LineDrawColor = defaultColor;
                break;
        }
    }
}
