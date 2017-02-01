using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade_UI : MonoBehaviour {

    #region Fields
    public Text[] regInfoitems;
    public Text[] extraInfoItems;

    public Text sourceIpText;
    public Text destIpText;
    public Text macAddrText;
    public Text portText;
    public Text transportText;

    private bool showingExtra;
    #endregion


    private void Awake()
    {
        FadeOut();

        HideExtraInfo();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Set the data for all of the UI elements on this object
    /// </summary>
    /// <param name="data">The data for us to use</param>
    public void SetValues(Source data)
    {
        
        sourceIpText.text = "Source IP: " + data.source.ip;
        destIpText.text = "Dest. IP: " + data.dest.ip;
        portText.text = "Type: " + data.type;
        macAddrText.text = "Source MAC: " + data.source.mac;
        transportText.text = "Transport: " + data.transport;
    }

    #region Showing info on enter

    private void OnTriggerEnter(Collider other)
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

            HideExtraInfo();
        }
    }

    private void FadeOut()
    {
        for(int i = 0; i < regInfoitems.Length; i++)
        {
            // Fade out each component
            regInfoitems[i].CrossFadeAlpha(0f, 1f, false);
        }
    }

    private void FadeIn()
    {
        for (int i = 0; i < regInfoitems.Length; i++)
        {
            // Fade out each component
            regInfoitems[i].CrossFadeAlpha(0.8f, 1f, false);
        }
    }
    #endregion


    #region Showing extra info

    /// <summary>
    /// Author: Ben Hoffman
    /// Toggle if we are showing the extra info or not
    /// </summary>
    public void ToggleExtra()
    {
        if (showingExtra)
        {
            HideExtraInfo();
        }
        else
        {
            ShowExtraInfo();
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Show extra info, like the port, 
    /// </summary>
    private void ShowExtraInfo()
    {
        showingExtra = true;
        for (int i = 0; i < extraInfoItems.Length; i++)
        {
            // Fade in each component
            extraInfoItems[i].CrossFadeAlpha(0.8f, 0.5f, false);
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Hide the extra info when we either leave, or we press 
    /// it again
    /// </summary>
    private void HideExtraInfo()
    {
        showingExtra = false;
        for(int i = 0; i < extraInfoItems.Length; i++)
        {
            // Fade out each component and set as inactive
            extraInfoItems[i].CrossFadeAlpha(0f, 0.5f, false);
        }
    }

    #endregion
}
