using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Author: Ben Hoffman
/// This menu will give the player controlls over the 
/// game menu
/// </summary>
public class UIController : MonoBehaviour {

    #region Fields
    public static UIController thisUIController;

    //public Animator MenuAnim;      // The animator of the menu

    public Sprite playSprite;     // The sprite for if we want to play or not
    public Sprite pauseSprite;    // The sprite for if we want to pause
    public Button pausePlayButton;// The button for toggling on and off with moniotring

    private bool isMonitoring;              // Are we monitoring?
    private int whichMethod;
    private Canvas mainCanvas;
    #endregion

    /// <summary>
    /// set the menus to false, enable player movement, set time scale to 1
    /// get the animator component
    /// </summary>
    void Awake ()
    {
        // Set the static reference
        thisUIController = this;

        // Get te animator for the menu
        Time.timeScale = 1f;

    }


    /// <summary>
    /// Check if we want to toggle the menu or not so that we can hide it
    /// </summary>
    void Update()
    {
        // If the user presses the P button then hide all of the 
        // UI elements
        if (Input.GetKeyDown(KeyCode.P) && Input.GetKeyDown(KeyCode.O))
        {
            // Toggle is we are hiding all the UI or not
            HideAllUI();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleMonitoring();
        }

        // If the user presses start or ESC, then pause monitoring
        if (Input.GetButtonDown("Cancel"))
        {
            ToggleMonitoring();
        }
    }

    /// <summary>
    /// Toggle all the UI on or off with the main canvas
    /// field
    /// </summary>
    private void HideAllUI()
    {
        // If we ARE hiding all the UI already....
        if (!mainCanvas.enabled)
        {
            // Show the UI
            mainCanvas.enabled = true;
            // Set the streaming info to showing so that we know to update it
            StreamingInfo_UI.currentStreamInfo.IsShowing = true;
        }
        // If we are NOT hiding the UI already....
        else
        {
            // hide the UI
            mainCanvas.enabled = false;
            // Set the streaming info to not showing so that we don;t waste resources
            StreamingInfo_UI.currentStreamInfo.IsShowing = false;
        }
    }

    /// <summary>
    /// This button is to be clicked after the IP 
    /// address of the server has been typed in. We need
    /// to try and ping that host and see if we get a response,
    /// if we do, then we can procede
    /// </summary>
    public void StartMonitoring()
    {
        // Make sure that the timescale is 1
        Time.timeScale = 1f;

        // Stop the player movement, and
        isMonitoring = true;

        // Start monitoring
        ManageMonitors.currentMonitors.StartMonitoringObjects();
    }  

    #region Toggles


    /// <summary>
    /// Stop the monitoring and enable player movement
    /// </summary>
    public void ToggleMonitoring()
    {
        // If we are monitoring, then stop
        if (isMonitoring)
        {
            // Make sure that we know that we are not monitoring anymore
            isMonitoring = false;

            // Stop monitoring
            ManageMonitors.currentMonitors.StopMonitor();

            // Set the play button as active
            pausePlayButton.image.sprite = playSprite;
        }
        // Start monitoring again
        else
        {
            pausePlayButton.image.sprite = pauseSprite;

            // Stop all coroutines first becore we start monitoring
            ManageMonitors.currentMonitors.StopMonitor();

            // Make sure that our timescale is up
            //Time.timeScale = 1f;

            // Make sure that we know that we are monitoring now
            isMonitoring = true;

            // Start monitoring
            StartMonitoring();

            // Disable player movement
            //playerMovement.enabled = false;
        }

    }


    #endregion


    #region Application settings
    /// <summary>
    /// Just reload the current scene, which will reset everything
    /// </summary>
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /// <summary>
    /// Author: Ben Hoffman
    /// Exit the application
    /// </summary>
    public void Quit()
    {
        // Quit the application
        Application.Quit();
    }

    #endregion
}
