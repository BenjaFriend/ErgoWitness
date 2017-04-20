using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

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
public class RotateWithTouchPad : MonoBehaviour {

    public Transform rotatingObject;        // The Object that we want to rotate
    [SerializeField]
    private float speed = 5f;               // The speed of the object

    private Hand _hand;                     // The hand object that needs to be on this gameobject as well
    private Vector2 touchPadInput;          // The actual input from the touch pad


    void Start ()
    {
        // Get the hand componenet
        _hand = GetComponent<Hand>();
    }
	
    /// <summary>
    /// Get input from the touch pad        
    /// 
    /// Author: Ben Hoffman
    /// </summary>	
    void Update ()
    {
        touchPadInput = _hand.controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
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
            // For example, rotate an object around:
            rotatingObject.Rotate(touchPadInput.x * speed, 0f, 0f);
        }
    }
}
