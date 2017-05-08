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

    private SnortAlertManager snortManager;

    private int _fakeIPint = 10002;
    private int _fakeIP_2 = 20002;

    private void Start()
    {
        snortManager = GetComponent<SnortAlertManager>();
    }

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
            GenerateFalseAlert(_fakeIPint, AlertTypes.TypeA);
        }
        // If we press 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GenerateFalseAlert(_fakeIPint, AlertTypes.TypeB);
        }
        // If we press 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GenerateFalseAlert(_fakeIPint, AlertTypes.TypeC);
        }

        // If we press 8
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            GenerateFalseAlert(_fakeIP_2, AlertTypes.TypeA);
        }
        // If we press 9
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GenerateFalseAlert(_fakeIP_2, AlertTypes.TypeB);
        }
        // If we press 0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GenerateFalseAlert(_fakeIP_2, AlertTypes.TypeD);
        }

    }

    private void GenerateFalseAlert(int iptoUse, AlertTypes alertType)
    {
        // Send an alert to the snort manager with a false Ip address 
        snortManager.Alert(iptoUse, alertType);
    }

}
