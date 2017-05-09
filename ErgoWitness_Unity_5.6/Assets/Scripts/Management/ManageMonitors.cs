using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will hold an array of moniotr objects
/// </summary>
public class ManageMonitors : MonoBehaviour {

    public MonitorObject[] monitors;

    /// <summary>
    /// Set the server IP address's of all the monitors
    /// </summary>
    public void SetServerIP(string serverIP)
    {
        // Loop through all the monitors
        for(int i = 0; i < monitors.Length; i++)
        {
            // Change their IP
            monitors[i].UpdateServerIP(serverIP);
        }
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

    public string GenerateTimeStamp(int hour, int minute, int second)
    {

        string timeStamp = System.DateTime.Today.Year.ToString() + "-";

        // Make sure we have proper format on the month
        if (System.DateTime.Today.Month < 10)
        {
            timeStamp += "0" + System.DateTime.Today.Month.ToString() + "-";
        }
        else
        {
            timeStamp += System.DateTime.Today.Month.ToString() + "-";
        }
        // Handle the day
        if (System.DateTime.Today.Day < 10)
        {
            timeStamp += "0" + System.DateTime.Today.Day.ToString();
        }
        else
        {
            timeStamp += System.DateTime.Today.Day.ToString();
        }

        if (hour < 10)
        {
            timeStamp += "T0" + hour + ":";
        }
        else
        {
            timeStamp += "T" + hour + ":";
        }

        if (minute < 10)
        {
            timeStamp += "0" + minute + ":";
        }
        else
        {
            timeStamp += minute + ":";
        }

        if (second < 10)
        {
            timeStamp += "0" + second;

        }
        else
        {
            timeStamp += second;
        }

        timeStamp += ".000Z";

        return timeStamp;

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

    #endregion

}
