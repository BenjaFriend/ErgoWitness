using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMonitoringButton : MonoBehaviour, IVirtualButton
{
    private Image image;     // The image that we want to change the sprite of

    private bool isMonitoring;

    private void Start()
    {
        // Get the image componenet
        image = GetComponentInChildren<UnityEngine.UI.Image>();
        // We are not monitoring
        isMonitoring = false;
    }

    public void ButtonAction()
    {
        Debug.Log("Start Monitoring!!");
        // Toggle Monitoring of the stuffs
        // Swap the icon of the button
        // Call the monitor manager and start moniotring

        // If we are monitoring, then stop
        if (isMonitoring)
        {
            // Make sure that we know that we are not monitoring anymore
            isMonitoring = false;

            // Stop monitoring
            ManageMonitors.currentMonitors.StopMonitor();

            // Set the play button as active
           // pausePlayButton.image.sprite = playSprite;
        }
        // Start monitoring again
        else
        {
            //pausePlayButton.image.sprite = pauseSprite;

            // Stop all coroutines first becore we start monitoring
            ManageMonitors.currentMonitors.StopMonitor();

            // Make sure that we know that we are monitoring now
            isMonitoring = true;
            ManageMonitors.currentMonitors.StartMonitoringObjects();
        }
    }

    public void ShowHover()
    {
        // Change the color of the text to a hover color of some kind
    }

    public void HideHover()
    {
        // Change the color of the text back
    }

}
