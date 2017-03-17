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

    public Movement playerMovement;     // The player movement so we can stop it on pause
    public Automated_Camera autoCam;
    public Animator MenuAnim;      // The animator of the menu

    public Sprite playSprite;
    public Sprite pauseSprite;
    public Button pausePlayButton;

    public Text ControlButton_Text;

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

        // Make sure that the player can move to start
        playerMovement.enabled = false;
        autoCam.enabled = true;

        // Get the main canvas element so that we can toggle it on and off
        mainCanvas = GetComponentInChildren<Canvas>();
    }

    /// <summary>
    /// Check if we want to toggle the menu or not so that we can hide it
    /// </summary>
    void Update()
    {
        // If the user presses the P button then hide all of the 
        // UI elements
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Toggle is we are hiding all the UI or not
            HideAllUI();
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
    /// This method will toggle between the player being in control,
    /// and the camera automatically rotating
    /// </summary>
    public void TogglePlayerControl()
    {
        // If the player is in controll, then make then not in controll
        if (playerMovement.enabled)
        {
            // Disable player movement
            playerMovement.enabled = false;

            // Sleep the player rigidbody
            playerMovement.Rb.Sleep();

            //Enable camera movement
            autoCam.enabled = true;

            // Change the UI button to 'Control'
            ControlButton_Text.text = "Control";
        }
        else
        {
            // Disable camera movement
            autoCam.enabled = false;

            // Enable player movement
            playerMovement.enabled = true;

            // Wake up the player rigidbody
            playerMovement.Rb.WakeUp();

            // Change the UI button to 'Auto'
            ControlButton_Text.text = "Auto";
        }
    }

	/// <summary>
	/// Toggles the main panels of the UI.
	/// </summary>
	public void ToggleMainPanels()
	{
		// If we ARE showing the menu...
		if(MenuAnim.GetBool("showMain"))
		{
			// Hide it
			MenuAnim.SetBool("showMain", false);
            StreamingInfo_UI.currentStreamInfo.IsShowing = false;

        }
        // if we are NOT showing the menu...
        else
		{
            // Show the menu
            StreamingInfo_UI.currentStreamInfo.IsShowing = true;
            MenuAnim.SetBool("showMain", true);
		}
	}

    /// <summary>
    /// Stop the monitoring and enable player movement
    /// </summary>
    public void ToggleMonitoring()
    {
        // If we are monitoring, then stop
        if (isMonitoring)
        {
            //Time.timeScale = 0f;

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

    /// <summary>
    /// Author: Ben Hoffman
    /// If the given menu is active, then make it not active.
    /// If the given menu is inactive, then make it active.
    /// </summary>
    /// <param name="menu"></param>
    public void ToggleMenu(Canvas menu)
    {
        // If this menu is active in the hierachy...
        if (!menu.gameObject.activeInHierarchy)
        {
            // Show the game menu
            menu.gameObject.SetActive(true);

            // Select the first button if there is one, this will allow me to traverse the 
            // menu with a controller          
           if (menu.GetComponentInChildren<Button>() != null)
                menu.GetComponentInChildren<Button>().Select();
        }
        else
        {
            menu.gameObject.SetActive(false);
        }
        
    }

    /// <summary>
    /// Display the help menu, or hide the help menu
    /// </summary>
    public void ToggleHelpMenu()
    {
        // Use the animator to do this
        // If we are not showing the menu and we are in idle state....
        if (!MenuAnim.GetBool("showHelp") && MenuAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            // Show the menu
            MenuAnim.SetBool(("showHelp"), true);
        }
        // If we are showing the menu and we are in that state too
        else if (MenuAnim.GetBool("showHelp") && MenuAnim.GetCurrentAnimatorStateInfo(0).IsName("ShowHelpMenu"))
        {
            // Hide the menu
            MenuAnim.SetBool(("showHelp"), false);
        }
    }
    #endregion

    #region Asking the player if they are sure

    /// <summary>
    /// Show the 'are you sure?' prompt
    /// </summary>
    /// <param name="newWhichMethod">Which method do we want to use?</param>
    public void ShowIsSure(int newWhichMethod)
    {
        whichMethod = newWhichMethod;

        MenuAnim.SetBool("showIsSure", true);

    }

    /// <summary>
    /// Transition the is sure stuff out
    /// </summary>
    public void HideIsSure()
    {
        whichMethod = -1;

        MenuAnim.SetBool("showIsSure", false);
    }

    /// <summary>
    /// This will be called when the 'Yes' option is sure
    /// </summary>
    /// <returns>True if the player hits the 'yes' button</returns>
    public void IsSure()
    {
        if (whichMethod == 0)
        {
            // Call quit
            Quit();
        }
        else if (whichMethod == 1)
        {
            // Call reset
            Reset();
        }

        // Hide ths is sure menu
        whichMethod = -1;

        MenuAnim.SetBool("showIsSure", false);
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
