using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;


public class MonitorObject : MonoBehaviour {

    #region Fields
    // Allow the picking of what beat this is (AKA what query to use)
    //public ListeningOptions.Beat whichBeat = ListeningOptions.Beat.Both;
    // Allow the picking of which JSON object to use for thi
    public JsonOptions.Option whatJson = JsonOptions.Option.Filebeat;

    [Range(0f, 1f)]
    public float frequency = 1f;

    public bool showDebug;
    private string serverIP;
    public bool keepGoing = false;            // If we want to keep going

    private string indexName;    // Either packetbeat or filebeat
    private string url;        // The filebeat index

    private Dictionary<string, string> headers;  // The dictionary with all the headers in it
    private string _Current_Query;

    // File locations of things
    public string fileLocation_queryTop;
    public string fileLocation_queryBottom;
    public string fileLocation_serverIP;
    public string fileLocation_latestTime;

    private string last_unique_id;

    private string _query_TOP;
    private string _query_BOTTOM;
    private string _latest_time;
    private bool _UseLastSuccess;
    private byte[] _PostData;        // The post data that we are using

    private WaitForSeconds _waitTime;

    #endregion

    // Use this for initialization
    void Start()
    {
        // Load in the query from the file locations
        _query_TOP = File.ReadAllText(Application.streamingAssetsPath + fileLocation_queryTop);
        _query_BOTTOM = File.ReadAllText(Application.streamingAssetsPath + fileLocation_queryBottom);

        // Set the latest time to the last one that we recorded
        _latest_time = File.ReadAllText(Application.streamingAssetsPath + fileLocation_latestTime);

        // Read in the server IP from the path that is specifed
        serverIP = File.ReadAllText(Application.streamingAssetsPath + fileLocation_serverIP);

        headers = new Dictionary<string, string>();

        last_unique_id = "";

        _waitTime = new WaitForSeconds(1 - frequency);

        SetUpURL();
    }

    private void SetUpURL()
    {
        switch (whatJson)
        {
            case (JsonOptions.Option.Packetbeat):
                indexName = "packetbeat";
                break;

            case (JsonOptions.Option.Filebeat):
                indexName = "filebeat";
                break;

            default:
                indexName = "filebeat";
                break;
                   
        }

        // Add the port and packet type
        url = "http://" + serverIP + ":9200/" + indexName + "-";

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
        url += dateUrl;
    }

    public void StartMonitor()
    { 
        Packetbeat_Json_Data packetdata;
        Json_Data filebeatdata;
        // Start whichever beat we want to list to, or both
        switch (whatJson)
        {
            case (JsonOptions.Option.Packetbeat):
                packetdata = new Packetbeat_Json_Data();
                StartCoroutine(MakePostRequest(packetdata));
                break;
            case (JsonOptions.Option.Filebeat):
                filebeatdata = new Json_Data();
                StartCoroutine(MakePostRequest(filebeatdata));
                break;
            default:

                // There is something wrong here
                // new Exception("The type of beat is not selected!");
                break;
        }
    }

    public void StopMonitor()
    {
        StopAllCoroutines();
    }

    private IEnumerator MakePostRequest<T>(T dataObject) 
    {
        if(dataObject == null)
        {
            yield break;
        }

        // Clear the headers:
        headers.Clear();
        // Add in content type:
        headers["Content-Type"] = "application/json";

        // Create a web request object
        WWW myRequest;

        if (!_UseLastSuccess)
        {
            // Build the query
            _Current_Query = _query_TOP + "\"" + _latest_time + _query_BOTTOM;
        }

        if(_Current_Query == null) yield  break;

        // Get the post data that I will be using, since it will always be the same
        _PostData = Encoding.GetEncoding("UTF-8").GetBytes(_Current_Query);

        myRequest = new WWW(url, _PostData, headers);

        // Set the priority to high and see what happens?
        myRequest.threadPriority = ThreadPriority.High;

        // Yield until the request is done
        yield return myRequest;

        // Check if we got an error in our request or not
        if (myRequest.error != null || myRequest.text == null)
        {
            Debug.Log("THERE WAS A REQUEST ERROR: " + myRequest.error);
            Debug.Log("The Query that failed: \n" + _Current_Query);
            if (myRequest.text != null)
                Debug.Log(myRequest.text);
            // If there was an error, then stop
            yield break;
        }

        // If we want to see debug info, then show it
        if (showDebug)
            Debug.Log(myRequest.text);


        // Create a new data object using the type of JSON that it is
        dataObject = JsonUtility.FromJson<T>(myRequest.text);

        if(typeof(T) == typeof(Packetbeat_Json_Data))
        {
            // Cast the object as necessary
            Packetbeat_Json_Data myPacketbeat = dataObject as Packetbeat_Json_Data;
            CheckData(myPacketbeat);
        }
        else if(typeof(T) == typeof(Json_Data))
        {
            // Cast the object as necessary
            Json_Data myFilebeat = dataObject as Json_Data;
            CheckData(myFilebeat);
        }


        // As long as we didn't say to stop yet
        if (keepGoing)
        {
            // Start this again after the frequency time
            yield return _waitTime;
            StartCoroutine(MakePostRequest(dataObject));
        }

    }

    private void CheckData(Packetbeat_Json_Data packetDataObj)
    {
        // ================= Check and make sure that our data is valid =====================
        // Make sure that our data is not null
        if (packetDataObj == null || packetDataObj.hits.hits == null || packetDataObj.hits.hits.Length == 0)
        {
            _UseLastSuccess = true;

            // Tell this to use the last successful query
            return;
        }

        // Make sure that this flow is not the same as the last one
        if (last_unique_id == packetDataObj.hits.hits[0]._id)
        {
            _UseLastSuccess = false;
            // If it is then break out and don't bother doing anything, this should
            // Save on processing power, and prevent duplicate
            return;
        }

        // Let this know that we no longer need to bank on the last success
        if (_UseLastSuccess)
        {
            _UseLastSuccess = false;
        }


        // ============= Keep track of stuff to prevent duplicates =======================
        // Keep track of our last successful query
        //_last_successful_Query = _Packetbeat_Current_Query;

        // It is new, so set the thing we use to check it to the current ID
        last_unique_id = packetDataObj.hits.hits[0]._id;

        // Set our latest packetbeat time to the most recent one
        _latest_time = packetDataObj.hits.hits[0]._source.runtime_timestamp + "\"";


        // ============== Actually loop through our hits data  =========================
        for (int i = 0; i < packetDataObj.hits.hits.Length; i++)
        {
            // Set the integer IP values of this object
            SetIntegerValues(packetDataObj.hits.hits[i]._source);

            // As long as what we got from those IP's is valid:
            if (packetDataObj.hits.hits[i]._source.destIpInt != 0 && packetDataObj.hits.hits[i]._source.sourceIpInt != 0)
            {
                // Send the data to the netflow controller
                NetflowController.currentNetflowController.CheckPacketbeatData(packetDataObj.hits.hits[i]._source);
            }
        }
    }

    private void CheckData(Json_Data dataObject)
    {
        // ================= Check and make sure that our data is valid =====================
        // Make sure that our data is not null
        if (dataObject == null || dataObject.hits.hits == null || dataObject.hits.hits.Length == 0)
        {
            _UseLastSuccess = true;

            // Tell this to use the last successful query
            return;
        }

        // Make sure that this flow is not the same as the last one
        if (last_unique_id == dataObject.hits.hits[0]._id)
        {
            // If it is then break out and don't bother doing anything, this should
            // Save on processing power, and prevent duplicate
            _UseLastSuccess = false;
            return;
        }

        // Let this know that we no longer need to bank on the last success
        if (_UseLastSuccess)
        {
            _UseLastSuccess = false;
        }

        // ============= Keep track of stuff to prevent duplicates ===============
        // It is new, so set the thing we use to check it to the current ID
        last_unique_id = dataObject.hits.hits[0]._id;

        // Set our latest packetbeat time to the most recent one
        _latest_time = dataObject.hits.hits[0]._source.runtime_timestamp + "\"";


        // Send the data to the game controller for all of our hits
        for (int i = 0; i < dataObject.hits.hits.Length; i++)
        {
            // Set the integer IP values if this source
            SetIntegerValues(dataObject.hits.hits[i]._source);

            // Send the bro data to the game controller, and add it to the network
            DeviceManager.currentDeviceManager.CheckIp(dataObject.hits.hits[i]._source);
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

        return BitConverter.ToInt32(System.Net.IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
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

    /// <summary>
    /// Write out the most recent query, and most recent timestamp at
    /// the end of the application
    /// </summary>
    void OnApplicationQuit()
    {
        // Write out the latest bro time
        System.IO.File.WriteAllText(Application.streamingAssetsPath + fileLocation_latestTime, _latest_time);
    }

}
