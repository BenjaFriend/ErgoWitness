using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: Generate alerts based on keyboard input so that we can
/// build the snort alert manager to. In production this would be
/// replaced with the the monitor object that is querying the database
/// for snort alerts. 
/// </summary>
public class GenerateFalseAlerts : MonoBehaviour {
	
	/// <summary>
    /// Check for input from the keyboard, and generate alerts if we 
    /// press the number keys of the alert type
    /// 
    /// Author: Ben Hoffman
    /// </summary>
	void Update ()
    {
        // If we press 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

        }
        // If we press 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

        }
        // If we press 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
        // If we press 4
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {

        }
    }

    private static void GenerateFalseAlert(int alertType)
    {
        // Send an alert to the snort manager with a false Ip address 
    }

}
