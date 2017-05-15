using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the monitor class that I will use in order to have Bro data coming
/// in from the ELK server. 
/// </summary>
public class BroMonitor : MonitorObject {

    // The JSON class of data for the bro data
    private Json_Data _broData;

    private Coroutine _checkDataRoutine;

    private int packetPerQuery = 0;

    private enum CheckDataStates { Running, Done }
    private CheckDataStates checkingState = CheckDataStates.Done;

    public int PacketPerQuery { get { return packetPerQuery; } }

    /// <summary>
    /// This will start the FSM with our specific stype of data
    /// </summary>
    public override void StartMonitor()
    {
        // Make sure that the FSM knows we are starting again
        base.StartMonitor();

        // Send it packetbeat data
        _broData = new Json_Data();

        // Start the finite satate machine for the web request
        StartCoroutine(FSM(_broData));

        checkingState = CheckDataStates.Done;

    }

    /// <summary>
    /// Check if this data is of the bro type.
    /// If it is, then check it like we need to for packetbeat
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public override void CheckRequestData<T>(T data)
    {
        if (typeof(T) == typeof(Json_Data))
        {
            // Cast the object as necessary
            _broData = data as Json_Data;

            // Make sure that we are not already running a coroutine
            if(_checkDataRoutine != null)
            {
                // If we are then stop it
                StopCoroutine(_checkDataRoutine);
            }
            // As long as we have finished checking out the previous request
            if (checkingState == CheckDataStates.Done)
            {
                // Otherwise, start a new coroutine
                _checkDataRoutine = StartCoroutine(CheckData(_broData));
            }

        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Check the data for a Json Object
    /// </summary>
    /// <param name="dataObject"></param>
    private IEnumerator CheckData(Json_Data dataObject)
    {
        // ================= Check and make sure that our data is valid ===================== //
        // Make sure that our data is not null
        if (dataObject.hits.hits.Length == 0)
        {
            _UseLastSuccess = true;

            // Tell this to use the last successful query
            yield break; //return;
        }

        // Let this know that we no longer need to bank on the last success
        if (_UseLastSuccess)
        {
            _UseLastSuccess = false;
        }

        // ============= Keep track of stuff to prevent duplicates ===============

        // Set our latest packetbeat time to the most recent one
        _latest_time = dataObject.hits.hits[0]._source.runtime_timestamp;

        packetPerQuery++;
        // Keep track of the state of this data checking
        checkingState = CheckDataStates.Running;

        // Send the data to the game controller for all of our hits
        for (int i = 0; i < dataObject.hits.hits.Length; i++)
        {
            // Set the integer IP values if this source
            SetIntegerValues(dataObject.hits.hits[i]._source);

            // If the source or dest ip's are 0 then break
            if(dataObject.hits.hits[i]._source.sourceIpInt != 0 && dataObject.hits.hits[i]._source.destIpInt != 0)
            {
                // Send the bro data to the game controller, and add it to the network
                DeviceManager.Instance.CheckIp(dataObject.hits.hits[i]._source);
            }

            // Make sure we get them smooth frames
            yield return null;
        }
        packetPerQuery = 0;
        // We are done chekcing this now
        checkingState = CheckDataStates.Done;

    }

    /// <summary>
    /// Take in a source object, and set it's integer values
    /// </summary>
    /// <param name="FilebeatSource"></param>
    private void SetIntegerValues(Source filebeatSource)
    {
        // Calculate the INTEGER version of the SOURCE IP address
        filebeatSource.sourceIpInt =
            IpToInt(filebeatSource.id_orig_h);

        // Calculate the INTEGER version of the DESTINATION IP address
        filebeatSource.destIpInt =
            IpToInt(filebeatSource.id_resp_h);
    }

}
