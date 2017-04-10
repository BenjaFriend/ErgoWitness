using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSettings : MonoBehaviour {


    public int groupNum;
    public UnityEngine.UI.InputField[] inputFields;
    public Text groupName;

    private void Start()
    {
        inputFields = GetComponentsInChildren<UnityEngine.UI.InputField>();
    }
	
    /// <summary>
    /// This method will send the info about the group ip address's
    /// to the group manager so that we know how to color them
    /// </summary>
    public void ApplyInfo()
    {
        // Send the infomation to the group manager

    }

}
