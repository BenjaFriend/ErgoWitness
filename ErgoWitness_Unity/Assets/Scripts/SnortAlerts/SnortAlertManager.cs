using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will handle what we do with new snort info.
/// Keep track of the largest number of hits for calculating the health
/// of the network.
/// 
/// Author: Ben Hoffman
/// </summary>
public class SnortAlertManager : MonoBehaviour {

    public static SnortAlertManager alertManager;

    [Tooltip("What alerts are concerned about?")]
    public AlertTypes[] alertsTypes;

    private int _topAlertCount;     // The computer with the most attack counts

   /* private void Awake()
    {
        // Make sure tha thtis is the only one of these objects in the scene
        if (alertManager == null)
        {
            // Set the currenent referece
            alertManager = this;
        }
        else if (alertManager != this)
            Destroy(gameObject);
    }*/

    private void Start()
    {
        // Make sure tha thtis is the only one of these objects in the scene
        if (alertManager == null)
        {
            // Set the currenent referece
            alertManager = this;
        }
        else if (alertManager != this)
            Destroy(gameObject);
    }


    /// <summary>
    /// Add an alert to this PC of this alert type
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="ipBeingAttacked"></param>
    public void Alert(int ipBeingAttacked, string attackType)
    {
        // Check the device manager for if we have this IP source or not
        // If we do, then add an alert to that device of this type
        // If we do not, then add this device to the scene and then give it an alert
        
    }

}
