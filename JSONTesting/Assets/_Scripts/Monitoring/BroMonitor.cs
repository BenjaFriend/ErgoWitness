using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroMonitor : MonitorObject {


    private Json_Data _broData;


    public override void StartMonitor()
    {
        // Make sure that the FSM knows we are starting again
        base.StartMonitor();

        // Send it packetbeat data
        _broData = new Json_Data();

        // Start the finite satate machine for the web request
        StartCoroutine(FSM(_broData));
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
            CheckData(_broData);
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
    private void CheckData(Json_Data dataObject)
    {
        // ================= Check and make sure that our data is valid =====================
        // Make sure that our data is not null
        if (dataObject.hits.hits.Length == 0)
        {
            _UseLastSuccess = true;

            // Tell this to use the last successful query
            return;
        }

        // Let this know that we no longer need to bank on the last success
        if (_UseLastSuccess)
        {
            _UseLastSuccess = false;
        }

        // ============= Keep track of stuff to prevent duplicates ===============

        // Set our latest packetbeat time to the most recent one
        _latest_time = dataObject.hits.hits[0]._source.runtime_timestamp;

        // Send the data to the game controller for all of our hits
        for (int i = 0; i < dataObject.hits.hits.Length; i++)
        {
            // Set the integer IP values if this source
            SetIntegerValues(dataObject.hits.hits[i]._source);

            // Send the bro data to the game controller, and add it to the network
            DeviceManager.currentDeviceManager.CheckIp(dataObject.hits.hits[i]._source);
        }
    }

}
