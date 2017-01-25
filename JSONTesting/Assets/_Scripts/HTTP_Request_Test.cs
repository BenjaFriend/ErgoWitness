using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Author: Ben Hoffman
/// Purpose of class: To see if I can't do a HTTP request up in here
/// </summary>
public class HTTP_Request_Test : MonoBehaviour {

    #region Fields
    // The URL of my ELK server
    private string url = "http://192.168.137.134:9200/packetbeat-2017.01.25/_search?pretty=true";

    // The output that I may get
    private string JSON_String = "";
    private Data dataObject;
    private WWW www;
    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose: Just start the coroutine that will constantly find the data
    /// </summary>
    void Start ()
    {
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
        www = new WWW(url);
        yield return www;

        if(www.error == null)
        {
            // As long as there was not an error, then we can set the JSON string to the output of the HTTP request
            JSON_String = www.text;
            
            // Use the JsonUtility to send the string of data that I got from the server, to a data object
            dataObject = JsonUtility.FromJson<Data>(JSON_String);

            // As long as we did not time out... 
            if (!dataObject.timed_out)
            {
                // Check the Json data to see if we need to do anything with it
                CheckData(dataObject);
            }
        }
        else
        {
            // Print out the error that happened
            Debug.Log("WWW Error: " + www.error);
        }

        // Run it again, forever and ever
        //StartCoroutine(GetJSONText());
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: Check the JSON data to see if we nee to make a new
    /// 'computer' object
    /// </summary>
    /// <param name="jsonData">The current JSON data</param>
    private void CheckData(Data jsonData)
    {
        Debug.Log(" Source IP: " + jsonData.hits.hits[0]._source.ip);

        // Loop through the information that I got
       /* for (int i = 0; i < jsonData.hits.hits.Length; i++)
        {
            // If there is an IP address here that is new, THEN make a new Computer object with that data
            // If there is an IP address that I already had, THEN add a connection to the list's of connections
            Debug.Log(i.ToString() + " Source IP: " + jsonData.hits.hits[i]._source.ip);
            Debug.Log(i.ToString() + " Client IP: " + jsonData.hits.hits[i]._source.client_ip);
            Debug.Log(i.ToString() + " Port: " + jsonData.hits.hits[i]._source.port);
            Debug.Log(i.ToString() + " Method: " + jsonData.hits.hits[i]._source.method);
        }*/
    }

}
