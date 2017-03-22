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

    private enum State
    {
        CurrentlyRunning,
        FinishedTheRequestAndParse,
        Stop
    }

    #region Fields
    private State currentState;
    // Have this bool so that the user can use a toggle box in the menu
    private bool keepGoing = true;

    [Range(0f, 1f)]
    public float frequency = 1f;    // How often we want to make a request, 1 is the highest(most frequent)

    private string serverIP;    // The IP address of the server running our database
    public bool logData = false; // Do we want to query the data
    public string indexName;    // Either packetbeat or filebeat
    private string url;        // The filebeat index

    private Dictionary<string, string> headers;  // The dictionary with all the headers in it
    private string _Current_Query;

    // File locations of things
    public string fileLocation_queryTop;
    public string fileLocation_queryBottom;
    public string fileLocation_serverIP;
    public string fileLocation_latestTime;
    public string filelocation_LogFile;

    private string filelocation_ErrorLog = "/ErrorLog.txt";  // The location of the error log 

    // The query data that we will use to build our request
    private string _query_TOP;
    private string _query_BOTTOM;
    // The latest successful time stamp that we used to make a request
    public string _latest_time;
    // This is true if we want to use our last success, which means that the newest query failed
    public bool _UseLastSuccess;

    private byte[] _PostData;        // The post data that we are using

    private WaitForSeconds _waitTime;   // A wait object so that we only need to create it once
    
    private IEnumerator request_Coroutine;  // The coroutine that is running the web request

    private bool _UseRealTime; // If this is true, then the user will be picking a specific range of times that they want to see
    private string _lessThenQuery;
    private string _greaterThenQuery;

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
        using (StreamReader reader = new StreamReader(Application.streamingAssetsPath + fileLocation_serverIP))
        {
            serverIP = reader.ReadLine() ?? "";
        }

        headers = new Dictionary<string, string>();

        _waitTime = new WaitForSeconds(1 - frequency);

        _UseRealTime = true;

        SetUpURL(indexName);

        // Set the current state
        currentState = State.FinishedTheRequestAndParse;
    }

    /// <summary>
    /// Set up the string that will be our URL based
    /// on whether we are to use Filebeat or 
    /// packetbeat, and the current date
    /// </summary>
    public virtual void SetUpURL(string indexName)
    {
        // Add the port and packet type
        url = serverIP  + indexName + "-";

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
    public virtual void StartMonitor()
    {
        keepGoing = true;

        currentState = State.FinishedTheRequestAndParse;
    }

    /// <summary>
    /// Stop the request coroutine
    /// </summary>
    public void StopMonitor()
    {
        // Set the current state to stop
        currentState = State.Stop;
        keepGoing = false;
    }

    /// <summary>
    /// This is so that we can toggle the monitoring with a UI checkbox
    /// </summary>
    public void ToggleMoniotr()
    {
        // If we are running, then call the stop method...
        if (keepGoing || currentState != State.Stop)
        {
            // Stop teh monitor
            StopMonitor();
        }
        else
        {
           // Start the monitor
            StartMonitor();
        }

    }

    /// <summary>
    /// This method will manage the web request coroutine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataObject"></param>
    /// <returns></returns>
    public IEnumerator FSM<T>(T dataObject)
    {
        // We always want to be looping and check what state we are in
        while(keepGoing)
        {
            if(currentState == State.Stop)
            {
                StopCoroutine(request_Coroutine);
                keepGoing = false;
                break;
            }

            switch (currentState)
            {
                case (State.Stop):
                    // If we have told it to stop, then break out of this coroutine
                    //Debug.Log("Stop!!!!!   SWITCH STATEMENT");
                    StopCoroutine(request_Coroutine);
                    keepGoing = false;
                    break;
                case (State.FinishedTheRequestAndParse):
                        // If we finished, then start a new one 
                        // Make a request and start it
                        yield return _waitTime;
                        // Create a new request with teh given data type
                        request_Coroutine = MakePostRequest(dataObject);
                        // Actually start that coroutine
                        StartCoroutine(request_Coroutine);
                        // Make sure we know that the state is currently running
                        currentState = State.CurrentlyRunning;
                    break;

                default:
                    // If we are currently running, or something else is happening
                    yield return null;
                    break;
            }
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

        // Clear the headers, otherwise we will get the same data from the last one:
        headers.Clear();

        // Add in content type:
        headers["Content-Type"] = "application/json";

        // Create a web request object
        WWW myRequest;

        // If we do not want to use the alst successful query...
        if (!_UseLastSuccess && _UseRealTime)
        {
            // Build the query
            _Current_Query = _query_TOP + "\"gt\":" + "\"" + _latest_time + "\"" + _query_BOTTOM;
        }
        else if (!_UseLastSuccess && !_UseRealTime)
        {
            // Use the query that is build on the timestamps provided by the user
            _Current_Query = _query_TOP  + _greaterThenQuery + "\",\n"  + _lessThenQuery + "\""  + _query_BOTTOM;
        }

        // If our query is empty then break out because it will fail
        if (_Current_Query == null) yield  break;

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
            // Log all the error data if there was an error
            LogData("THERE WAS A REQUEST ERROR: " + myRequest.error, filelocation_ErrorLog);
            LogData("The Query that failed: \n" + _Current_Query, filelocation_ErrorLog);
            if (myRequest.text != null)
            {
                LogData("The HTTP request text:\n" + myRequest.text, filelocation_ErrorLog);
                // If we are in the editor then send this info the console
                #if UNITY_EDITOR
                Debug.Log("The HTTP request text:\n" + myRequest.text);
                Debug.Log("The query was: " + _Current_Query);
                #endif
            }
            // If there was an error, then stop
            yield break;
        }

        // If we want to log this data, then send this string to the log writer
        if (logData)
        {
            // Send the resulting query to the log file
            LogData(myRequest.text, filelocation_LogFile);
        }

        // Create a new data object using the type of JSON that it is
        dataObject = JsonUtility.FromJson<T>(myRequest.text);

        // Call this method so that the more specifc child class can deal with it
        CheckRequestData(dataObject);

        // Mark the state as finished, as long as we were not told to stop
        if(currentState != State.Stop)
            currentState = State.FinishedTheRequestAndParse;
    }

    /// <summary>
    /// This is a class that will be overridden by the child class
    /// depending on what type of JSON data we want to look at
    /// </summary>
    /// <typeparam name="T">The JSON data type</typeparam>
    /// <param name="data">The actual data object from the query</param>
    public virtual void CheckRequestData<T>(T data) { }

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
            writer.Write("\n" + log + "\n");
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

    /// <summary>
    /// This method will append a filter query to 
    /// </summary>
    public virtual void AddFilter()
    {
        // Add in the filter

        // Do this by appending a string to the query
    }


    /// <summary>
    /// This will tell this object to use a differernt query,
    /// so that the use does a query between two timestamps
    /// that they add in 
    /// </summary>
    public void QueryBetweenTimes(string minTimestamp, string maxTimestamp)
    {
        // Set the less then query to the max timestamp
        _lessThenQuery = "\"lt\":\"" + maxTimestamp;
        // Set the greater then query to the min timestamp
        _greaterThenQuery = "\"gt\": \"" + minTimestamp;

        // Make sure that we are not using realtime
        _UseRealTime = false;
    }

    /// <summary>
    /// Tells us to use real time timestamps
    /// </summary>
    public void UseRealTime()
    {
        _UseRealTime = true;
        // Restart the monitor
        StopMonitor();
        StartMonitor();
    }

}
