﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine.UI;

/// <summary>
/// Author: Ben Hoffman
/// This class will be the main controller for the network monitoring in this visualization.
/// It will send HTTP GET requests to my ELK server, and use Unity's JsonUtility to 
/// put the data into a data object in C#. Then that data will be analyzed to see 
/// IF that IP address has been seen here before, and if has, then we will tell our computer
/// controller to make a new computer with the given information. 
/// </summary>
public class NetworkMonitor : MonoBehaviour
{

    #region Fields
    public static NetworkMonitor currentNetworkMonitor;

    public string serverIP;
    public Text current_Index_Text;
    public Text statusText;
    public Color runningColor;
    public Color stoppedColor;
    public bool keepGoing = false;            // If we want to keep going

    // The URL of my ELK server
    private string elk_url_filebeat;        // The filebeat index
    private string elk_url_packetbeat;      // The packetbeat index
    private Json_Data dataObject;           // The actual JSON data class 
    private Packetbeat_Json_Data packetDataObj;

    // The data of the post request
    private byte[] broPostData;        // The post data that we are using
    private byte[] packetPostData;        // The post data that we are using

    private Dictionary<string, string> headers;

    private string broHeaderString;
    private string packetHeaderString;

    private string lastFlowRecived;
    private string lastFilebeatRecieved;

    public string ServerIP { get { return serverIP; } set { serverIP = value; StartMonitoring();  } }
    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose: Just start the coroutine that will constantly find the data
    /// </summary>
    void Start()
    {
        // Set the static reference
        currentNetworkMonitor = this;

        // Initalize the data objects that I will be using
        dataObject = new Json_Data();
        packetDataObj = new Packetbeat_Json_Data();

        // Initialize the headers
        headers = new Dictionary<string, string>();

        // Add in content type:
        headers["Content-Type"] = "application/json";

        // Read in the query that I will use for filebeat from streaming assets
        string broHeaderString = File.ReadAllText(Application.streamingAssetsPath + "/bro_Headers.json");

        // Read in the query that I will use for packetbeat from streaming assets
        string packetHeaderString = File.ReadAllText(Application.streamingAssetsPath + "/packetbeat_Headers.json");

        // Get the post data that I will be using, since it will always be the same
        broPostData = Encoding.GetEncoding("UTF-8").GetBytes(broHeaderString);

        // Get the post data for packetbeat
        packetPostData = Encoding.GetEncoding("UTF-8").GetBytes(packetHeaderString);

        // Initalize the strings to not get a null ref exception
        lastFlowRecived = "";
        lastFilebeatRecieved = "";
    }

    /// <summary>
    /// Called on the setting of the IP address
    /// </summary>
    private void StartMonitoring()
    {
        // Set up my URL to get info from
        SetUpURL();

        //Start monitoring packetbeat and filebeat
        StartCoroutine(PostJsonData(elk_url_filebeat, false));
        StartCoroutine(PostJsonData(elk_url_packetbeat, true));
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To set up the URL that I will be using 
    /// to get my JSON data by getting the current date and matching
    /// it to the index
    /// </summary>
    private void SetUpURL()
    {
        // Add the port and packet type
        elk_url_filebeat = "http://" + serverIP + ":9200/filebeat-";
        // Set up the packetbeat
        elk_url_packetbeat = "http://" + serverIP + ":9200/packetbeat-";

        // Set up the year
        string dateUrl = DateTime.Today.Year.ToString() + ".";

        // Make sure we have proper format on the month
        if (DateTime.Today.Month < 10)
        {
            dateUrl += "0" + DateTime.Today.Month.ToString() + ".";
        }
        else
        {
            dateUrl += DateTime.Today.Month.ToString() + ".";
        }
        // Handle the day
        if (DateTime.Today.Day < 10)
        {
            dateUrl += "0" + DateTime.Today.Day.ToString() + "/_search?pretty=true";
        }
        else
        {
            dateUrl += DateTime.Today.Day.ToString() + "/_search?pretty=true";
        }

        // Add the date to each of the URL's
        elk_url_filebeat += dateUrl;
        elk_url_packetbeat += dateUrl;
    }

    /// <summary>
    /// Make a post request to the specified URL with the JSON
    /// query from the streaming assets folder.
    /// </summary>
    /// <param name="url">The URL that we want to make a post request to</param>
    /// <param name="isFlowData">If this is flow data or not, determines if we send it to the netflow controller
    /// or the game controller</param>
    /// <returns>The data downloaded from the server</returns>
    private IEnumerator PostJsonData(string url, bool isFlowData)
    {
        // Clear the headers:
        headers.Clear();

        // Add in content type:
        headers["Content-Type"] = "application/json";

        // Create a web request object
        WWW myRequest;

        if (!isFlowData)
        {
            // Read in the query that I will use for filebeat from streaming assets
            broHeaderString = File.ReadAllText(Application.streamingAssetsPath + "/bro_Headers.json");
            // Get the post data that I will be using, since it will always be the same
            broPostData = Encoding.GetEncoding("UTF-8").GetBytes(broHeaderString);

            // Start up the reqest for FILEBEAT:
            myRequest = new WWW(url, broPostData, headers);
        }
        else
        {
            // Read in the query that I will use for packetbeat from streaming assets
            packetHeaderString = File.ReadAllText(Application.streamingAssetsPath + "/packetbeat_Headers.json");
            // Get the post data for packetbeat
            packetPostData = Encoding.GetEncoding("UTF-8").GetBytes(packetHeaderString);

            // Start up the reqest for PACKETBEAT:
            myRequest = new WWW(url, packetPostData, headers);
        }

        // Yield until it's done:
        yield return myRequest;

        // Check if we got an error in our request or not
        if (myRequest.error != null || myRequest.text == null)
        {
            Debug.Log("THERE WAS A REQUEST ERROR: " + myRequest.error);

            if (myRequest.text != null)
                Debug.Log(myRequest.text);

            yield break;
        }

        // Actually send the JSON data to either the netflow controller or the game controller
        if (isFlowData)
        {
            // Use the JSON utility with the packetbeat data to parse this text
            packetDataObj = JsonUtility.FromJson<Packetbeat_Json_Data>(myRequest.text);

            // Send to Netflow Controller
            CheckPacketbeat();
        }
        else
        {
            // Use the JsonUtility to send the string of data that I got from the server, to a data object
            dataObject = JsonUtility.FromJson<Json_Data>(myRequest.text);

            // Send to GameController
            CheckFilebeat();
        }

        // As long as we didn't say to stop yet
        if (keepGoing)
        {
            // Start this again
            StartCoroutine(PostJsonData(url, isFlowData));
        }
    }

    /// <summary>
    /// Check if the ID of this data is the same as the previous one,
    /// if it is then return out of this method. Otherwise, send the 
    /// data to the Netflow Controller
    /// </summary>
    private void CheckPacketbeat()
    {
        // Make sure that our data is not null
        if (packetDataObj == null || packetDataObj.hits.hits == null || packetDataObj.hits.hits.Length == 0)
        {
            return;
        }
        Debug.Log(packetDataObj.hits.hits[0]._source.start_time);
        // If we have seen this data before, then return out of this method
        // Because that means it is esxactly the same, and no new data has been
        // pushed to our data base's stack
        if (lastFlowRecived == packetDataObj.hits.hits[0]._id)
        {
            // If it is then break out and don't bother doing anything, this should
            // Save on processing power
            return;
        }

        // It is new, so set the thing we use to check it to the current ID
        lastFlowRecived = packetDataObj.hits.hits[0]._id;

        // Loop through our data and send that data to the netflow controller
        for (int i = 0; i < packetDataObj.hits.hits.Length; i++)
        {
            // Set the integer IP values of this object
            SetIntegerValues(packetDataObj.hits.hits[i]._source);

            // As long as what we got from those IP's is valid:
            if(packetDataObj.hits.hits[i]._source.destIpInt != 0 && packetDataObj.hits.hits[i]._source.sourceIpInt != 0)
            {
                // Send the data to the netflow controller
                NetflowController.currentNetflowController.CheckPacketbeatData(packetDataObj.hits.hits[i]._source);
            }
        }

        // Keep track of the most recent timestamp, and write it into the file
        WriteNewHeaders.currentWriter.LastTimestamp_Packetbeat = packetDataObj.hits.hits[0]._source.start_time;

    }

    /// <summary>
    /// Check if the data that was given is the same as the previous,
    /// if it is then break out of this method. Otherwise, send that
    /// data to the game controller and add the computer to the network
    /// </summary>
    private void CheckFilebeat()
    {
        // Make sure that our data is not null
        if (dataObject == null || dataObject.hits.hits == null || dataObject.hits.hits.Length == 0)
        {
            return;
        }
        
        // If this data is the same as the last one that we did, 
        // then we don't need to look at any of the other stuff
        if (lastFilebeatRecieved == dataObject.hits.hits[0]._id)
        {
            // If it is then break out and don't bother doing anything, this should
            // Save on processing power
            return;
        }

        // This is a new data object, so keep track of the most recent ID
        lastFilebeatRecieved = dataObject.hits.hits[0]._id;
        
        // Send the data to the game controller for all of our hits
        for (int i = 0; i < dataObject.hits.hits.Length; i++)
        {
            // Set the integer IP values if this source
            SetIntegerValues(dataObject.hits.hits[i]._source);

            // Send the bro data to the game controller, and add it to the network
            GameController.currentGameController.CheckIp(dataObject.hits.hits[i]._source);
        }

        // Keep track of the most recent timestamp, and write a new one
        WriteNewHeaders.currentWriter.LastTimestamp_Filebeat = dataObject.hits.hits[0]._source.timestamp;

    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Give the user the option to stop monitoring the network
    /// Collect the gabrage because they would not notice it happening
    /// and it could possibly help the situation. 
    /// </summary>
    public void ToggleMonitoring()
    {
        // Collect garbage now because people wouldn't notice
        GC.Collect();

        if (keepGoing)
        {
            // Tell the method to stop
            keepGoing = false;
            // Stop all coroutines
            StopAllCoroutines();
            // Set up the UI
            statusText.text = "Monitor Status: Stopped";
            statusText.CrossFadeColor(stoppedColor, 0.3f, true, true);
        }
        else
        {
            // Tell the method to keep going
            keepGoing = true;

            // Stop all coroutines so that we cna start fresh
            StopAllCoroutines();

            //Start monitoring packetbeat and filebeat again
            StartCoroutine(PostJsonData(elk_url_filebeat, false));
            StartCoroutine(PostJsonData(elk_url_packetbeat, true));

            // Set up the monitoring text
            statusText.text = "Monitor Status: Running";
            statusText.CrossFadeColor(runningColor, 0.3f, true, true);
        }
    }

    /// <summary>
    /// Use Bit conversion to send the IP address to an integer
    /// </summary>
    /// <param name="ipAddr"></param>
    /// <returns></returns>
    private int IpToInt(string ipAddr)
    {
        if (ipAddr == null) return 0;

        return BitConverter.ToInt32(IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
    }

    private string ToString(int ipAddr)
    {
        return new IPAddress(BitConverter.GetBytes(ipAddr)).ToString();

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

}
