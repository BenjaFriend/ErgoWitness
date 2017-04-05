using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// This object will send HTTP queries to the specified server,
/// and then set up that data to be sent to the device manager
/// class.
/// </summary>
public class MonitorObject : MonoBehaviour {

    #region Fields
    private MonitorState currentState;    // The current state of this web request

    [Range(0f, 1f)]
    public float frequency = 1f;    // How often we want to make a request, 1 is the highest(most frequent)

    private string serverIP;    // The IP address of the server running our database
    public string indexName;    // Either packetbeat or filebeat
    private string url;         // The filebeat index

    private Dictionary<string, string> headers;  // The dictionary with all the headers in it
    private string _current_Query;               // The current query for us to use

    // File locations of the queries that we need to use
    public string fileLocation_queryTop;        // Location of the top part of the query
    public string fileLocation_queryBottom;     // Location of the bottom part fl the query, with what fields we want
    public string fileLocation_serverIP;        // The location of the serverIP file
    public string fileLocation_latestTime;      // Location of the latest time file
    public string filelocation_LogFile;         // Location of the error log file we want to use if we want to record the data we get


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

    /// <summary>
    /// If _UseRealTime is true, then we will use the current timestamp of the system 
    /// to make a query. If it is false, then we will use the data from the custom
    /// user timestamp to make a query
    /// </summary>
    private bool _UseRealTime;      

    // Strings that will be used for custom queries if we need them
    private string _lessThenQuery;
    private string _greaterThenQuery;

    /// <summary>
    /// The current state of this request
    /// </summary>
    public MonitorState CurrentState { get { return currentState; } }
    /// <summary>
    /// The top query so that child classes can view it if they need to, for custom quereis
    /// </summary>
    public string Query_Top { get { return _query_TOP; } }
    /// <summary>
    /// The bottom query so that child classes can view it if they need to, for custom queries
    /// </summary>
    public string Query_Bottom { get { return _query_BOTTOM; } }
    #endregion


    #region Methods
    /// <summary>
    /// Read in all of the query information, the server IP, and initalize the headers
    /// for use in the query. Set up the URL as necessary. Create a wait time object
    /// so that we only need to create it once
    /// </summary>
    void Start()
    {
        // Set the location of the log files to the streaming assets
        filelocation_LogFile = Application.streamingAssetsPath + filelocation_LogFile;
        filelocation_ErrorLog = Application.streamingAssetsPath + filelocation_ErrorLog;

        // Load in the query from the file locations
        _query_TOP = File.ReadAllText(Application.streamingAssetsPath + fileLocation_queryTop);
        _query_BOTTOM = File.ReadAllText(Application.streamingAssetsPath + fileLocation_queryBottom);

        // Set the latest time to the last one that we recorded
        _latest_time = File.ReadAllText(Application.streamingAssetsPath + fileLocation_latestTime);

        // Read in the server IP from the path that is specifed
        serverIP = File.ReadAllText(Application.streamingAssetsPath + fileLocation_serverIP);

        // Read in the server IP
        using (StreamReader reader = new StreamReader(Application.streamingAssetsPath + fileLocation_serverIP))
        {
            // Read in the server IP as long as it is not null or empty
            serverIP = reader.ReadLine() ?? "";
        }

        // Create new headers
        headers = new Dictionary<string, string>();

        // Create a wait time variable, so that we don't have to create new one every request
        _waitTime = new WaitForSeconds(1 - frequency);

        // Set real time to true in the beginning
        _UseRealTime = true;

        // Set up the Url of this object with it's specified index
        url = SetUpURL(indexName);

        // Set the current state to stopped
        currentState = MonitorState.Stop;
    }

    /// <summary>
    /// Set up the string that will be our URL based
    /// on whether we are to use Filebeat or 
    /// packetbeat, and the current date
    /// </summary>
    private string SetUpURL(string indexName)
    {
        // Add the port and packet type
        string newURL = serverIP  + indexName + "-";

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
        newURL += dateUrl;
        // Return the URL for us to use
        return newURL;
    }

    /// <summary>
    /// Set the current state of the monitor so that the FSM will create a new 
    /// request
    /// </summary>
    public virtual void StartMonitor()
    {
        // Set the current state to restart
        currentState = MonitorState.Finished;
    }

    /// <summary>
    /// Stop the request coroutine if there is one, and set the current state of the FSM to stop
    /// </summary>
    public void StopMonitor()
    {
        // Set the current state to stop, so the FSM will stop
        currentState = MonitorState.Stop;
    }

    #endregion


    #region Coroutines

    /// <summary>
    /// While the current state is not to stop, either wait for the current request to finish, 
    /// </summary>
    /// <typeparam name="T">The type of JSON data we want to use</typeparam>
    /// <param name="dataObject">The json data object</param>
    /// <returns>Will return null until the request is finished, in which case it makes a new request</returns>
    public IEnumerator FSM<T>(T dataObject)
    {
        // While we have not told this method to stop...
        while (currentState != MonitorState.Stop)
        {
            // If the current request is finished...
            if(currentState == MonitorState.Finished)
            {
                // Create a new request with teh given data type
                request_Coroutine = MakePostRequest(dataObject);

                // Actually start that coroutine
                StartCoroutine(request_Coroutine);
            }

            // Return null because we are in a loop
            yield return null;
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
        // Set the state to currently running so that we know we are running
        currentState = MonitorState.CurrentlyRunning;

        // Clear the headers, otherwise we will get the same data from the last one:
        headers.Clear();

        // Add in content type to JSON to the HTML headers:
        headers["Content-Type"] = "application/json";

        // Create a web request
        WWW myRequest;

        // Build the query again if the alst one was a success
        if (!_UseLastSuccess)
        {
            _current_Query = BuildQuery();
        }

        // Get the post data that I will be using, since it will always be the same
        _PostData = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(_current_Query);

        // Initalize the WWW request to have our query and proper URL/headers
        myRequest = new WWW(url, _PostData, headers);

        // Set the priority to high, so that we get the info as soon as possible
        //myRequest.threadPriority = ThreadPriority.High;

        // Yield until the request is done
        yield return myRequest;

        // If there was an error...
        if (myRequest.error != null)
        {
            // Log all the error data if there was an error
            LogData("THERE WAS A REQUEST ERROR: " + myRequest.error, filelocation_ErrorLog);
            LogData("The Query that failed: \n" + _current_Query, filelocation_ErrorLog);

            // If we are in the editor, then print the error to the console
            #if UNITY_EDITOR
                Debug.Log("The HTTP request text:\n" + myRequest.text);
                Debug.Log("The query was: " + _current_Query);
            #endif
            
            // If there was an error, then stop
            yield break;
        }

        // Create a new data object using the type of JSON that it is
        dataObject = JsonUtility.FromJson<T>(myRequest.text);

        // Call this method so that the more specifc child class can deal with it
        CheckRequestData(dataObject);

        // Wait however long we want to, so that we don't make a crazy amount of requests
        yield return _waitTime;

        // Change the current state to finished, so that the FSM knows to create another request
        currentState = MonitorState.Finished;
    }

    /// <summary>
    /// This will build up a query by default using the data that was read
    /// in by the specified query files.
    /// </summary>
    /// <param name="optionalData">In case we have some kind of data that changes at runtime that we want to query</param>
    /// <returns></returns>
    public virtual string BuildQuery()
    {
        string query = "";

        // If we are using real time and the previous query was not a failure...
        if ( _UseRealTime)
        {
            // Build the query using the latest timestamp
            query = _query_TOP + "\"gt\":" + "\"" + _latest_time + "\"" + _query_BOTTOM;
        }
        // If we want to use a custom query and we had a previous success...
        else if (!_UseRealTime)
        {
            // Use the query that is build on the timestamps provided by the user
            query = _query_TOP + _greaterThenQuery + "\",\n" + _lessThenQuery + "\"" + _query_BOTTOM;
        }

        return query;
    }


    /// <summary>
    /// This is a class that will be overridden by the child class
    /// depending on what type of JSON data we want to look at
    /// </summary>
    /// <typeparam name="T">The JSON data type</typeparam>
    /// <param name="data">The actual data object from the query</param>
    public virtual void CheckRequestData<T>(T data) { }

    #endregion


    #region Custom queries

    /// <summary>
    /// Set up the greater then and less then timestamp queries to use.
    /// This will be called when the user clicks the 'use custom timestamp'
    /// button. 
    /// </summary>
    /// <param name="minTimestamp">The minimum timestamp to use. Farther back in time</param>
    /// <param name="maxTimestamp">The maximim timestamp to use. More recent time</param>
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
    /// Tells us to use real time timestamps and restarts this monitor.
    /// This will be called when the user presses the button to use real
    /// time.
    /// </summary>
    public void UseRealTime()
    {
        // We want to use real time now
        _UseRealTime = true;

        // Restart the monitor
        StopMonitor();
        StartMonitor();
    }

    #endregion


    #region System utilities

    /// <summary>
    /// Use Bit conversion to send the IP address to an integer
    /// </summary>
    /// <param name="ipAddr">The string IP address that we want the integer version of</param>
    /// <returns>A bit conversion of the IP address string in integer format</returns>
    public int IpToInt(string ipAddr)
    {
        // If this IP is null, then return 0
        if (ipAddr == null) return 0;
        // Otherwise, return a bit conversion of the IP address as an integer value
        return System.BitConverter.ToInt32(System.Net.IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
    }

    /// <summary>
    /// Write the given string to the specifed log file field
    /// </summary>
    /// <param name="log">The information we want to log</param>
    /// <param name="filelocation">The file location to which we want to log</param>
    private void LogData(string log, string filelocation)
    {
        // Use a streamwriter to log the given string to the given loccation
        using (StreamWriter writer = new StreamWriter(filelocation))
        {
            // Write that data with a new line at the beginng and end of it
            writer.Write("\n" + log + "\n");
        }
    }

    /// <summary>
    /// Write out the most recent timestamp at
    /// the end of the application.
    /// </summary>
    private void OnApplicationQuit()
    {
        // Write out the latest time stamp
        File.WriteAllText(Application.streamingAssetsPath + fileLocation_latestTime, _latest_time);
    }

    /// <summary>
    /// This method will toggle this monitor on or off with a call from 
    /// a Toggle UI element
    /// </summary>
    public void ToggleMonitor()
    {
        // If we are not stopped...
        if (currentState != MonitorState.Stop)
        {
            // Then stop
            StopMonitor();
        }
        // Otherwise we are stopped...
        else
        {
            // So start again
            StartMonitor();
        }

    }

    #endregion
}
