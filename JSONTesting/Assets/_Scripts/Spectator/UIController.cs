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
    public Movement playerMovement;     // The player movement so we can stop it on pause
    public Canvas debugInfo;            // The debug menu
    public Canvas gameMenu;             // The pause menu
    private Animator anim;      // The animator of the menu

    private bool showingMenu;           // Are we showing the menu?
    private bool isPaused;              // Are we paused?
    private int whichMethod;
    #endregion

    /// <summary>
    /// set the menus to false, enable player movement, set time scale to 1
    /// get the animator component
    /// </summary>
    void Start ()
    {
        // Get te animator for the menu
        anim = gameMenu.GetComponent<Animator>();
        Time.timeScale = 1f;

        // Make sure that me menus are OFF to start
        gameMenu.gameObject.SetActive(false);
        debugInfo.gameObject.SetActive(false);

        // Make sure that the player can move to start
        playerMovement.enabled = true;
	}
	
    /// <summary>
    /// Author: Ben Hoffman
    /// Look for input from the player and call the 
    /// necessary functions baced on it
    /// </summary>
	void Update ()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            // Show the pause menu
            ToggleMenu(gameMenu);
            // Stop the player from moving
            TogglePlayerMovement();
            // Toggle the blur that we are doing
            ToggleBlur();
        }

        // Toggle the 'debug' menu with the plus key on the num pad or d-pad down
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetAxis("DPadUpDown") < 0f)
        {
            // Show the debug menu
            ToggleMenu(debugInfo);
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
            if(menu.GetComponentInChildren<Button>() != null)
                menu.GetComponentInChildren<Button>().Select();
        }
        else
        {
            // Hide the game menu
            menu.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// If the player movement is enabled, then make it 
    /// not enabled. If we the player movement is not
    /// enabled then enable it
    /// </summary>
    private void TogglePlayerMovement()
    {
        // If the player movement is enabled...
        if (playerMovement.isActiveAndEnabled)
        {
            // Disable player movement
            playerMovement.enabled = false;
        }
        else
        {
            // Enable player movement
            playerMovement.enabled = true;
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Resume the game
    /// </summary>
    public void Resume()
    {
        // Set the time to normal
        Time.timeScale = 1f;
        // Hide the menu
        ToggleMenu(gameMenu);
        // Enable player movement
        TogglePlayerMovement();
        // Make sure that we are not blurry
        BoxBlur.currentBlur.doTheBlur = false;

    }

    /// <summary>
    /// Display the help menu, or hide the help menu
    /// </summary>
    public void ToggleHelpMenu()
    {
        // Use the animator to do this
        // If we are not showing the menu and we are in idle state....
        if (!anim.GetBool("showHelp") && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            // Show the menu
            anim.SetBool(("showHelp"), true);
        }
        // If we are showing the menu and we are in that state too
        else if (anim.GetBool("showHelp") && anim.GetCurrentAnimatorStateInfo(0).IsName("ShowHelpMenu"))
        {
            // Hide the menu
            anim.SetBool(("showHelp"), false);
        }
    }

    /// <summary>
    /// Toggle if we want time scale to be happening or not
    /// </summary>
    private void ToggleTimescale()
    {
        if(Time.timeScale == 0f)
        {
            // Set the time scale to normal
            Time.timeScale = 1f;
        }
        else
        {
            // Effectively pause all active effects that are scaled by time
            Time.timeScale = 0f;
        }

    }

    /// <summary>
    /// Just reload the current scene, which will reset everything
    /// </summary>
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Show the 'are you sure?' prompt
    /// </summary>
    /// <param name="newWhichMethod">Which method do we want to use?</param>
    public void ShowIsSure(int newWhichMethod)
    {
        whichMethod = newWhichMethod;
        anim.SetBool("showIsSure", true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if the player hits the 'yes' button</returns>
    public void IsSure()
    {
        if(whichMethod == 0)
        {
            // Call quit
            Quit();
        }
        else if(whichMethod == 1)
        {
            // Call reset
            Reset();
        }

        // Hide ths is sure menu
        whichMethod = -1;
        anim.SetBool("showIsSure", false);


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

    private void ToggleBlur()
    {
        if (BoxBlur.currentBlur.doTheBlur)
        {
            BoxBlur.currentBlur.doTheBlur = false;
        }
        else
        {
            BoxBlur.currentBlur.doTheBlur = true;
        }

    }
}
