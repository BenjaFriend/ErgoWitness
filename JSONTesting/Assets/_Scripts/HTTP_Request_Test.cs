using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Author: Ben Hoffman
/// Purpose of class: To see if I can't do a HTTP request up in here
/// </summary>
public class HTTP_Request_Test : MonoBehaviour {

    // The URL of my ELK server
    private string url = "http://192.168.137.134:9200/packetbeat-2017.01.24/_search?pretty=true";

    // The output that I may get
    private string JSON_String = "";

	void Start ()
    {
        StartCoroutine(GetJSONText());     
	}

    IEnumerator GetJSONText()
    {
        // Make a WWW object and give it the URL to my ELK stack server
        WWW www = new WWW(url);
        yield return www;

        if(www.error == null)
        {
            // it worked, so set my output to a string. 
            JSON_String = www.text;

        }
        else
        {
            // Print out the error that happened
            Debug.Log("WWW Error: " + www.error);
        }
    }

}
