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
    public UnityEngine.UI.InputField[] inputFields;
    public Text groupName;

    private void Start()
    {
        // Get the input fields for us to use for ip address's
        inputFields = GetComponentsInChildren<InputField>();
    }
	
    /// <summary>
    /// This method will send the info about the group ip address's
    /// to the group manager so that we know how to color them
    /// </summary>
    public void ApplyInfo()
    {
        // Send the infomation to the group manager
        List<string> ipAddresses = new List<string>();
        
        // Look for some ip address's that we can use
        for(int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].text != "") { }
            ipAddresses.Add(inputFields[i].text);
        }

        // Send the information to the the group manager
        IPGroupManager.currentIpGroups.SetIpsViaOptions(ipAddresses.ToArray(), groupNum);
    }

}
