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

    private Color healthyColor = Color.green;
    private Color hurtColor = Color.red;

    private int sourceInt;

    [SerializeField]
    private float lifetime = 30f;            // How long until the computer will go off of the network
    private float timeSinceDiscovery = 0f;   // How long it has been since we were discovered

    [SerializeField]    
    private float deathAnimTime = 0.5f;                        // The length of the death animation
    private Computer_AnimationController animationController;  // A reference to the animations for the computer object

    private bool isSpecialTeam;        // This is true if this object is of special interest to the user

    private bool isDying = false;      // This will be used to make sure that we don't call the death function when we don't need to
    private WaitForSeconds deathWait;  // How long we wait for our animation to play when we go inactive
    private MeshRenderer meshRend;     // The mesh renderer component of this object so that we can

    private IPGroup myGroup;           // A reference to the IP group that I am in


    private int[] alertCount;			// [ A , B ] where A is an integer representing the Alert Type Enum cast as an int, and B is the count of that. 
    public float[] riskNumbers;			// Array in which the index is the integer representing the alert type enum cast as an int, and the value is the count of that


	public UnityEngine.UI.Image colorQuadPrefab;
	public Transform canvasTransform;
	private UnityEngine.UI.Image[] quadObjs;


	private ParticleSystem alertParticleSystem;

    

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
        // Get the mesh rend componenet
        meshRend = GetComponentInChildren<MeshRenderer>();      
		alertCount = new int[System.Enum.GetNames(typeof(AlertTypes)).Length - 1];

		// Get the particle system on this object to show the alerts on
		alertParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
		// Create a wait fro seconds object so taht we can avoid creating one every time
		deathWait = new WaitForSeconds(deathAnimTime);

		// Get the animation componenet
		animationController = GetComponent<Computer_AnimationController>();

		riskNumbers = new float[System.Enum.GetNames(typeof(AlertTypes)).Length];


		quadObjs = new UnityEngine.UI.Image[System.Enum.GetNames (typeof(AlertTypes)).Length - 1];

		for (int i = 0; i < quadObjs.Length; i++) 
		{
			quadObjs [i] = Instantiate (colorQuadPrefab);
			quadObjs [i].transform.SetParent (canvasTransform);
			quadObjs [i].rectTransform.localPosition = Vector3.zero;
		}
    }

    /// <summary>
    /// Add one to the index of the attack type
	/// 
	/// Author: Ben Hoffman
    /// </summary>
    /// <param name="attackType"></param>
    public void AddAlert(AlertTypes attackType)
    {
        int alertInt = (int)attackType;

        alertCount[alertInt]++;

        // Calculate the percentage of health on this node based on the 
		riskNumbers[alertInt] =  (float)alertCount[alertInt] / (float)SnortAlertManager.maxAlertCounts[alertInt, 0] + 1;
        

		// Set the color 
		SetColor(alertInt);

		// If the alert value is less then the specified point at which the user wants to be alerted at...
		if (riskNumbers [alertInt] >= 0.6f) 
		{
			// Start the alert ping
			alertParticleSystem.Play();
		}
		else if(alertParticleSystem != null)
		{
			alertParticleSystem.Stop();
		}

		// If we are above the value of 0.5, then ping the alert particle system here

    }

	public void CalculateAllAlerts()
	{
		for (int i = 0; i < riskNumbers.Length; i++) 
		{
			// Calculate the health
			riskNumbers[i] =  (float)alertCount[i] / (float)SnortAlertManager.maxAlertCounts[i, 0] + 1;
			// Set the color
			SetColor (i);
		}
	}

    public void SetColor(int alertInt)
    {
		meshRend.material.color = Color.Lerp(healthyColor, hurtColor, riskNumbers[alertInt]);
		quadObjs[alertInt].color = Color.Lerp(healthyColor, hurtColor, riskNumbers[alertInt]);
    }

    /// <summary>
    /// Get the alert count of this alert type on this object
    /// </summary>
    /// <param name="attackType">The type of alert we are checking</param>
    /// <returns>How many of these alerts have occured on this object</returns>
    public int AlertCount(AlertTypes attackType)
    {
        return alertCount[(int)attackType];
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
    public void ResetLifetime()
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
