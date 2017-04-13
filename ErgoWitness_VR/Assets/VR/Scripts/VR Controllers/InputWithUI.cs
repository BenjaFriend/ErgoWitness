using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputWithUI : MonoBehaviour {
    
    public bool shouldClick;            // If this is true then we should be able to click on a button
            
    private IVirtualButton _button;     // Keep track of this object

    private SteamVR_TrackedController _controller;  // The controller device that we can use to listen for the delegate methods

    private SteamVR_TrackedObject trackedObj;       // The tracked object that is the controller

    private SteamVR_Controller.Device ControllerDevice    // The device property that is the controller, so that we can tell what index we are on
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        // Get the tracked object componenet
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    /// <summary>
    /// Subscribe our method to the steam VR tracked controller
    /// </summary>
    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.TriggerClicked += TriggerPressed;
    }

    /// <summary>
    /// Remove our methods from the delegate events of the steam VR tracked controller.
    /// This will prevent memory errors
    /// </summary>
    private void OnDisable()
    {
        _controller.TriggerClicked -= TriggerPressed;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Button"))
        {
            // We should be able to click, becuse we are touching a button
            shouldClick = true;
            // Get the button interface of this object
            _button = other.GetComponent<IVirtualButton>();
            // Show the hover on this button
            _button.ShowHover();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // We should not click anymore
        shouldClick = false;
        // Hide the hover of the button object
        _button.HideHover();
        // Setthe object as null so that we know that we don't have a button anymore
        _button = null;
    }

    /// <summary>
    /// Show the line renderer at the start point, and where ever we are hitting
    /// 
    /// Author: Ben hoffman
    /// </summary>
    private void TriggerPressed(object sender, ClickedEventArgs e)
    {
        Debug.Log("Trigger pressed!");

        // Play some kind of animation tot tlel the user that they clicked

        // If we have a UI object that we can interact with, then
        if (shouldClick)
        {
            // Trigger some haptic feedback hit we hit something
            ControllerDevice.TriggerHapticPulse(750);

            // Do the button action on the thing that we are pressing
            _button.ButtonAction();
        }
    }

}
