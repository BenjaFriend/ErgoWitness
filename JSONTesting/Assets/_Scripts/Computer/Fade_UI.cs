using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade_UI : MonoBehaviour {

    #region Fields
    public Text[] regInfoitems;

    public Text sourceIpText;
    public Text destIpText;
    public Text serviceText;
    public Text portText;
    public Text transportText;
    public Text connTypeText;

    private bool showingExtra;
    #endregion

    private void OnEnable()
    {
        FadeOut();
    }


    /// <summary>
    /// Author: Ben Hoffman
    /// Set the data for all of the UI elements on this object
    /// </summary>
    /// <param name="data">The data for us to use</param>
    public void SetValues(Bro_Json data)
    {
        sourceIpText.text = "Source IP: " + data.id_orig_h;
        destIpText.text = "Dest. IP: " + data.id_resp_h;
        serviceText.text = "Service: " + data.service;
        portText.text = "Port: " + data.id_orig_p;
        transportText.text = "Protocol: " + data.proto;
        connTypeText.text = "Conn. State: " + data.conn_state;
    }

    #region Showing info on enter

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FadeIn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            FadeOut();
        }
    }*/

    public void FadeOut()
    {
        for(int i = 0; i < regInfoitems.Length; i++)
        {
            // Fade out each component
            regInfoitems[i].CrossFadeAlpha(0f, 1f, false);
        }
    }

    public void FadeIn()
    {
        for (int i = 0; i < regInfoitems.Length; i++)
        {
            // Fade out each component
            regInfoitems[i].CrossFadeAlpha(0.8f, 1f, false);
        }
    }
    #endregion
}
