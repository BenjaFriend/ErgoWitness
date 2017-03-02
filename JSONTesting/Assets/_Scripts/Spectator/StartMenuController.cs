using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the class that will  enable me to trype in the address of the ELK
/// server at the start 
/// </summary>
public class StartMenuController : MonoBehaviour {

    public InputField serverIP_Input;
    public Canvas reticleCanv;
    public Canvas startCanvas;
    public Text pingInfoTest;

    private float timeoutTime = 500f;

	// Use this for initialization
	void Start ()
    {
        // Stop player movement
        UIController.thisUIController.TogglePlayerMovement();

        // Blur the background
        BoxBlur.currentBlur.doTheBlur = true;
        pingInfoTest.text = "";
    }

    public void CheckInputField()
    {
        pingInfoTest.text = "Started ping test...";
        serverIP_Input.interactable = false;
    }


    /// <summary>
    /// This button is to be clicked after the IP 
    /// address of the server has been typed in. We need
    /// to try and ping that host and see if we get a response,
    /// if we do, then we can procede
    /// </summary>
    public void StartMonitoring()
    {
        // Set the reticle canvas as active
        reticleCanv.gameObject.SetActive(true);

        // set the ELK server to the input field
        NetworkMonitor.currentNetworkMonitor.ServerIP = serverIP_Input.text;

        // Remove any blur
        BoxBlur.currentBlur.doTheBlur = false;

        // Toggle this start menu
        UIController.thisUIController.ToggleMenu(startCanvas);

        // Start the player movement
        UIController.thisUIController.TogglePlayerMovement();

    }
}
