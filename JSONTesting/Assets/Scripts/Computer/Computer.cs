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
    public Source sourceInfo;                // The info that bro gives you
    [SerializeField]
    private float lifetime = 30f;            // How long until the computer will go off of the network
    private float timeSinceDiscovery = 0f;   // How long it has been since we were discovered

    [SerializeField]    
    private float deathAnimTime = 0.5f;                        // The length of the death animation
    private Computer_AnimationController animationController;  // A reference to the animations for the computer object
    private AudioSource audioSource;

    /// <summary>
    /// Use a list for this because it is better for insertion
    /// but the same for searching, there are only benefits to this
    /// </summary>
    private LinkedList<Computer> connectedPcs;

    private bool isSpecialTeam;     // This is true if this object is of special interest to the user

	// This is to keep track of how many times we have seen this PC
	private int hits = 0;

    private bool isDying = false;      // This will be used to make sure that we don't call the death function when we don't need to
    private WaitForSeconds deathWait;  // How long we wait for our animation to play when we go inactive
    private MeshRenderer meshRend;     // The mesh renderer component of this object so taht we can 
    private IPGroup myGroup;           // A reference to the IP group that I am in
    #endregion

    #region Mutators

    public bool IsSpecialTeam
    { get { return isSpecialTeam;  }
        set { isSpecialTeam = value; } }

    public Source SourceInfo {
        get { return sourceInfo; }

        set
        {
            sourceInfo = value;
        }
    }
    #endregion

    void Awake()
    {
        // Get the animation componenet
        animationController = GetComponent<Computer_AnimationController>();

        // Create a wait fro seconds object so taht we can avoid creating one every time
        deathWait = new WaitForSeconds(deathAnimTime);

        // Get the mesh rend componenet
        meshRend = GetComponentInChildren<MeshRenderer>();

        // Initialize a new list object
        connectedPcs = new LinkedList<Computer>();

        // Get the audio source compoenent
        audioSource = GetComponent<AudioSource>();
    }


    private void OnEnable()
    {
        // Make sure tha we know that the time since my discover is reset
        timeSinceDiscovery = 0f;

        // If this object is on a team that we care extra about...
        if (isSpecialTeam)
        {
            // Make it's lifetime longer
            lifetime *= 3;
        }

        // We are not dying anymore
        isDying = false;
    }

    /// <summary>
    /// Set the current mesh renderer's material to this new material.
    /// Also set the group reference on this object to the 
    /// </summary>
    /// <param name="groupMat">The group material</param>
    public void SetUpGroup(Material groupMat, IPGroup myNewGroup)
    {
        // Set the 
        meshRend.material = groupMat;

        // Get the reference to a group
        myGroup = myNewGroup;

        // Play my audio now that I am moved to my group
        audioSource.Play();
    }

    /// <summary>
    /// Keep track of how active this node is, and if it has exceeded its lifetime
    /// then take it out of the dictionary
    /// </summary>
    private void Update()
    {
        // If we havce exceeded our active lifetime, and we are not on blue team...
        if (!isSpecialTeam & timeSinceDiscovery >= lifetime && !isDying)
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

        // TODO: Find out if I actually really need this, i think not.
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
            connectedPcs.AddLast(connectedToMe);
        }

		// Tell the streaming information that we got another hit on this IP
		hits++;

        // If the streamign assests is showing, then send it the updated information
        if(StreamingInfo_UI.currentStreamInfo.IsShowing)
            StreamingInfo_UI.currentStreamInfo.CheckTop(sourceInfo.id_orig_h, hits);

    }

    /// <summary>
    /// Disable this computer object because it has been inactive for long enough
    /// </summary>
    private void DisableMe()
    {
        // As long as I am actually in a group...
        if(myGroup != null)
        {
            // Remove myself from that group
            myGroup.RemoveIp(sourceInfo.sourceIpInt);

            // Remove the reference to my group
            myGroup = null;
        }

        // Clear my connected computers 
        connectedPcs.Clear();

        // I do not want a parent anymore, so set it to null
        gameObject.transform.parent = null;

        // Remove myself from the dictoinary of computers that are active
        DeviceManager.ComputersDict.Remove(sourceInfo.sourceIpInt);

        // Call our death function if we are not already diein
        if (!isDying)
        {
            // Start the die coroutine
            StartCoroutine(Die());
        }
    }

    /// <summary>
    /// This will wait for the death animation to finish before actually killing it
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
