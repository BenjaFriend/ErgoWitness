using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendInfoToMonitor : MonoBehaviour {

    public MinMaxSliders hours;
    public MinMaxSliders minutes;
    public MinMaxSliders seconds;

    private MonitorObject[] monitors;

    private void Start()
    {
        // Get the moniotrs
        monitors = GetComponentsInChildren<MonitorObject>();
    }


    public void SendDataTo()
    {
        // Create the min and max timestamps based on the values that we have
        string minTimestamp = GenerateTimeStamp(hours.MinValue, minutes.MinValue, seconds.MinValue);
        string maxTimestamp = GenerateTimeStamp(hours.MaxValue, minutes.MaxValue, seconds.MaxValue);

        for(int i = 0; i < monitors.Length; i++)
        {
            // Tell the moniotr to query between these two now
            monitors[i].QueryBetweenTimes(minTimestamp, maxTimestamp);
        }


    }

    public void UseRealTime()
    {
        for (int i = 0; i < monitors.Length; i++)
        {
            // Tell the moniotr to query between these two now
            monitors[i].UseRealTime();
        }
    }



    /// <summary>
    /// Take in a couple fields, and then generate a timestamp that 
    /// is in the proper format for this.
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <returns></returns>
    public string GenerateTimeStamp(int hour, int minute, int second)
    {

        string timeStamp = System.DateTime.Today.Year.ToString() + "-";

        // Make sure we have proper format on the month
        if (System.DateTime.Today.Month < 10)
        {
            timeStamp += "0" + System.DateTime.Today.Month.ToString() + "-";
        }
        else
        {
            timeStamp += System.DateTime.Today.Month.ToString() + "-";
        }
        // Handle the day
        if (System.DateTime.Today.Day < 10)
        {
            timeStamp += "0" + System.DateTime.Today.Day.ToString();
        }
        else
        {
            timeStamp += System.DateTime.Today.Day.ToString();
        }

        if (hour < 10)
        {
            timeStamp += "T0" + hour + ":";
        }
        else
        {
            timeStamp += "T" + hour + ":";
        }
        
        if(minute < 10)
        {
            timeStamp += "0" + minute + ":";
        }
        else
        {
            timeStamp += minute + ":";
        }

        if(second < 10)
        {
            timeStamp += "0" + second;

        }
        else
        {
            timeStamp += second;
        }

        timeStamp += ".000Z";

        return timeStamp;

    }

}
