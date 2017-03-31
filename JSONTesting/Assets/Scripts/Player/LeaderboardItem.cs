using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will have a reference to the UI elements of this object
/// and have a method to set each of them
/// </summary>
[RequireComponent(typeof(Text))]
public class LeaderboardItem : MonoBehaviour {

    private Text ipText;        // The text component of the IP address
    private Text countText;     // The text componenet of the count text

    /// <summary>
    /// Set up the references to the text components
    /// </summary>
    private void Start()
    {
        // Get all text components on this object
        Text[] textComponents = GetComponentsInChildren<Text>();
        for (int i = 0; i < textComponents.Length; i++)
        {
            // If we don't have the first IP component yet, then set it equal to this
            if (ipText == null)
            {
                ipText = textComponents[i];
            }

            // If this is the second component, then set it equal to the count text
            if (textComponents[i].GetInstanceID() != ipText.GetInstanceID())
            {
                countText = textComponents[i];
                break;
            }
        }

        // Set their text fields to empty to start as long as they are not null
        try
        {
            // Set the text components
            ipText.text = "";
            countText.text = "";
        }
        catch { }
    }

    /// <summary>
    /// Sets the text of the UI elements
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="count"></param>
    public void SetText(string ip, int count)
    {
        try
        {
            ipText.text = ip;
            countText.text = count.ToString();
        }
        catch { }
    }

    /// <summary>
    /// Allow me to clear the text fields of this objectt
    /// </summary>
    public void ClearText()
    {
        // Set the text components
        ipText.text = "";
        countText.text = "";
    }

}
