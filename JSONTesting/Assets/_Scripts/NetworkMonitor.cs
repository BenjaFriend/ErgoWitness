using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;

public struct ListeningOptions
{
    public enum Beat { Packetbeat, Filebeat , Both};
}


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
    public ListeningOptions.Beat whichBeat = ListeningOptions.Beat.Both;

    [Range(0f,1f)]
    public float frequency = 1f;
    #region Fields
    public static NetworkMonitor currentNetworkMonitor;

    public string serverIP;
    public bool keepGoing = false;            // If we want to keep going

    // The URL of my ELK server
    private string elk_url_filebeat;        // The filebeat index
    private string elk_url_packetbeat;      // The packetbeat index
    private Json_Data dataObject;           // The actual JSON data class 
    private Packetbeat_Json_Data packetDataObj;

    // The data of the post request
    private byte[] broPostData;        // The post data that we are using
    private byte[] packetPostData;        // The post data that we are using

    private Dictionary<string, string> headers_packetbeat;  // The dictionary with all the headers in it
    private Dictionary<string, string> headers;  // The dictionary with all the headers in it

    private string broHeaderString;
    private string _Packetbeat_Current_Query;

    private string lastFlowRecived;
    private string lastFilebeatRecieved;

    private string _packetbeat_TOP;
    private string _packetbeat_BOTTOM;
    private string _latest_packetbeat_time;
    private string _last_successful_Query;
    private bool _packetbeat_UseLastSuccess;


    private string _bro_TOP;
    private string _bro_BOTTOM;
    private string _latest_bro_time;

    private WaitForSeconds waitTime;

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
        headers_packetbeat = new Dictionary<string, string>();

        // Initalize the strings to not get a null ref exception
        lastFlowRecived = "";
        lastFilebeatRecieved = "";

        // Read in the top and bottom files
        _packetbeat_TOP = File.ReadAllText(Application.streamingAssetsPath + "/Packetbeat/packetbeat_Headers_TOP.json");
        _packetbeat_BOTTOM = File.ReadAllText(Application.streamingAssetsPath + "/Packetbeat/packetbeat_Headers_BOTTOM.json");
        _latest_packetbeat_time = File.ReadAllText(Application.streamingAssetsPath + "/Packetbeat/latest_packetbeat_time.txt");

        // Set up the bro files how I need to
        _bro_TOP = File.ReadAllText(Application.streamingAssetsPath + "/Bro/bro_Headers_TOP.json");
        _bro_BOTTOM = File.ReadAllText(Application.streamingAssetsPath + "/Bro/bro_Headers_BOTTOM.json");
        _latest_bro_time = File.ReadAllText(Application.streamingAssetsPath + "/Bro/latest_bro_time.txt");

        waitTime = new WaitForSeconds(frequency);
    }

    /// <summary>
    /// Called on the setting of the IP address
    /// </summary>
    private void StartMonitoring()
    {
        // Set up my URL to get info from
        SetUpURL();

        // Start whichever beat we want to list to, or both
        switch (whichBeat)
        {
            case (ListeningOptions.Beat.Packetbeat):
                StartCoroutine(PostJsonData(elk_url_packetbeat, true));
                break;
            case (ListeningOptions.Beat.Filebeat):
                StartCoroutine(PostJsonData(elk_url_filebeat, false));
                break;
            case (ListeningOptions.Beat.Both):
                StartCoroutine(PostJsonData(elk_url_packetbeat, true));
                StartCoroutine(PostJsonData(elk_url_filebeat, false));
                break;
            default:
                // There is something wrong here
                // new Exception("The type of beat is not selected!");
                break;
        }
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
        if (isFlowData)
        {
            // Clear the headers:
            headers_packetbeat.Clear();

            // Add in content type:
            headers_packetbeat["Content-Type"] = "application/json";
        }
        else
        {
            // Clear the headers:
            headers.Clear();

            // Add in content type:
            headers["Content-Type"] = "application/json";
        }


        // Create a web request object
        WWW myRequest;

        if (!isFlowData)
        {
            // Set up the query with the latest bro time
            broHeaderString = _bro_TOP + "\"" + _latest_bro_time + _bro_BOTTOM;

            // Get the post data that I will be using, since it will always be the same
            broPostData = Encoding.GetEncoding("UTF-8").GetBytes(broHeaderString);

            // Start up the reqest for FILEBEAT:
            myRequest = new WWW(url, broPostData, headers);
        }
        else
        {
            // If the last query failed, then fall back to our last successful one
            if (_packetbeat_UseLastSuccess && _last_successful_Query != null)
            {
                _Packetbeat_Current_Query = _last_successful_Query;
            }
            else
            {
                // Set up the query string with teh latest packetbeat string
                _Packetbeat_Current_Query = _packetbeat_TOP + "\"" + _latest_packetbeat_time + _packetbeat_BOTTOM;
            }

            // Get the post data for packetbeat
            packetPostData = Encoding.GetEncoding("UTF-8").GetBytes(_Packetbeat_Current_Query);

            // Start up the reqest for PACKETBEAT:
            myRequest = new WWW(url, packetPostData, headers_packetbeat);
        }

        // Yield until it's done:
        yield return myRequest;

        // Check if we got an error in our request or not
        if (myRequest.error != null || myRequest.text == null)
        {
            //throw new Exception("There was a request error!");
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

            // Start checking packetbeat
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
            yield return waitTime;
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
        // ================= Check and make sure that our data is valid =====================
        // Make sure that our data is not null
        if (packetDataObj == null || packetDataObj.hits.hits == null || packetDataObj.hits.hits.Length == 0)
        {
            _packetbeat_UseLastSuccess = true;

            // Tell this to use the last successful query
            return;
        }

        // Make sure that this flow is not the same as the last one
        if (lastFlowRecived == packetDataObj.hits.hits[0]._id)
        {
            // If it is then break out and don't bother doing anything, this should
            // Save on processing power, and prevent duplicate
            return;
        }

        // Let this know that we no longer need to bank on the last success
        if (_packetbeat_UseLastSuccess)
        {
            _packetbeat_UseLastSuccess = false;
        }


        // ============= Keep track of stuff to prevent duplicates ================
        // Keep track of our last successful query
        _last_successful_Query = _Packetbeat_Current_Query;

        // It is new, so set the thing we use to check it to the current ID
        lastFlowRecived = packetDataObj.hits.hits[0]._id;

        // Set our latest packetbeat time to the most recent one
        _latest_packetbeat_time = packetDataObj.hits.hits[0]._source.start_time + "\"";


        // ============== Actually loop through our hits data  =========================
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
    }

    /// <summary>
    /// Check if the data that was given is the same as the previous,
    /// if it is then break out of this method. Otherwise, send that
    /// data to the game controller and add the computer to the network
    /// </summary>
    private void CheckFilebeat()
    {
        // Make sure that our data is not null, if it is then return
        if (dataObject == null || dataObject.hits.hits == null || dataObject.hits.hits.Length == 0)
        {
            return;
        }
        
        // If this data is the same as the last one that we did, 
        // then we don't need to look at any of the other stuff
        if (lastFilebeatRecieved == dataObject.hits.hits[0]._id)
        {
            // If it is then break out and don't bother doing anything, this should
            // Save on processing power, and prevent duplicated
            return;
        }

        // This is a new data object, so keep track of the most recent ID
        lastFilebeatRecieved = dataObject.hits.hits[0]._id;

        _latest_bro_time = dataObject.hits.hits[0]._source.timestamp + "\"";

        // Send the data to the game controller for all of our hits
        for (int i = 0; i < dataObject.hits.hits.Length; i++)
        {
            // Set the integer IP values if this source
            SetIntegerValues(dataObject.hits.hits[i]._source);

            // Send the bro data to the game controller, and add it to the network
            GameController.currentGameController.CheckIp(dataObject.hits.hits[i]._source);
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

        return BitConverter.ToInt32(IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
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
