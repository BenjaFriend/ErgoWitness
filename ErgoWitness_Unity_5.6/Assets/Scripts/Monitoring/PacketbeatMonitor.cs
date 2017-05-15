using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the packetbeat monitor 
/// Author: Ben Hoffman
/// </summary>
public class PacketbeatMonitor : MonitorObject {

    private int packetPerQuery;

    private Packetbeat_Json_Data _packetbeatJsonData;  // The JSON data that we are gonna keep track of
    public bool assumeHttp = false;                    // If this is true then all traffic on ports 80 and 8080 will be considered HTTP traffic
    public ConnectionController connectionController;
    private Coroutine _CheckDataRoutine;

    
    private enum CheckDataStates { Running, Done }
    private CheckDataStates checkingState = CheckDataStates.Done;

    public int PacketPerQuery { get { return packetPerQuery; } }

    /// <summary>
    /// Start the necessary finite state machine with the
    /// specific data for this object
    /// </summary>
    public override void StartMonitor()
    {
        // Make sure that the FSM knows we are starting again
        base.StartMonitor();

        // Instantiate the data for our request
        _packetbeatJsonData = new Packetbeat_Json_Data();

        // Start the finite satate machine for the web request
        StartCoroutine(FSM(_packetbeatJsonData));


        checkingState = CheckDataStates.Done;
    }

    /// <summary>
    /// Check if this data is of the packetbeat type.
    /// If it is, then check it like we need to for packetbeat
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public override void CheckRequestData<T>(T data)
    {
        // Attempt to cast the data to the type that we want it to be
        if (typeof(T) == typeof(Packetbeat_Json_Data))
        {
            // Cast the object as necessary
            _packetbeatJsonData = data as Packetbeat_Json_Data;

            // If we are currently running a coroutine then quit that
            if (_CheckDataRoutine != null)
            {
                // Stop the current cortoutine after it finishes
                //StopCoroutine(_CheckDataRoutine);
            }

            if(checkingState == CheckDataStates.Done)
            {
                // Start a new coroutine
                _CheckDataRoutine = StartCoroutine(CheckData(_packetbeatJsonData));
            }           
    
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Loop through the data that we have and send it to the netflow
    /// controller if we should
    /// </summary>
    /// <param name="packetDataObj"></param>
    private IEnumerator CheckData(Packetbeat_Json_Data packetDataObj)
    {
        
        // ================= Check and make sure that our data is valid =====================
        // Make sure that our data is not null
        if (packetDataObj.hits.hits.Length == 0)
        {
            _UseLastSuccess = true;

            // Tell this to use the last successful query
            yield break;
        }

        // Let this know that we no longer need to bank on the last success
        if (_UseLastSuccess)
        {
            _UseLastSuccess = false;
        }

        // ============= Keep track of stuff to prevent duplicates =======================
        packetPerQuery = 0;
        // Set our latest packetbeat time to the most recent one
        _latest_time = packetDataObj.hits.hits[packetDataObj.hits.hits.Length - 1]._source.runtime_timestamp;
        checkingState = CheckDataStates.Running;
        // ============== Actually loop through our hits data  =========================
        for (int i = 0; i < packetDataObj.hits.hits.Length; i++)
        {
            // Set the integer IP values of this object
            SetIntegerValues(packetDataObj.hits.hits[i]._source);
            
            // As long as what we got from those IP's is valid:
            if (packetDataObj.hits.hits[i]._source.destIpInt != 0 && packetDataObj.hits.hits[i]._source.sourceIpInt != 0)
            {
                // Change the protocol to HTTP if we want to, this is optional because
                // sometimes it is techincally incorrect
                if (assumeHttp && packetDataObj.hits.hits[i]._source.dest.port == 80 ||
                   packetDataObj.hits.hits[i]._source.dest.port == 8080)
                {
                    // This traffic is HTTP
                    packetDataObj.hits.hits[i]._source.transport = "http";
                }

                // Send the data to the netflow controller
                connectionController.CheckPacketbeatData(
                    packetDataObj.hits.hits[i]._source.sourceIpInt,
                    packetDataObj.hits.hits[i]._source.destIpInt,
                    packetDataObj.hits.hits[i]._source.transport);
                packetPerQuery++;
            }

            // Get them frames
            yield return null;
        }
        checkingState = CheckDataStates.Done;
    }


    /// <summary>
    /// Take in a source object, and set it's integer values
    /// </summary>
    /// <param name="FilebeatSource"></param>
    private void SetIntegerValues(Source_Packet packetbeatSource)
    {
        // Calculate the INTEGER version of the SOURCE IP address
        packetbeatSource.sourceIpInt =
            IpToInt(packetbeatSource.packet_source.ip);

        // Calculate the INTEGER version of the DESTINATION IP address
        packetbeatSource.destIpInt =
            IpToInt(packetbeatSource.dest.ip);
    }
}
