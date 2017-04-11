using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this class to handle the input given to me from the 
/// VR controller. Put this script on the left/right controlelr 
/// on the Camera Rig.
/// 
/// Author: Ben Hoffman
/// </summary>
public class ControllerInput : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;       // The tracked object that is the controller

    private SteamVR_Controller.Device Controller    // The device property that is the controller, so that we can tell what index we are on
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        // Get the tracked object componenet
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    /// <summary>
    /// Check for input from the Vive controller
    /// </summary>
    private void Update()
    {
        // If we are pulling the trigger on this controller...
        if (Controller.GetHairTrigger())
        {
            // Just log it to the console for now
            Debug.Log(gameObject.name + " Trigger Press");
            // Allow the user to pick up an object 
        }

        // If we press the touchpad button down...
        if (Controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            // We want to allow the user to pick a location to teleport to
            Debug.Log(gameObject.name + " Trackpad Press");

        }
    }


}
