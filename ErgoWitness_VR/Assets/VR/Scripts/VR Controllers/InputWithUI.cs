using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWithUI : MonoBehaviour {

    public LayerMask uiMask;              // The UI layer  
    private Vector3 _hitPoint;


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

    // Check for a trigger press, to see if we want to raycast against a UI element
    private void Update()
    {
        if()


    }

    /// <summary>
    /// Show the line renderer at the start point, and where ever we are hitting
    /// </summary>
    private void ShowLine()
    {

    }

}
