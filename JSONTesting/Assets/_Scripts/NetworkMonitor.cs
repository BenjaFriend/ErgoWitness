using System;
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
public class NetworkMonitor : MonoBehaviour {

    #region Fields
    public string serverIP;
    public Text current_Index_Text;
    public Text statusText;
    public Color runningColor;
    public Color stoppedColor;
    public bool keepGoing = true;            // If we want to keep going

    // The URL of my ELK server
    private string elk_url_filebeat;        // The filebeat index
    private string elk_url_packetbeat;      // The packetbeat index
    private Json_Data dataObject;           // The actual JSON data class 

    private string queryString;     // The JSON data that we are sending with the GET request
    private byte[] postData;        // The post data that we are using
    Dictionary<string, string> headers; 
    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose: Just start the coroutine that will constantly find the data
    /// </summary>
    void Start()
    {
        headers = new Dictionary<string, string>();
        // Add in content type:
        headers["Content-Type"] = "application/json";

        // Get the post data from a streaming assets file
        queryString = File.ReadAllText(Application.streamingAssetsPath + "/gimmeData.json");

        // Set up my URL to get info from
        SetUpURL();

        // Get the post data that I will be using, since it will always be the same
        postData = Encoding.GetEncoding("UTF-8").GetBytes(queryString);

        // Start looking for filebeat data, NO this is not netflow data
        StartCoroutine(PostJsonData(elk_url_filebeat, false));
        // Start looking for packetbeat data, YES this is netflow data
        StartCoroutine(PostJsonData(elk_url_packetbeat, true));

        // Say that the monitor is running
        statusText.text = "Monitor Status: Running";
        statusText.CrossFadeColor(runningColor, 0.3f, true, true);
        // Show the current index that we are using
        current_Index_Text.text = elk_url_filebeat;
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
            dateUrl +=  DateTime.Today.Month.ToString() + ".";
        }
        // Handle the day
        if(DateTime.Today.Day < 10)
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
        // Build up the headers:
        headers.Clear();

        // Add in content type:
        headers["Content-Type"] = "application/json";

        // Start up the reqest:
        WWW myRequest = new WWW(url, postData, headers);

        // Yield until it's done:
        yield return myRequest;

        if(myRequest.error != null)
        {
            Debug.Log(myRequest.error);
            // The request is bad, there was a problem connecting to the server stop monitoring
            //ToggleMonitoring();
            // Break out of the coroutine
            //yield break;
        } 
        //Debug.Log(myRequest.text);
        if (myRequest.text == null) yield break;

        // Use the JsonUtility to send the string of data that I got from the server, to a data object
        dataObject = JsonUtility.FromJson<Json_Data>(myRequest.text);

        // Break if we have null data
        if (dataObject == null || dataObject.hits.hits == null)
        {
            yield break;
        }

        // Send the data to the game controller for all of our hits
        for (int i = 0; i < dataObject.hits.hits.Length; i++)
        {
            // If this is not flow data, then send it the bro controller
            if (!isFlowData)
            {
                // handle it being a none flow data object... so a device on the network
                GameController.currentGameController.CheckIpEnum(dataObject.hits.hits[i]._source);
            }
            else
            {
                // Handle it being flow data from packetbeat
                NetflowController.currentNetflowController.CheckPacketbeatData(dataObject.hits.hits[i]._source);
            }
        }

        // As long as we didn't say to stop yet
        if (keepGoing)
        {
            // Start this again
            StartCoroutine(PostJsonData(url, isFlowData));
        }
    }


    /// <summary>
    /// Author: Ben Hoffman
    /// Give the user the option to stop monitoring the network
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
            // Reset the headers
            headers = new Dictionary<string, string>();
            // Add in content type:
            headers["Content-Type"] = "application/json";

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

}
