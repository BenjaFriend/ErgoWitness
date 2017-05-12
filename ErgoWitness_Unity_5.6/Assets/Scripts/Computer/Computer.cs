using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class holds the Data that this computer has, and a list
/// of computers that it is conenct to
/// 
/// Author: Ben Hoffman
/// </summary>
public class Computer : MonoBehaviour
{
    // A static count of all the computers in the scene
    public static int ComputerCount;

    #region Fields
    
    [Space]
    [Header("Snort Alert Interface")]
    [Tooltip("Determines if we are showing the health report or not")]
    public bool showHealthReport;       // Bool determining whether or not we should be showing the tiles that represent health
    public UnityEngine.UI.Image colorQuadPrefab;
	public Transform canvasTransform;
    
    [Tooltip("The snort manager for this computer")]
    public SnortAlertManager snortManager;

    private int sourceInt;

    [SerializeField]
    private float lifetime = 30f;            // How long until the computer will go off of the network
    private float timeSinceDiscovery = 0f;   // How long it has been since we were discovered
  
    private float deathAnimTime = 0.667F;                        // The length of the death animation
    private WaitForSeconds deathWait;  // How long we wait for our animation to play when we go inactive
    private Computer_AnimationController animationController;  // A reference to the animations for the computer object

    private bool isSpecialTeam;        // This is true if this object is of special interest to the user

    private bool isDying = false;      // This will be used to make sure that we don't call the death function when we don't need to
    private MeshRenderer meshRend;     // The mesh renderer component of this object so that we can

    private int[] alertCount;		   // [ A , B ] where A is an integer representing the Alert Type Enum cast as an int, and B is the count of that. 
    private float[] riskNumbers;       // Array in which the index is the integer representing the alert type enum cast as an int, and the value is the count of that

	
	private UnityEngine.UI.Image[] quadObjs;

    private float _currentHealth;

    private Color healthyColor = Color.green;
    private Color hurtColor = Color.red;

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

    #region Unity Methods
    void Awake()
    {
        // Get the mesh rend componenet
        meshRend = GetComponentInChildren<MeshRenderer>();      
		alertCount = new int[System.Enum.GetNames(typeof(AlertTypes)).Length];

        // Create two arrays based on the number of alert types in 
        quadObjs = new UnityEngine.UI.Image[System.Enum.GetNames(typeof(AlertTypes)).Length];

        riskNumbers = new float[System.Enum.GetNames(typeof(AlertTypes)).Length];

        // Get the particle system on this object to show the alerts on
        alertParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
		// Create a wait fro seconds object so taht we can avoid creating one every time
		deathWait = new WaitForSeconds(deathAnimTime);

		// Get the animation componenet
		animationController = GetComponent<Computer_AnimationController>();

        // If we have a snort alert manager...
        if (snortManager != null)
        {
            // Create the quad objects for our alert type
            for (int i = 0; i < quadObjs.Length; i++)
            {
                // Instantiate the object
                quadObjs[i] = Instantiate(colorQuadPrefab);
                // Set the partent of the image, so that it is a layout object in the UI
                quadObjs[i].transform.SetParent(canvasTransform);
                // Set the local positoin to 0 so that
                quadObjs[i].rectTransform.localPosition = Vector3.zero;
                // Set the starting color to green
                quadObjs[i].color = healthyColor;
            }

            // Set the colors of everything 
            CalculateAllAlerts();

            canvasTransform.gameObject.SetActive(false);
        }

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

        // Increment the count of computers
        ComputerCount++;
 
    }

    private void OnDisable()
    {
        // This computer is no longer active, so decrement the static field
        ComputerCount--;
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
        else if(!NetflowPauseController.IsPaused)
        {
            // Add how long it has been to the field
            timeSinceDiscovery += Time.deltaTime;
        }
    }

    #endregion

    /// <summary>
    /// Add one to the index of the attack type
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="attackType"></param>
    public void AddAlert(AlertTypes attackType)
    {
        if(snortManager == null)
        {
            return;
        }

        // Cast the alert type and store it
        int alertInt = (int)attackType;

        // Add to the count of this alert type
        alertCount[alertInt]++;

        // Calculate the percentage of health on this node based on the
        riskNumbers[alertInt] = 
            (float)alertCount[alertInt] / (float)(snortManager.maxAlertCounts[alertInt] + 1);

        // If we are showign the health report and this quad specifically,
        if (showHealthReport && quadObjs[alertInt].isActiveAndEnabled)
        {
            // Set the color of the quad object
            quadObjs[alertInt].color = Color.Lerp(healthyColor, hurtColor, riskNumbers[alertInt]);
        }
        
        // Tell our group this
        GetComponentInParent<IPGroup>().AddAlert(alertInt, alertCount[alertInt]);
    }

    /// <summary>
    /// Calculate the average health based off of the risk number array.
    /// 
    /// Author: Ben Hoffman
    /// </summary>
	public void CalculateAllAlerts()
	{
        if (snortManager == null)
        {
            return;
        }

        // Reset the current health to 0
        _currentHealth = 0f;

        // Loop through the risk numbers and calculate them based on the max alert count
		for (int i = 0; i < riskNumbers.Length; i++) 
		{
            // IF this index is active in the snort manager, then account for it.
            if (snortManager.CheckToggleOn(i))
            {
                // Calculate the health
                riskNumbers[i] =  (float)alertCount[i] / ((float)snortManager.maxAlertCounts[i] + 1);
            
                // Set the color of that quad object
                quadObjs[i].color = Color.Lerp(healthyColor, hurtColor, riskNumbers[i]);

                // Add to the current health
                _currentHealth += riskNumbers[i];
            }
		}
        
        // Average the health numbers
        _currentHealth /= riskNumbers.Length;

        // Set the color of the mesh
        meshRend.material.color = Color.Lerp(healthyColor, hurtColor, _currentHealth);

        // If the average health of the network is above 0.6 un-healthy
        if (_currentHealth >= 0.6f)
        {
            // Start the alert ping
            alertParticleSystem.Play();
        }
        else if (alertParticleSystem != null)
        {
            // Stop the particle system
            alertParticleSystem.Stop();
        }
    }

    /// <summary>
    /// Get the alert count of this alert type on this object
    /// </summary>
    /// <param name="attackType">The type of alert we are checking</param>
    /// <returns>How many of these alerts have occured on this object</returns>
    public int AlertCount(AlertTypes attackType)
    {
        if (snortManager == null)
        {
            return -1;
        }

        return alertCount[(int)attackType];
    }

    /// <summary>
    /// Set the showing health report bool and the canvas with the health
    /// report on it to the opposite of their current state
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    public void ToggleAlertMessages()
    {
        if (snortManager == null)
        {
            return;
        }

        // Invert the bool indicating whether or not we ar showing the health report
        showHealthReport = !showHealthReport;

        // Invert whether or not this game object is active
        canvasTransform.gameObject.SetActive(!canvasTransform.gameObject.activeInHierarchy);
    }

    /// <summary>
    /// Toggles if we are showing this specific type of attack in our Ui
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="attackType"></param>
    public void ToggleAttackType(int attackType)
    {
        if (snortManager == null)
        {
            return;
        }

        // Set the object's active to the opposite of what it currently is
        quadObjs[attackType].gameObject.SetActive(!quadObjs[attackType].gameObject.activeInHierarchy);
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
        // Remove this object from the IP gorup it is in
        GetComponentInParent<IPGroup>().RemoveIp(sourceInt);

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
