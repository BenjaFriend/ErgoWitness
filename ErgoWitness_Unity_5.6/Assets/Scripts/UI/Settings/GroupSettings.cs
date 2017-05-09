using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This will allow the groups in the options menu to send the configuration
/// info to the group manager
/// 
/// Author: Ben Hoffman
/// </summary>
public class GroupSettings : MonoBehaviour {

    public int groupNum;
    public InputField[] inputFields;
    public Text groupName;

    private string groupPlayerPrefName;

    private void Start()
    {
        // Make a gruop name to use with the player prefs
        groupPlayerPrefName = "GROUP" + groupNum.ToString();

        // Get the input fields for us to use for ip address's
        inputFields = GetComponentsInChildren<InputField>();

        // Load in any previous inputs
        Load();
    }

    /// <summary>
    /// Save the input fields to the player prefs, so that we can 
    /// load them every time we load up the game.
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="infoArray"></param>
    private void Save(string[] infoArray)
    {
        // Loop throgh the given amount of inputs and set them in player prefs
        for(int i = 0; i < infoArray.Length; i++)
        {
            // Set the player prefs string for this object at this index
            PlayerPrefs.SetString(groupPlayerPrefName + i.ToString(), infoArray[i]);
        }
    }


    private void Load()
    {
        // load in whatever we need to for this group name
        for(int i = 0; i < inputFields.Length; i++)
        {
            // If a key exists for this group at this index, then load it
            if(PlayerPrefs.HasKey(groupPlayerPrefName + i.ToString()))
            {
                inputFields[i].text = PlayerPrefs.GetString(groupPlayerPrefName + i.ToString());
            }
        }

        // Apply the info that we have loaded
        ApplyInfo();
    }
	
    /// <summary>
    /// This method will send the info about the group ip address's
    /// to the group manager so that we know how to color them
    /// </summary>
    public void ApplyInfo()
    {
        string[] groups = GetCurrentGroups();

        // Send the information to the the group manager
        IPGroupManager.currentIpGroups.SetIpsViaOptions(groups, groupNum);

        // Save the group prefs
        Save(groups);
    }

    /// <summary>
    /// Get all the group info from the input fields, and set them as 
    /// </summary>
    /// <returns></returns>
    public string[] GetCurrentGroups()
    {
        // Send the infomation to the group manager
        List<string> ipAddresses = new List<string>();

        // Look for some ip address's that we can use
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].text != "") { }
            ipAddresses.Add(inputFields[i].text);
        }

        // Send that info to an array and return it
        return ipAddresses.ToArray();
    }

    /// <summary>
    /// Set the current groups from a string array
    /// </summary>
    /// <param name="newGroups"></param>
    public void SetCurrentGroups(string[] newGroups)
    {
        // Make sure that this index is valid index
        if(newGroups.Length >= inputFields.Length || newGroups == null)
        {
#if UNITY_EDITOR
            Debug.LogError("This group info is bad! \n" + this.name);
#endif
            return;
        }

        int i;
        
        for(i = 0; i < newGroups.Length; i++)
        {
            inputFields[i].text = newGroups[i];
        }
        // Clear the rest of the groups
        ClearGroups(i);

        // Apply the info
        ApplyInfo();

        // Save it
        Save(newGroups);
    }

    /// <summary>
    /// Clear the text of the groups past a certain index
    /// </summary>
    /// <param name="startIndex">What index we want to start at, so that we don't 
    /// have to clear multiple text fields if we are just setting them in a different place. </param>
    private void ClearGroups(int startIndex)
    {
        // Make sure that the index we want to use will not break anything
        if(startIndex < 0 || startIndex >= inputFields.Length)
        {
            return;
        }

        // Loop through the field componenets and clear the rest
        for(int i = startIndex; i < inputFields.Length; i++)
        {
            inputFields[i].text = "";
        }
    }

}
