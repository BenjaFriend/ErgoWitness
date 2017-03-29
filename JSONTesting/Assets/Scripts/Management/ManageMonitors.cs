using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will hold an array of moniotr objects
/// </summary>
public class ManageMonitors : MonoBehaviour {

    public MonitorObject[] monitors;
    public static ManageMonitors currentMonitors;

    /// <summary>
    /// Set the static reference to this object
    /// </summary>
    void Awake()
    {
        // Make sure that this is the only one of these components in the scene
        if (currentMonitors == null)
        {
            currentMonitors = this;
        }
        else if (currentMonitors != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Start the monitoring on all of the 
    /// </summary>
    public void StartMonitoringObjects()
    {
        // Loop through and start all the moniotrs
        for(int i = 0; i < monitors.Length; i++)
        {
            monitors[i].StartMonitor();
        }
    }

    /// <summary>
    /// Stop the monitors that are running
    /// </summary>
    public void StopMonitor()
    {
        // Loop through and start all the moniotrs
        for (int i = 0; i < monitors.Length; i++)
        {
            monitors[i].StopMonitor();
        }
    }

    #region String to integer conversion stuff

    /// <summary>
    /// Use Bit conversion to send the IP address to an integer
    /// </summary>
    /// <param name="ipAddr"></param>
    /// <returns></returns>
    private int IpToInt(string ipAddr)
    {
        if (ipAddr == null) return 0;

        return System.BitConverter.ToInt32(System.Net.IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
    }

    /// <summary>
    /// Take in a source object, and set it's integer values
    /// </summary>
    /// <param name="FilebeatSource"></param>
    public void SetIntegerValues(Source FilebeatSource)
    {
        // Calculate the INTEGER version of the SOURCE IP address
        FilebeatSource.sourceIpInt =
            IpToInt(FilebeatSource.id_orig_h);

        // Calculate the INTEGER version of the DESTINATION IP address
        FilebeatSource.destIpInt =
            IpToInt(FilebeatSource.id_resp_h);
    }

    /// <summary>
    /// Take in a source object, and set it's integer values
    /// </summary>
    /// <param name="FilebeatSource"></param>
    public void SetIntegerValues(Source_Packet PacketbeatSource)
    {
        // Calculate the INTEGER version of the SOURCE IP address
        PacketbeatSource.sourceIpInt =
            IpToInt(PacketbeatSource.packet_source.ip);

        // Calculate the INTEGER version of the DESTINATION IP address
        PacketbeatSource.destIpInt =
            IpToInt(PacketbeatSource.dest.ip);
    }

    #endregion

}
