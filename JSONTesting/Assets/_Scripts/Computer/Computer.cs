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
    public Source sourceInfo;            // The info that bro gives you
    public Material connectionColor;

    private float lifetime = 5f;       // How long until the computer will go off of the network
    private float timeSinceDiscovery = 0f;
    [SerializeField]    
    private float deathAnimTime = 0.5f; // The length of the death animation
    private Computer_AnimationController animationController;

    /// <summary>
    /// Use a list for this because it is better for insertion
    /// but the same for searching, there are only benefits to this
    /// </summary>
    private List<Computer> connectedPcs;

    private bool isBlueTeam;

	// This is to keep track of how many times we have seen this PC
	private int hits = 0;

    private bool isDying = false;   // This will be used to make sure that we don't call the death function when we don't need to
    private WaitForSeconds deathWait;
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

    void Start()
    {
        animationController = GetComponent<Computer_AnimationController>();
        deathWait = new WaitForSeconds(deathAnimTime);
    }

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

        // We are not dying anymore
        isDying = false;
    }


    /// <summary>
    /// Keep track of how active this node is, and if it has exceeded its lifetime
    /// then take it out of the dictionary
    /// </summary>
    private void Update()
    {
        // If we havce exceeded our active lifetime, and we are not on blue team...
        if(!isBlueTeam & timeSinceDiscovery >= lifetime && !isDying)
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
        // Make sure that we keep track of it being active
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

        // If the streamign assests is showing, then send it the updated information
        if(StreamingInfo_UI.currentStreamInfo.IsShowing && sourceInfo.sourceIpInt != 0)
            StreamingInfo_UI.currentStreamInfo.CheckTop(sourceInfo.id_orig_h, hits);

    }

    /// <summary>
    /// Disable this computer object because it has been inactive for long enough
    /// </summary>
    private void DisableMe()
    {
        // Get a reference to my group
        IPGroup myGroup = GetComponentInParent<IPGroup>();

        // As long as I am actually in a group...
        if(myGroup != null)
        {
            // Remove myself from that group
            myGroup.RemoveIp(sourceInfo.sourceIpInt);
        }

        // I do not want a parent anymore, so set it to null
        gameObject.transform.parent = null;

        // Remove myself from the dictoinary of computers that are active
        DeviceManager.ComputersDict.Remove(sourceInfo.sourceIpInt);

        // Call our death function if we are not already diein
        if (!isDying)
        {
            StartCoroutine(Die());
        }
    }

    /// <summary>
    /// This will wait for 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Die()
    {
        // We are currently dying, so make sure that we know that
        isDying = true;

        // Play the animation
        animationController.PlaySleepAnim();

        // Wait for the animation to finish
        yield return deathWait;

        // Once that is done the animation, set ourselves as inactive
        gameObject.SetActive(false);
    }

}
