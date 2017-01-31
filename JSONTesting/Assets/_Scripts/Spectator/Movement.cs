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
    private Rigidbody rb;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

	/// <summary>
    /// Check for input from the user and call the functions to calculate the movement
    /// </summary>
	void Update ()
    {
        Rotate();

        Move();
	}

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To do the movement on this game object
    /// </summary>
    void Move()
    {
        rb.velocity = Vector3.zero;
        // The z movement
        rb.velocity += (transform.forward * Input.GetAxis("Vertical") * xMoveSpeed * Time.deltaTime);
        // The X movement
        rb.velocity += (transform.right * Input.GetAxis("Horizontal") * xMoveSpeed * Time.deltaTime);
        // Press Q or the LEFT BUMPER to move up
        if (Input.GetKey(KeyCode.Q) || Input.GetAxis("LeftBumper") > 0f)
        {
            // Move up
            rb.velocity += transform.up * yMoveSpeed * Time.deltaTime;
        }
        // Press E or RIGHT BUMPER to move down
        if (Input.GetKey(KeyCode.E) || Input.GetAxis("RightBumper") > 0f)
        {
            // Move down
            rb.velocity += -transform.up * yMoveSpeed * Time.deltaTime;
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
    /// Or follow the mouse
    /// </summary>
    private void Rotate()
    {
        transform.Rotate(
            Input.GetAxis("RS_Vertical") * Time.deltaTime * panSpeed,
            Input.GetAxis("RS_Horizontal") * Time.deltaTime * panSpeed,
            0f);
    }
}
