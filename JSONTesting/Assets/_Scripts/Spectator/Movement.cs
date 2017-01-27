using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// purpose of class: To make a camera movement method
/// that will allow the spectator to fly through the space 
/// and look at each node ('computer') and their connections
/// </summary>
public class Movement : MonoBehaviour {

    #region Fields

    public float xMoveSpeed;
    public float yMoveSpeed;
    public float panSpeed;

    #endregion
	
	/// <summary>
    /// Check for input from the user and call the functions to calculate the movement
    /// </summary>
	void Update ()
    {
        Move();
	}

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To do the movement on this game object
    /// </summary>
    void Move()
    {
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * xMoveSpeed * Time.deltaTime);
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * xMoveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Q))
        {
            // Move up
            transform.Translate(transform.up * yMoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            // Move down
            transform.Translate(-transform.up * yMoveSpeed * Time.deltaTime);
        }
        // Clamp the position
    }
}
