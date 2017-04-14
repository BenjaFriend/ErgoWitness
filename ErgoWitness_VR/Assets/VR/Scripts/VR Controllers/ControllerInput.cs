using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this class to handle the input given to me from the 
/// VR controller. Put this script on the left/right controlelr 
/// on the Camera Rig. This will allwo me to grab stuff with the grip
/// buttons
/// 
/// Used this guide for some help on my first Vive project:
/// https://www.raywenderlich.com/149239/htc-vive-tutorial-unity
/// 
/// Author: Ben Hoffman
/// </summary>
public class ControllerInput : MonoBehaviour
{
    [Tooltip("The layer that you do not want to be thrown.")]
    public LayerMask DontThrowLayer;

    private GameObject objectInHand;                // Use this to keep track of an object that we want to pick up
    private GameObject collidingObject;             // The object that is in our collider
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
    /// Check for input from the Vive controller, and handle either allowing the player
    /// to teleport, or to grab a device object and throw it around
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    private void Update()
    {
        // if we are pressing the grip button...
        if (Controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            // Handle the player pulling the trigger
            if (collidingObject)
            {
                GrabObject();
            }
        }

        // IF we release the grip button and we have something in our hand...
        if (Controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            // Release the object in our hand
            if (objectInHand)
            {
                ReleaseObject();
            }
        }

    }


    #region Trigger events
    public void OnTriggerEnter(Collider other)
    {
        // Check our colliding object
        CheckCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        // Check our colliding object
        CheckCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        // If we do not have a colliding object, return
        if (!collidingObject)
        {
            return;
        }
        // Clear our colliding object, because it has left our trigger
        collidingObject = null;
    }
    #endregion


    /// <summary>
    /// Make sure that the object we are given hsa a rigidbody, andthat we do not
    /// already have a colliding object
    /// </summary>
    /// <param name="c">Something that we are colliding with</param>
    private void CheckCollidingObject(Collider c)
    {
        // If we have a colliding object, or this object does not have a RB, then return
        if (collidingObject || !c.GetComponent<Rigidbody>())
        {
            return;
        }

        // Otherwise, we want to set our colliding object reference to this.
        collidingObject = c.gameObject;
    }


    /// <summary>
    /// Grab the object, and query elasticsearch to get more informatino about it
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    private void GrabObject()
    {
        // Add a fixed joint to the object that we are collidnig with
        objectInHand = collidingObject;
        // Set our colliding object to null, because now it is in our hand
        collidingObject = null;

        // Add a fixed joint to our game object
        var joint = AddFixedJoint();
        // Connect our object in our hand to that fixed joint
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Add a fixed joint to our rigidbody
    /// </summary>
    /// <returns>A joint that we want to connect to our controller</returns>
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    /// <summary>
    /// Destroy the fixed joint that is connecting our objects
    /// </summary>
    private void ReleaseObject()
    {
        // If we have a fixed joint...
        if (GetComponent<FixedJoint>())
        {
            // Disconnect the fixed joint from it's conencted body
            GetComponent<FixedJoint>().connectedBody = null;
            // Destroy the fixed joint component
            Destroy(GetComponent<FixedJoint>());

            // As long as what we are holding is not a computer object, throw it
            if (objectInHand.layer != DontThrowLayer)
            {
                // Set our object in our hands velocity to the controllers, so that we can throw it
                objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
                // Do the same thing with the angular velocity, so it does the right way
                objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
            }
        }

        // We no longer have something in our hand, so set it to null
        objectInHand = null; 
    }

}
