using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// This script will handle what we do with new snort info.
/// Keep track of the largest number of hits for calculating the health
/// of the network.
/// 
/// Author: Ben Hoffman
/// </summary>
public class SnortAlertManager : MonoBehaviour {

    #region Fields
    
    public Image alertPanel;
    public Toggle togglePrefab;

    private Toggle[] alertToggles;

    public int[] maxAlertCounts;   // A 2D int array that represents the max counts of alerts, where the index is the type of attack

    #endregion

    private void Start()
    {
        maxAlertCounts = new int[System.Enum.GetNames(typeof(AlertTypes)).Length];

        alertToggles = new Toggle[System.Enum.GetNames(typeof(AlertTypes)).Length];

        // Create the UI panel that shows the max conuts of attack types, and the toggle options
        for (int i = 0; i < alertToggles.Length; i++)
        {
            alertToggles[i] = Instantiate(togglePrefab);
            int index = i;
            alertToggles[i].onValueChanged.AddListener(delegate { HideGroup(index); });

            alertToggles[i].transform.SetParent(alertPanel.transform);
            alertToggles[i].GetComponentInChildren<Text>().text =
                 System.Enum.GetName(typeof(AlertTypes), i);
        }
    }


    public void HideGroup(int value)
    {
        // Hides the group aspects
        StartCoroutine(DeviceManager.Instance.HideAlertType(value));
        // Hide it in the roups as well
        StartCoroutine(IPGroupManager.currentIpGroups.HideAlertType(value));
    }

    /// <summary>
    /// Add an alert to this PC of this alert type
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="ipBeingAttacked"></param>
    public void Alert(int ipBeingAttacked, AlertTypes alertType)
    {
        // If we do NOT have this IP that is being attacked:
        if (!DeviceManager.Instance.CheckDictionary(ipBeingAttacked))
        {
            // Create a new source based on the source IP that we have
            Source newSource = new Source();
 
            // Set up the source data to properlly represent a computer that we don't yet have
            newSource.sourceIpInt = ipBeingAttacked;

            // Add them to the network, and wait for that to finish:
			DeviceManager.Instance.NewComputer(newSource);

            return;
        }
        
        // Set the alert to that PC
        DeviceManager.ComputersDict[ipBeingAttacked].AddAlert(alertType);

        // Check if that is the most amount of alerts for this alert type
        if (DeviceManager.ComputersDict[ipBeingAttacked].AlertCount(alertType) > maxAlertCounts[(int)alertType])
        {
            // Keep track of the highest number alert count we have to calculate the health
            maxAlertCounts[(int)alertType] = DeviceManager.ComputersDict[ipBeingAttacked].AlertCount(alertType);

            // Tell all the computers to calculate their new health for this alert type
            DeviceManager.Instance.CalculateColors();           
        }
    }

    /// <summary>
    /// Returns if the given toggle is on or not
    /// </summary>
    /// <param name="index">What alert index we are checking</param>
    public bool CheckToggleOn(int index)
    {
        return alertToggles[index].isOn;
    }

    public static void ToggleAlertPanelsOff()
    {
        
    }



}
