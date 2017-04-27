using System;
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

    private int sourceInt;

    [SerializeField]
    private float lifetime = 30f;            // How long until the computer will go off of the network
    private float timeSinceDiscovery = 0f;   // How long it has been since we were discovered

    [SerializeField]    
    private float deathAnimTime = 0.5f;                        // The length of the death animation
    private Computer_AnimationController animationController;  // A reference to the animations for the computer object

    private bool isSpecialTeam;     // This is true if this object is of special interest to the user

    private bool isDying = false;      // This will be used to make sure that we don't call the death function when we don't need to
    private WaitForSeconds deathWait;  // How long we wait for our animation to play when we go inactive
    private MeshRenderer meshRend;     // The mesh renderer component of this object so taht we can 

    private IPGroup myGroup;           // A reference to the IP group that I am in

    private int[,] alertCount;

    #endregion


    #region Mutators

    public bool IsSpecialTeam
    { get { return isSpecialTeam; }
        set { isSpecialTeam = value; } }

    public int SourceInt
    {
        get { return sourceInt; }
        set { sourceInt = value; }
    }

    #endregion


    #region Methods

    void Awake()
    {
        // Get the animation componenet
        animationController = GetComponent<Computer_AnimationController>();

        // Create a wait fro seconds object so taht we can avoid creating one every time
        deathWait = new WaitForSeconds(deathAnimTime);

        // Get the mesh rend componenet
        meshRend = GetComponentInChildren<MeshRenderer>();
        
    }


    private void Start()
    {
        alertCount = new int[SnortAlertManager.alertManager.alertsTypes.Length, 1];
    }

    /// <summary>
    /// Add one to the index of the attack type
    /// </summary>
    /// <param name="attackType"></param>
    public void AddAlert(int attackType)
    {

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
    }

    /// <summary>
    /// This will reset the lifetime of this computer because it was
    /// seen again, and we want to mark is as active
    /// </summary>
    public void AddHit()
    {
        // Reset the lifetime of this computer
        timeSinceDiscovery = 0f;
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
            myGroup.RemoveIp(sourceInt);
            // Remove the reference to my group
            myGroup = null;
        }

        // I do not want a parent anymore, so set it to null
        gameObject.transform.parent = null;

        // Remove myself from the dictoinary of computers that are active
        DeviceManager.ComputersDict.Remove(sourceInt);

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

    #endregion

}
