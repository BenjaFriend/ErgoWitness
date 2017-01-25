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
    private string url = "http://192.168.137.134:9200/packetbeat-2017.01.25/_search?pretty=true";

    // The output that I may get
    private string JSON_String = "";
    private Data dataObject;

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
            dataObject = JsonUtility.FromJson<Data>(JSON_String);
           // Debug.Log(dataObject.hits.hits[0].source._ip);

            for(int i = 0; i < dataObject.hits.hits.Length; i++)
            {
                Debug.Log(i.ToString() + " Source IP: " + dataObject.hits.hits[i]._source.ip);
                Debug.Log(i.ToString() + " Client IP: " + dataObject.hits.hits[i]._source.client_ip);
                Debug.Log(i.ToString() + " Port: " + dataObject.hits.hits[i]._source.port);
                Debug.Log(i.ToString() + " Method: " + dataObject.hits.hits[i]._source.method);
            }
        }
        else
        {
            // Print out the error that happened
            Debug.Log("WWW Error: " + www.error);
        }
    }

}
