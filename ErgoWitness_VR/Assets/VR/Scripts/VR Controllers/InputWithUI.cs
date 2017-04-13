using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputWithUI : MonoBehaviour {

    public LayerMask uiMask;              // The UI layer  
    public float rayDistance = 100f;      // How far we want to raycast
    public GameObject hitpointSprite;     // What we want to show the user where we are raycasting
    public LineRenderer lineRend;         // The line renderer to be draw between our controller and our target

    private Ray _raycast;           // The raycast that will come out of the controller
    private RaycastHit _rayHit;     // The raycast hit information that will be used when we hit an object
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
        // Instantiate the raycast we will use
        _raycast = new Ray(transform.position, transform.forward);

        lineRend.SetPosition(0, transform.position);
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

    /// <summary>
    /// Use a raycast to detemrine if we are looking at a UI element, if we are
    /// then draw a small sprite where we are aiming, and show our line renderer
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    private void Update()
    {
        // If the raycast that we are sending out hits a UI object...
        if (Physics.Raycast(_raycast, out _rayHit, rayDistance, uiMask))
        {
            // We are hitting somethign on the UI mask
            // Draw the sprite that we on the hit point
            hitpointSprite.transform.position = _rayHit.point;
            hitpointSprite.transform.LookAt(transform.position);

            // Enable the line renderer
            lineRend.enabled = true;
            // Set the start point to our position
            lineRend.SetPosition(0, transform.position);
            // Set the point that we are hitting
            lineRend.SetPosition(1, _rayHit.point);
        }
        else
        {
            // Disable the line renderer
            lineRend.enabled = false;
        }

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
        if (_rayHit.collider)
        {
            // Get a button component on that object and activate it
            Debug.Log("Pressed button: " + _rayHit.collider.name);
            // trigger some haptic feedback hit we hit something
            ControllerDevice.TriggerHapticPulse(400);
            
            // Press the button that we are lookin at
        }
    }

}
