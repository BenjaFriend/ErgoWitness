using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Ben Hoffman
/// This menu will give the player controlls over the 
/// game menu
/// </summary>
public class UIController : MonoBehaviour {
    
    #region Fields
    public Movement playerMovement;
    public Canvas debugInfo;
    public Canvas gameMenu;

    private bool showingMenu;
    private bool isPaused;
    #endregion

    // Use this for initialization
    void Start ()
    {
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
            ToggleMenu(gameMenu);
            TogglePlayerMovement();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
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
        // Hide the menu and allow the player to move again
        ToggleMenu(gameMenu);
        TogglePlayerMovement();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Exit the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
