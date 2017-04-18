using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will save your current configuration that is 
/// in the options menu, or also load in the settings that the user has saved last time
/// 
/// Author: Ben Hoffman
/// </summary>
public class OptionsSaving : MonoBehaviour {

    // Error message text for if we don't have a previous config
    private string configFileLocation = "Defaults/settingsconf";
    // reference to the ip string
    // reference to the group settings
    public string serverIP;
    public string[] group1;
    public string[] group2;

	void Start ()
    {
        // Try to load in the settings from last time, if there is one
        LoadSettings();
	}
	

    /// <summary>
    /// Try and load in the settings from the resources folder
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    public void LoadSettings()
    {
        // Load in the settings that were given to us from a string, and overwrite this class.
        JsonUtility.FromJsonOverwrite(Resources.Load(configFileLocation).ToString(), this);

        // Actually set the text fields for the UI that we need to

        // Apply those changes
    }

    /// <summary>
    /// Save the current settings to a JSON file that can be loaded
    /// in next time.
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    public string SaveSettings()
    {
        // Get the server IP and set it equal to our field

        // Get the group IP's and set them equal to our fields

        // Return this class in json
        return JsonUtility.ToJson(this);
    }



}
