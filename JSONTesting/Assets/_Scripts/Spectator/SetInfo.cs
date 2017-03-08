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
            timestamp.text = data.runtime_timestamp;
            source.text = data.packet_source.ip;
            dest.text = data.dest.ip;
            port.text = data.dest.port.ToString();
            protocol.text = data.proto;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
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
            timestamp.text = data.runtime_timestamp;
            source.text = data.id_orig_h;
            dest.text = data.id_resp_h;
            port.text = data.id_resp_p.ToString();
            protocol.text = data.proto;
        }
        catch (Exception e)
        {
            
            Debug.Log(e.Message);
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
        catch (Exception e)
        {

        }
    }

}
