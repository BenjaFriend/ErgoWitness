using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// This class holds the Data that this computer has, and a list
/// of computers that it is conenct to
/// </summary>
public class Computer : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private float lifetime = 5f;       // How long until the computer will go off of the network
    private float timeSinceDiscovery = 0f;

    public Source sourceInfo;            // The info that bro gives you
    public Material connectionColor;
    /// <summary>
    /// Use a list for this because it is better for insertion
    /// but the same for searching, there are only benefits to this
    /// </summary>
    public List<Computer> connectedPcs;

    [SerializeField]
    private bool isBlueTeam;

	// This is to keep track of how many times we have seen this PC
	private int hits = 0;
    #endregion

    #region Mutators

    public bool IsBlueTeam { get { return isBlueTeam;  }
        set { isBlueTeam = value; } }

    public Source SourceInfo {
        get { return sourceInfo; }

        set
        {
            sourceInfo = value;
        }
    }
    
    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Set up the components that I need
    /// </summary>
    void OnEnable()
    {
        // Initialize a new list object
        connectedPcs = new List<Computer>();
        
        // Make sure tha we know that the time since my discover is reset
        timeSinceDiscovery = 0f;

        if (isBlueTeam)
        {
            lifetime *= 3;
        }
    }

    /// <summary>
    /// Disable this computer object because it has been inactive for long enough
    /// </summary>
    private void DisableMe()
    {
        // Check if the group is empty, if it is then disable the group
        IPGroupManager.currentIpGroups.RemoveIpFromGroup(this);

        // Remove myself from the dictoinary
        DeviceManager.ComputersDict.Remove(sourceInfo.sourceIpInt);

        // Set myself to inactive in the heigharchy, so that I be used again
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Keep track of how active this node is, and if it has exceeded its lifetime
    /// then take it out of the dictionary
    /// </summary>
    private void Update()
    {
        // If we havce exceeded our active lifetime, and we are not on blue team...
        if(!isBlueTeam & timeSinceDiscovery >= lifetime)
        {
            // Remove it from the dictionary
            DisableMe();
        }
        else
        {
            // Add how long it has been to the field
            timeSinceDiscovery += Time.deltaTime;
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To add the given computer to my
    /// list of connected PC's
    /// </summary>
    /// <param name="connectedToMe">the PC that is connected to me</param>
    public void AddConnectedPC(Computer connectedToMe)
    {
        timeSinceDiscovery = 0f;
        // If the object that we are given is null or it is this game object:
        if (connectedToMe == null || connectedToMe == gameObject)
        {
            // Return out of this method
            return;
        }

        // If this device is NOT in my list already:
        if (!connectedPcs.Contains(connectedToMe))
        {
            // Add the connection to my linked list
            connectedPcs.Add(connectedToMe);
        }
		// Tell the streaming information that we got another hit on this IP
		hits++;

        if(StreamingInfo_UI.currentStreamInfo.IsShowing && sourceInfo.sourceIpInt != 0)
            StreamingInfo_UI.currentStreamInfo.CheckTop(sourceInfo.id_orig_h, hits);

    }

}
