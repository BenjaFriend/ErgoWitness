using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Net;
using System.Text;
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
    // The URL of my ELK server
    private string elk_url = "http://192.168.137.134:9200/packetbeat-2017.01.26/_search?pretty=true";
    private GameController gameControllerObj; // The game controller 
    private string JSON_String = "";        // The string that represents the JSON data
    private Data dataObject;                // The actual JSON data class 
    private WWW www;                        // This class has methods that are built in to make HTTP requests

    private string queryLocation;
    private string queryString;



    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose: Just start the coroutine that will constantly find the data
    /// </summary>
    void Start()
    {
        queryLocation =  Application.streamingAssetsPath + "/gimmeData.json";
        queryString = File.ReadAllText(queryLocation);
        Debug.Log(queryString);
        gameControllerObj = GameObject.FindObjectOfType<GameController>();

        // Find the latest index name and make my URL
        
        StartCoroutine(GetJSONText());
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To send out a WWW HTTP request to my ELK stack 
    /// server, and put that data into a DATA class object, so that I can access
    /// all of my stuff as I regualarlly would with any C# variable
    /// </summary>
    IEnumerator GetJSONText()
    {
        // Make a WWW object and give it the URL to my ELK stack server
        www = new WWW(elk_url + queryString);
        yield return www;   // Wait for the website to give me a response

        if (www.error == null)
        {
            // As long as there was not an error, then we can set the JSON string to the output of the HTTP request
            JSON_String = www.text;

            // Use the JsonUtility to send the string of data that I got from the server, to a data object
            dataObject = JsonUtility.FromJson<Data>(JSON_String);

            // As long as we did not time out... 
            if (!dataObject.timed_out)
            {
                // Give my game controller the JSON data to sort out if there is a new computer on it or not
                gameControllerObj.CheckIP(dataObject);
            }
        }
        else
        {
            // Print out the error that happened
            Debug.Log("WWW Error: " + www.error);
        }

        // Run it again, forever and ever
        StartCoroutine(GetJSONText());
    }

   /* public void DifferentWay()
    {
        string messaggio;
        messaggio = "Caspita non ci posso credere!!!";

        System.Net.WebRequest request = WebRequest.Create(elk_url);

        request.ContentType = "application/json";
        request.Method = "POST";
        //string authInfo = "usr:pwd";
        //request.Headers["X-Parse-Application-Id"] = "aaaaaaaaaaaaaaa";
        //request.Headers["X-Parse-REST-API-Key"] = "bbbbbbbbbbbbbbb";
        byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes("{\"channels\": [\"\"], \"data\": { \"alert\": \" " + messaggio + "\" } }");
        string result = System.Convert.ToBase64String(buffer);
        Stream reqstr = request.GetRequestStream();
        reqstr.Write(buffer, 0, buffer.Length);
        reqstr.Close();

        WebResponse response = request.GetResponse();
    }*/


}
