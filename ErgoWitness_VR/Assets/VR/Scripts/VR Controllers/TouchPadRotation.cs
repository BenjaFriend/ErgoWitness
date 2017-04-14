using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I will use this script to allow the player to be abled
/// to use their touch apd to rotate an object. For example,
/// in Tilt Brush you can use it to rotate the menu around your controller.
/// I want to do something similar, but allow it be done to any object.
/// 
/// Put this on the steam VR controller and then you are done. But you can also combine this with 
/// trigger/grip input, to pick up an object and them be able to rotate it aronud or something.
/// 
/// Author: Ben Hoffman
/// </summary>
public class TouchPadRotation : MonoBehaviour {

    public Transform rotatingObject;    // the object that we want to rotate
    [SerializeField]    
    private float speed = 3f;           // How fast we want to rotate

	private SteamVR_TrackedObject trackedObj;       // The tracked object that is the controller

    private SteamVR_Controller.Device Controller    // The device property that is the controller, so that we can tell what index we are on
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private Vector2 touchPadInput;              // The actual input from the touch pad

    private void Awake()
    {
        // Get the tracked object componenet 
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
	
	
    /// <summary>
    /// Get input from the touch pad        
    /// 
    /// Author: Ben Hoffman
    /// </summary>
	void Update ()
    {
        // Gather the input from the touch pad
        touchPadInput = Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
	}

    /// <summary>
    /// Actually rotate around the controller object. Do this
    /// in late update so that we get the right axis while moving
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    void LateUpdate()
    {
        // If we have some kind of input...
        if (touchPadInput.x != 0f)
        {
            // ============ Do whaterver you want here =================== //

            // For example, rotate an object around:
            rotatingObject.Rotate(touchPadInput.x * speed, 0f,0f);
        }
    }
}
