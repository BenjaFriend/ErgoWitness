using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetInfo : MonoBehaviour {

    #region fields
    public Text timestamp;
    public Text source;
    public Text dest;
    public Text port;
    public Text protocol;
    #endregion


    /// <summary>
    /// Set the text of this object
    /// </summary>
    /// <param name="data">The info from a packet</param>
    public void SetText(Source_Packet data)
    {
        try
        {
            protocol.text = data.transport;
            string[] goodTimes = data.runtime_timestamp.Split('T');
            timestamp.text = goodTimes[1];
            source.text = data.packet_source.ip;
            dest.text = data.dest.ip;
            port.text = data.dest.port.ToString();
        }
        catch
        {
           // Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// Set the text with a filebeat source data
    /// </summary>
    /// <param name="data"></param>
    public void SetText(Source data)
    {
        try
        {
            // Set the time stamp to actuall be readable
            string[] goodTimes = data.runtime_timestamp.Split('T');
            timestamp.text = goodTimes[1];

            source.text = data.id_orig_h;
            dest.text = data.id_resp_h;
            //port.text = data.id_resp_p.ToString();
            protocol.text = data.proto;
        }
        catch
        {           

        }
    }

    /// <summary>
    /// Set all the text elements of this object to ""
    /// </summary>
    public void ClearText()
    {
        try
        {

            timestamp.text = "";
            source.text = "";
            dest.text = "";
            port.text = "";
            protocol.text = "";
        }
        catch
        {
        }
    }

}
