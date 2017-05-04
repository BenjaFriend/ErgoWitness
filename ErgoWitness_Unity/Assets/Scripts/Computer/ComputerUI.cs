using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerUI : MonoBehaviour {

    #region Fields
    public Image backgroundImage;
    public Text sourceIpText;
    public Text sourcePortText;
    public Text destIpText;
    public Text destPortText;
    public Text serviceText;
    public Text transportText;
    #endregion


    /// <summary>
    /// Author: Ben Hoffman
    /// Set the data for all of the UI elements on this object
    /// </summary>
    /// <param name="data">The data for us to use</param>
    public void SetValues(Source data)
    {
        sourceIpText.text = "Source IP: " + data.id_orig_h.ToString();
        // Source port
        //sourcePortText.text = "Source Port: " + data.id_orig_p.ToString();


        destIpText.text = "Dest. IP: " + data.id_resp_h.ToString();
        // Dest port
        //destPortText.text = "Dest. Port: " + data.id_resp_p.ToString();

        if (data.service == null)
        {
            serviceText.text = "Service: Null";
        }
        else
        {
            serviceText.text = "Service: " + data.service.ToString();
        }

        if (data.proto == null)
        {
            transportText.text = "Protocol: Null";
        }
        else
        {
            transportText.text = "Protocol: " + data.proto.ToString();
        }

    }
}
