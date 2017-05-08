using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will allow me to change the server IP to something 
/// new with the input field in the options menu
/// </summary>
public class ServerIP : MonoBehaviour {

    public InputField serverInput;

    void OnEnable()
    {
        if (PlayerPrefs.HasKey("serverIP"))
        {
            serverInput.text = PlayerPrefs.GetString("serverIP");
            // Actually set the server IP
            SetServerIP();
        }
    }

    /// <summary>
    /// Send the server IP info to the necessary monitors
    /// </summary>
    public void SetServerIP()
    {
        GetComponentInParent<UIController>().monitorManager.SetServerIP(serverInput.text);
        //ManageMonitors.currentMonitors.SetServerIP(serverInput.text);

        PlayerPrefs.SetString("serverIP", serverInput.text);
    }
}