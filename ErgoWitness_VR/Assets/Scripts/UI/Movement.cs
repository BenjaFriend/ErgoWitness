using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// purpose of class: To make a camera movement method
/// that will allow the spectator to fly through the space 
/// and look at each node ('computer') and their connections
/// </summary>
[RequireComponent (typeof(Rigidbody))]
public class Movement : MonoBehaviour {

    #region Fields
    public float xMoveSpeed;    // X axis movement speed
    public float yMoveSpeed;    // Y axis movement speed
    public float maxSpeed_Norm;      // The thing that I will clamp velocity with
    public float speed_Multiplier;   // The max speed while we hold shift or left analog down
    public float panSpeed;
    public Transform camTransform;
    public Transform character;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public float smoothTime = 5f;
    public bool useController = false;

    private Rigidbody rb;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private float xRot;
    private float yRot;
    private float angleX;
    private float dt;
    #endregion

    public Rigidbody Rb { get { return rb; } set { rb = value; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camTransform.localRotation;

        dt = Time.deltaTime;
    }

    /// <summary>
    /// Check for update from the user as to if they want to 
    /// toggle the camera
    /// </summary>
    void Update()
    {
        // If we press the camera toggle input, then toggle the use of the controller
        if (Input.GetButtonDown("ControllerToggle"))
        {
            ToggleControllerUse();
        }
    }

	/// <summary>
    /// Check for input from the user and call the functions to calculate the movement
    /// </summary>
	void FixedUpdate ()
    {
        // Store delta time so we onl reference it once
        dt = Time.deltaTime;

        Rotate();

        Move();
	}

    /// <summary>
    /// Author: Ben Hoffman
    /// Toggle whether or not we are using a controller 
    /// with the press of a button
    /// </summary>
    private void ToggleControllerUse()
    {
        if (useController)
        {
            useController = false;
        }
        else
        {
            useController = true;
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To do the movement on this game object
    /// </summary>
    void Move()
    {
        // Reset the velocity
        rb.velocity = Vector3.zero;
        // The z movement
        rb.velocity += (transform.forward * Input.GetAxis("Vertical") * xMoveSpeed * dt);
        // The X movement
        rb.velocity += (transform.right * Input.GetAxis("Horizontal") * xMoveSpeed * dt);

        // Press Q or the LEFT BUMPER to move up
        if (Input.GetKey(KeyCode.Q) || Input.GetAxis("LeftBumper") > 0f)
        {
            // Move up
            rb.velocity += transform.up * yMoveSpeed * dt;
        }
        // Press E or RIGHT BUMPER to move down
        if (Input.GetKey(KeyCode.E) || Input.GetAxis("RightBumper") > 0f)
        {
            // Move down
            rb.velocity += -transform.up * yMoveSpeed * dt;
        }
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed_Norm);

        if (Input.GetButton("Speedy"))
        {
            rb.velocity *= speed_Multiplier;
        }

    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Use the right stick on the controller to rotate
    /// Or follow the mouse. This is a modified version of 
    /// Unity's "MouseLook.cs" script from the standard
    /// assets. Modified to use the controller
    /// </summary>
    private void Rotate()
    {
        
        if (!useController)
        {
            yRot = Input.GetAxis("Mouse X") * panSpeed;
            xRot = Input.GetAxis("Mouse Y") * panSpeed;
        }
        else
        {
            yRot = Input.GetAxis("RS_Horizontal") * panSpeed;
            xRot = -Input.GetAxis("RS_Vertical") * panSpeed;
        }

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        // Do some smoothing
        character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
            smoothTime * dt);
        camTransform.localRotation = Quaternion.Slerp(camTransform.localRotation, m_CameraTargetRot,
            smoothTime * dt);
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
