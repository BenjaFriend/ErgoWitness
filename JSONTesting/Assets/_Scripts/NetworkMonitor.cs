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
    public enum packetName { packetbeat, filebeat }

    public string serverIP;
    public packetName packetType;
    public Text current_Index_Text;
    public Text statusText;
    public Color runningColor;
    public Color stoppedColor;

    public bool keepGoing = true;
    // The URL of my ELK server
    private string elk_url;
    private string JSON_String = "";        // The string that represents the JSON data
    private Json_Data dataObject;           // The actual JSON data class 
    private Bro_Json broDataObj;            // The bro data that we might have

    private string queryString;
    private GameController gameControllerObj; // The game controller 

    // These are fields for my web request so that I am not making new objects in mem
    // With each call
    private WebRequest request;
    private HttpWebResponse response;
    private Stream requestStream;
    private StreamReader reader;
    private byte[] buffer;
    private string bufferResult;
    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose: Just start the coroutine that will constantly find the data
    /// </summary>
    void Start()
    {
        queryString = File.ReadAllText(Application.streamingAssetsPath + "/gimmeData.json");

        gameControllerObj = GameObject.FindObjectOfType<GameController>();

        // Set up my URL to get info from
        SetUpURL();

        // Find the latest index name and make my URL, or maybe get all the indexes and ask the
        // user which one they want to use
        StartCoroutine(SetJsonData());

        statusText.text = "Monitor Status: Running";
        statusText.CrossFadeColor(runningColor, 0.3f, true, true);

        current_Index_Text.text = elk_url;
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To set up the URL that I will be using 
    /// to get my JSON data by getting the current date and matching
    /// it to the index
    /// </summary>
    private void SetUpURL()
    {
        elk_url = "";
        // Add the packet type
        elk_url = "http://" + serverIP + ":9200/";
        switch (packetType)
        {
            case (packetName.packetbeat):
                elk_url += "packetbeat-";
                break;
            case (packetName.filebeat):
                elk_url += "filebeat-";
                break;
        }

        elk_url += DateTime.Today.Year.ToString() + ".";
        // Make sure we have proper format on the month
        if (DateTime.Today.Month < 10)
        {
            elk_url += "0" + DateTime.Today.Month.ToString() + ".";
        }
        else
        {
            elk_url +=  DateTime.Today.Month.ToString() + ".";
        }
        if(DateTime.Today.Day < 10)
        {
            elk_url += "0" + DateTime.Today.Day.ToString() + "/_search?pretty=true";
        }
        else
        {
            elk_url += DateTime.Today.Day.ToString() + "/_search?pretty=true";
        }
    }


    /// <summary>
    /// Author: Ben Hoffman
    /// This will allow me to get the most recent event
    /// from the ELK server, in a string that can then be parsed 
    /// by the Unity JsonUtility. This is a co routine so that
    /// I don't get framerate drops if it takes too long
    /// to download
    /// </summary>
    private IEnumerator SetJsonData()
    {
        // Make a new web request with the .net WebRequest
        request = WebRequest.Create(elk_url);
        yield return null;

        request.ContentType = "application/json";
        request.Method = "POST";
        buffer = Encoding.GetEncoding("UTF-8").GetBytes(queryString);
        var bufferResult = System.Convert.ToBase64String(buffer);

        // Put this yield here so that I get higher FPS
        yield return null;

        requestStream = request.GetRequestStream();
        requestStream.Write(buffer, 0, buffer.Length);
        requestStream.Close();
        // Again this is about getting higher FPS
        yield return null;

        response = (HttpWebResponse)request.GetResponse();
        // Wait until we have all of the data we need from the response to continue
        yield return requestStream = response.GetResponseStream();

        // Open the stream using a StreamReader for easy access.
        reader = new StreamReader(requestStream);
        // Set my string to the response from the website
        JSON_String = reader.ReadToEnd();

        yield return null;
        // Cleanup the streams and the response.
        reader.Close();
        requestStream.Close();
        response.Close();
        yield return null;

        // As long as we are not null, put this in as real C# data
        if (JSON_String != null)
        {
            // Wait until we finish converting the string to JSON data to continue
            yield return StartCoroutine(PacketbeatToJson());

            // If we have a message from a filebeat, then send that to a different JSON parser
            if(dataObject.hits.hits[0]._source.message != null)
            {
                yield return StartCoroutine(FilebeatToJson(dataObject.hits.hits[0]._source.message));
            }

            yield return null;

            // Give my game controller the JSON data to sort out if there is a new computer on it or not
            //gameControllerObj.CheckIP(dataObject, broDataObj);
            StartCoroutine(gameControllerObj.CheckIpEnum(dataObject, broDataObj));
        }

        dataObject = null;
        broDataObj = null;
        // As long as we didn't say to stop yet
        if (keepGoing)
        {
            yield return null;

            // Start this again
            StartCoroutine(SetJsonData());
        } 
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Take the Json string that we just got from the website,
    /// and use the JsonUtility to make it JsonData. After that 
    /// it will send it to the game controller
    /// </summary>
    private IEnumerator PacketbeatToJson()
    {
        // Use the JsonUtility to send the string of data that I got from the server, to a data object
        yield return dataObject = JsonUtility.FromJson<Json_Data>(JSON_String);
    }

    private IEnumerator FilebeatToJson(string message)      
    {
        // Use the JsonUtility to send the string of data that I got from the server, to a data object
        yield return broDataObj = JsonUtility.FromJson<Bro_Json>(message);
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Give the user the option to stop monitoring the network
    /// </summary>
    public void ToggleMonitoring()
    {
        if (keepGoing)
        {
            keepGoing = false;
            StopCoroutine(SetJsonData());
            statusText.text = "Monitor Status: Stopped";
            statusText.CrossFadeColor(stoppedColor, 0.3f, true, true);
        }
        else
        {
            keepGoing = true;
            StartCoroutine(SetJsonData());
            statusText.text = "Monitor Status: Running";
            statusText.CrossFadeColor(runningColor, 0.3f, true, true);

        }
    }

}
