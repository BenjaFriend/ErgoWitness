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
    private string elk_url;
    private string JSON_String = "";        // The string that represents the JSON data
    private Json_Data dataObject;           // The actual JSON data class 

    private string queryString;     // The JSON data that we are sending with the GET request
    private GameController gameControllerObj; // The game controller 

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
        StartCoroutine(PostJsonData());

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
        // Add the port and packet type
        elk_url = "http://" + serverIP + ":9200/filebeat-";

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
    /// Post the data to my server, and then get the JSON
    /// data that I want from it
    /// </summary>
    /// <returns></returns>
    private IEnumerator PostJsonData()
    {
        // Build up the headers:
        Dictionary <string, string> headers = new Dictionary<string, string>();

        // Add in content type:
        headers["Content-Type"] = "application/json";

        // Get the post data:
        byte[] postData = Encoding.GetEncoding("UTF-8").GetBytes(queryString);

        // Start up the reqest:
        WWW myRequest = new WWW(elk_url, postData, headers);

        // Yield until it's done:
        yield return myRequest;

        // Get the response (into your JSON_String field like above)
        JSON_String = myRequest.text;

        // As long as we are not null, put this in as real C# data
        if (JSON_String != null)
        {
            // Wait until we finish converting the string to JSON data to continue
            yield return StartCoroutine(StringToJson());
        }

        if (dataObject != null)
        {

            // Send the data to the game controller for all of our hits
            for (int i = 0; i < dataObject.hits.hits.Length; i++)
            {
                StartCoroutine(
                    gameControllerObj.CheckIpEnum(dataObject.hits.hits[i]._source));
            }
        }

        // As long as we didn't say to stop yet
        if (keepGoing)
        {
            yield return null;

            // Start this again
            StartCoroutine(PostJsonData());
        }
    }


    /// <summary>
    /// Author: Ben Hoffman
    /// Take the Json string that we just got from the website,
    /// and use the JsonUtility to make it JsonData. After that 
    /// it will send it to the game controller
    /// </summary>
    private IEnumerator StringToJson()
    {
        // Use the JsonUtility to send the string of data that I got from the server, to a data object
        yield return dataObject = JsonUtility.FromJson<Json_Data>(JSON_String);
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
            StopAllCoroutines();
            statusText.text = "Monitor Status: Stopped";
            statusText.CrossFadeColor(stoppedColor, 0.3f, true, true);
        }
        else
        {
            keepGoing = true;
            StopAllCoroutines();
            StartCoroutine(PostJsonData());
            statusText.text = "Monitor Status: Running";
            statusText.CrossFadeColor(runningColor, 0.3f, true, true);
        }
    }

}
