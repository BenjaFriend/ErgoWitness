using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// This object will send HTTP queries to the specified server,
/// and then set up that data to be sent to the device manager
/// class
/// </summary>
public class MonitorObject : MonoBehaviour {

    #region Fields
    // Allow the picking of which JSON object to use for thi
    public JsonOptions.Option whatJson = JsonOptions.Option.Filebeat;

    [Range(0f, 1f)]
    public float frequency = 1f;    // How often we want to make a request, 1 is the highest(most frequent)

    public bool showDebug;  // If true then debug.log things
    private string serverIP;    // The IP address of the server running our database
    public bool logData = false; // Do we want to query the data

    private string indexName;    // Either packetbeat or filebeat
    private string url;        // The filebeat index

    private Dictionary<string, string> headers;  // The dictionary with all the headers in it
    private string _Current_Query;

    // File locations of things
    public string fileLocation_queryTop;
    public string fileLocation_queryBottom;
    public string fileLocation_serverIP;
    public string fileLocation_latestTime;
    public string filelocation_LogFile;
    private string filelocation_ErrorLog = "/ErrorLog.txt";

    private string last_unique_id;

    // The query data that we will use to build our request
    private string _query_TOP;
    private string _query_BOTTOM;
    // The latest successful time stamp that we used to make a request
    private string _latest_time;
    // This is true if we want to use our last success, which means that the newest query failed
    private bool _UseLastSuccess;

    private byte[] _PostData;        // The post data that we are using

    private WaitForSeconds _waitTime;   // A wait object so that we only need to create it once
    
    private IEnumerator request_Coroutine;  // The coroutine that is running the web request
    #endregion

    /// <summary>
    /// Read in all of the query information, the server IP, and initalize the headers
    /// for use in the query. Set up the URL as necessary. Create a wait time object
    /// so that we only need to create it once
    /// </summary>
    void Start()
    {
        filelocation_LogFile = Application.streamingAssetsPath + filelocation_LogFile;
        filelocation_ErrorLog = Application.streamingAssetsPath + filelocation_ErrorLog;

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

    /// <summary>
    /// Set up the string that will be our URL based
    /// on whether we are to use Filebeat or 
    /// packetbeat, and the current date
    /// </summary>
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
        string dateUrl = System.DateTime.Today.Year.ToString() + ".";

        // Make sure we have proper format on the month
        if (System.DateTime.Today.Month < 10)
        {
            dateUrl += "0" + System.DateTime.Today.Month.ToString() + ".";
        }
        else
        {
            dateUrl += System.DateTime.Today.Month.ToString() + ".";
        }
        // Handle the day
        if (System.DateTime.Today.Day < 10)
        {
            dateUrl += "0" + System.DateTime.Today.Day.ToString() + "/_search?pretty=true";
        }
        else
        {
            dateUrl += System.DateTime.Today.Day.ToString() + "/_search?pretty=true";
        }

        // Add the date to each of the URL's
        url += dateUrl;
    }

    /// <summary>
    /// Start the HTTP request coroutine, and store the instane in the 
    /// request_Courinte field
    /// </summary>
    public void StartMonitor()
    {
        // If we are already running this corountine, then stop it first
        if (request_Coroutine != null)
        {
            StopMonitor();
        }              

        Packetbeat_Json_Data packetdata;
        Json_Data filebeatdata;
        // Start whichever beat we want to list to, or both
        switch (whatJson)
        {
            case (JsonOptions.Option.Packetbeat):
                // Initalize the packetbeat request
                packetdata = new Packetbeat_Json_Data();
                // Store the coroutine, so that we can stop it specifically
                request_Coroutine = MakePostRequest(packetdata);
                // Start to request the data
                StartCoroutine(request_Coroutine);
                break;
            case (JsonOptions.Option.Filebeat):
                filebeatdata = new Json_Data();

                // Keep track of the co routine, so that we can stop it specifically
                request_Coroutine = MakePostRequest(filebeatdata);

                StartCoroutine(request_Coroutine);
                break;
            default:
                // There is something wrong here
                // new Exception("The type of beat is not selected!");
                break;
        }
    }

    /// <summary>
    /// Stop the request coroutine
    /// </summary>
    public void StopMonitor()
    {
        // Stop the store coroutine as long as it exists
        if(request_Coroutine != null)
        {
            StopCoroutine(request_Coroutine);
        }
        // Otherwise stop everything as a failsafe
        else
        {
            StopAllCoroutines();
        }
    }

    /// <summary>
    /// Take in what JSON class to use, and create a WWW request based off of
    /// the queries that we were given. At the end, wait (1 - frequency) and then
    /// start the method again recursively
    /// </summary>
    /// <typeparam name="T">What serialized JSON class we want to use</typeparam>
    /// <param name="dataObject">The JSON data object</param>
    /// <returns>A finished WWW request to the given server</returns>
    private IEnumerator MakePostRequest<T>(T dataObject) 
    {
        // Make sure that this data object is no null
        if(dataObject == null)
        {
            // Break out of the coroutine
            yield break;
        }

        // Clear the headers:
        headers.Clear();

        // Add in content type:
        headers["Content-Type"] = "application/json";

        // Create a web request object
        WWW myRequest;

        // If we do not want to use the alst successful query...
        if (!_UseLastSuccess)
        {
            // Build the query
            _Current_Query = _query_TOP + "\"" + _latest_time + _query_BOTTOM;
        }
        // If our query is empty then break out because it will fail
        if(_Current_Query == null) yield  break;

        // Get the post data that I will be using, since it will always be the same
        _PostData = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(_Current_Query);

        // Initalize the WWW request to have our query and proper URL/headers
        myRequest = new WWW(url, _PostData, headers);

        // Set the priority to high and see what happens?
        myRequest.threadPriority = ThreadPriority.High;

        // Yield until the request is done
        yield return myRequest;

        // Check if we got an error in our request or not
        if (myRequest.error != null || myRequest.text == null)
        {
            // Log all the error data if there was one
            LogData("THERE WAS A REQUEST ERROR: " + myRequest.error, filelocation_ErrorLog);
            LogData("The Query that failed: \n" + _Current_Query, filelocation_ErrorLog);
            if (myRequest.text != null)
                LogData("The HTTP request text:\n" + myRequest.text, filelocation_ErrorLog);
            // If there was an error, then stop
            yield break;
        }

        // If we want to see debug info, then show it
        if (showDebug)
            Debug.Log(myRequest.text);

        // If we want to log this data, then send this string to the log writer
        if (logData)
        {
            LogData(myRequest.text, filelocation_LogFile);
        }

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

        // Start this again after the frequency time
        yield return _waitTime;
        request_Coroutine = MakePostRequest(dataObject);
        StartCoroutine(request_Coroutine);
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

        return System.BitConverter.ToInt32(System.Net.IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
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
    /// Write the given string to the specifed log file field
    /// </summary>
    /// <param name="log">What we want to write out</param>
    private void LogData(string log, string filelocation)
    {
        using (StreamWriter writer = new StreamWriter(filelocation))
        {
            writer.Write("\n" + log);
        }
    }

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
