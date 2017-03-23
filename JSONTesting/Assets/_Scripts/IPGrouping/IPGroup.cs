using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This represents a group of IP address that have the same
/// first 3 numbers. I.E. 192.168.1.XXX, all IP's with "192.168.1"
/// would be in this group
/// </summary>
[RequireComponent(typeof(Light))]
public class IPGroup : MonoBehaviour {

    [SerializeField]
    private bool isBlueTeam; // True if this IP is a blue team
    [SerializeField]
    private float increasePerComputer = 0.5f;
    [SerializeField]
    
    private float radius = 5f;
    private float startRadius;
    [SerializeField]
    private float minDistanceApart = 1f;
    [SerializeField]
    private float lightRangeScaler = 5f;
    [SerializeField]
    private float smoothing = 5f;

    private List<Computer> groupedComputers;

    private Material groupColor;        // The color of the group
    private int groupAddress;          // This is the IP address parsed into integers, with delimeters at the periods
    private string[] stringValues;      // Temp variable used to store an IP split at the '.'
    private int[] tempIntValues;        // Used for comparisons
    private Computer tempObj;         // Use to store a gameobject that I may need
    private int attemptCount;           // This is so that we can break out of finding a position if we try this many times

    private Vector3 temp;           // Store a temp positoin for calcuations
    private Collider[] neighbors;   // Store a temp array of colliders for calculations
    private Light myPointLight;
    private IEnumerator currentScalingRoutine;
    private Vector3 positionWithRadius;
    private bool isDying = false;

    public int GroupAddress { get { return groupAddress; } set { groupAddress = value; } }
    public Material GroupColor { get { return groupColor; } set { groupColor = value; } }
    public bool IsBlueTeam { get { return isBlueTeam; } set { isBlueTeam = value; } }

    /// <summary>
    /// Instantiate the list of grouped computers, 
    /// set the position of this
    /// </summary>
    private void Awake()
    {
        // Initialize the list
        groupedComputers = new List<Computer>();

        // Set the attempt count
        attemptCount = 0;
        myPointLight = GetComponent<Light>();
        myPointLight.range = radius * lightRangeScaler;

        startRadius = radius;

        isDying = false;
    }

    /// <summary>
    /// Using the given IP address we will add it to this group
    /// </summary>
    /// <param name="IpAddress"></param>
    public void AddToGroup(int IpAddress)
    {
        // Exit if the ip address is 0
        if (IpAddress == 0) return;

        // If our dictionary contains this...
        if (DeviceManager.currentDeviceManager.CheckDictionary(IpAddress))
        {
            // Cache the object here
            tempObj = DeviceManager.ComputersDict[IpAddress];

            // Make sure that we know that this is a blue team object if we are blue team
            if(isBlueTeam)
                tempObj.IsBlueTeam = true;

            // Add it to our list
            groupedComputers.Add(tempObj);

            // Increaset the radius of the group
            radius += increasePerComputer;

            // Move the object to our spot
            MoveToGroupSpot(tempObj);

            // Assign the the group color to this object
            SetColor(tempObj);

            // Increase the size of my light
            // if we are currently scalling the light, then stop
            if(currentScalingRoutine != null)
            {
                StopCoroutine(currentScalingRoutine);
            }
            // Start scaling with a new number!
            currentScalingRoutine = ScaleLightRange(radius * 2f);
            StartCoroutine(currentScalingRoutine);
        }
    }

    /// <summary>
    /// This method will move a gameobject to the group position
    /// </summary>
    private void MoveToGroupSpot(Computer thingToMove)
    {
        attemptCount++;

        // Make the this group the parent of the computer object
        thingToMove.transform.parent = gameObject.transform;

        // Calculate a random spot that is within a certain radius of our positon
        temp = transform.position + UnityEngine.Random.onUnitSphere * radius;

        // Check if I am colliding with any other groups
        neighbors = Physics.OverlapSphere(temp, minDistanceApart);

        // There is something colliding with us, recursively call this function
        if (neighbors.Length > 0 && attemptCount <= 3)
        {
            // Try again
            MoveToGroupSpot(thingToMove);
        }
        else
        {
            // Actually move the object to the position
            thingToMove.transform.position = temp;
        }
    }

    /// <summary>
    /// Assign the particle system of this computer
    /// to a color
    /// </summary>
    /// <param name="thingToChange"></param>
    private void SetColor(Computer thingToChange)
    {
        // Get the component that has the method to change the color for the particle system
        MeshRenderer temp = thingToChange.GetComponent<MeshRenderer>();

        // if this is null, then return
        if (temp == null)
            return;

        // Set the material to the group materials
        temp.material = groupColor;
    }

    /// <summary>
    /// Smoothly lerp the radius of this object
    /// </summary>
    /// <param name="newRange">The desired radius of this</param>
    /// <returns></returns>
    private IEnumerator ScaleLightRange(float newRange)
    {
        // While I am smaller then what I want to be
        while (myPointLight.range < newRange)
        {
            myPointLight.range = Mathf.Lerp(myPointLight.range, newRange, smoothing * Time.deltaTime);

            yield return null;
        }
    }

    public void RemoveIp(int removeMe)
    {
        // Remove this computer object from this group
        groupedComputers.Remove(DeviceManager.ComputersDict[removeMe]);

        radius -= increasePerComputer;
        if(radius <= startRadius)
        {
            radius = startRadius;

            // Start scaling with a new number!
            currentScalingRoutine = ScaleLightRange(radius * 2f);
            StartCoroutine(currentScalingRoutine);
        }
      
        // If we have nothing in our group and we are not already dying...
        if (groupedComputers.Count <= 0 && !isDying)
        {
            // Start the death cotoutine
            StartCoroutine(Die());
        }

    }

    /// <summary>
    /// This will shrink out light down, remove us from the group manager, and then
    /// destroy this gameobject
    /// </summary>
    /// <returns></returns>
    private IEnumerator Die()
    {
        isDying = true;

        // Wait for out light to hit 0
        currentScalingRoutine = ScaleLightRange(0f);

        // Wait until our light shrinks down
        
        StartCoroutine(currentScalingRoutine);
        yield return currentScalingRoutine;

        // Remove us from the group manager
        IPGroupManager.currentIpGroups.RemoveGroup(groupAddress);

        // Destroy this object
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

}
