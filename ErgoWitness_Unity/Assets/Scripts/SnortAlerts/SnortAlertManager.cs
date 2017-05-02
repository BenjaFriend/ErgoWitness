using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will handle what we do with new snort info.
/// Keep track of the largest number of hits for calculating the health
/// of the network.
/// 
/// Author: Ben Hoffman
/// </summary>
public class SnortAlertManager : MonoBehaviour {

    //public static SnortAlertManager alertManager;
    public Image alertPanel;
    public Text alertUIPrefab;

    private static Text[] alertUI;

    public static int[,] maxAlertCounts;   // A 2D int array that represents the max counts of alerts

    private int _topAlertCount;     // The computer with the most attack counts

    private void Start()
    {
        maxAlertCounts = new int[System.Enum.GetNames(typeof(AlertTypes)).Length, 1];

        alertUI = new Text[System.Enum.GetNames(typeof(AlertTypes)).Length];

        for(int i = 0; i <  alertUI.Length; i++)
        {
            alertUI[i] = Instantiate(alertUIPrefab);
            alertUI[i].transform.SetParent(alertPanel.transform);
            alertUI[i].text = maxAlertCounts[i, 0].ToString();
        }
    }

    /// <summary>
    /// Add an alert to this PC of this alert type
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="ipBeingAttacked"></param>
    public static void Alert(int ipBeingAttacked, AlertTypes alertType)
    {
        // Check the device manager for if we have this IP source or not

        // If we do NOT have this IP that is being attacked:
        if (!DeviceManager.currentDeviceManager.CheckDictionary(ipBeingAttacked))
        {
            Source newSource = new Source();
 
            // Set up the source data to properlly represent a computer that we don't yet have
            newSource.sourceIpInt = ipBeingAttacked;

            // Add them to the network, and wait for that to finish:
			DeviceManager.currentDeviceManager.NewComputer(newSource);
        }

        // Set the alert to that PC
        DeviceManager.ComputersDict[ipBeingAttacked].AddAlert(alertType);

        // Check if that is the most amount of alerts for this alert type
        if (DeviceManager.ComputersDict[ipBeingAttacked].AlertCount(alertType) > maxAlertCounts[(int)alertType, 0])
        {
            // Keep track of the highest number alert count we have to calculate the health
            maxAlertCounts[(int)alertType, 0] = DeviceManager.ComputersDict[ipBeingAttacked].AlertCount(alertType);

            // update the UI
            alertUI[(int)alertType].text = maxAlertCounts[(int)alertType, 0].ToString();

            // Tell all the computers to calculate their new health for this alert type

        }


    }

}
