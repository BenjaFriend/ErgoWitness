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

    public static int IPGROUP_COUNT;

    #region Fields

    private float deathWaitTime = 2f;
    private bool isSpecialTeam; // True if this IP is a blue team
    private float increasePerComputer = 0.1f;

    private float radius = 2f;
    private float startRadius;

    private float minDistanceApart = 0.1f;

    private float lightRangeScaler = 2.3f;    // How much larger is the light range then the 
    private float smoothing = 10f;      // How fast the light will transition
            
    private List<Computer> groupedComputers;

    private Color groupColor;        // The color of the group
    private int groupAddress;          // This is the IP address parsed into integers, with delimeters at the periods
    private Computer tempObj;         // Use to store a gameobject that I may need
    private int attemptCount;           // This is so that we can break out of finding a position if we try this many times

    private Vector3 temp;           // Store a temp positoin for calcuations
    private Collider[] neighbors;   // Store a temp array of colliders for calculations
    private Light myPointLight;

    private Coroutine currentScalingRoutine;

    private Vector3 positionWithRadius;
    private bool isDying = false;



    #endregion


    #region Health Monitor Fields

    private int[] maxAlertCountForGroup;		   // [ A , B ] where A is an integer representing the Alert Type Enum cast as an int, and B is the count of that. 

    public UnityEngine.UI.Image colorQuadPrefab;
    public Transform canvasTransform;
    private UnityEngine.UI.Image[] quadObjs;


    private Color healthyColor = Color.green;
    private Color hurtColor = Color.red;

    #endregion


    #region Mutators

    public int GroupAddress { get { return groupAddress; } set { groupAddress = value; } }
    public Color GroupColor { get { return groupColor; } set { groupColor = value; myPointLight.color = value; } }
    public bool IsSpecialTeam { get { return isSpecialTeam; } set { isSpecialTeam = value; } }
    public bool IsDying { get { return isDying; } }

    #endregion


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

    private void Start()
    {
        maxAlertCountForGroup = new int[System.Enum.GetNames(typeof(AlertTypes)).Length];

        if(canvasTransform != null)
        {
            // Create two arrays based on the number of alert types in 
            quadObjs = new UnityEngine.UI.Image[System.Enum.GetNames(typeof(AlertTypes)).Length];

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
        }

      
    }

    /// <summary>
    /// Using the given IP address we will add it to this group
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="IpAddress"></param>
    public void AddToGroup(int IpAddress)
    {
        // If our dictionary contains this...
        if (DeviceManager.Instance.CheckDictionary(IpAddress))
        {
            // Cache the object here
            tempObj = DeviceManager.ComputersDict[IpAddress];

            // Make sure that we know that this is a blue team object if we are blue team
            if(isSpecialTeam)
                tempObj.IsSpecialTeam = true;

            // Add it to our list
            groupedComputers.Add(tempObj);

            // Increaset the radius of the group
            radius += increasePerComputer;

            // Move the object to our spot
            MoveToGroupSpot(tempObj.transform);

            // Set the object's material to the group color, and give it a reference to this as it's group

            // Set the parent obect of the computer to this IP group
            tempObj.transform.SetParent(this.transform);

            // Increase the size of my light
            // if we are currently scalling the light, then stop
            if (currentScalingRoutine != null)
            {
                StopCoroutine(currentScalingRoutine);
            }

            // Start scaling with a new number! use Radis * 2 becuase it is a radius, and we want to create a sphere
            currentScalingRoutine = StartCoroutine(ScaleLightRange(radius * 2f, smoothing));
        }
    }

    /// <summary>
    /// This method will move a gameobject to the group position
    /// </summary>
    private void MoveToGroupSpot(Transform thingToMove)
    {
        attemptCount++;

        // Make the this group the parent of the computer object
        transform.parent = gameObject.transform;

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
    /// Remove a specific IP address from this group.
    /// If this group then has nothing in it, then destroy it.
    /// </summary>
    /// <param name="removeMe">The IP integer of the object that we want to remove</param>
    public void RemoveIp(int removeMe)
    {
        // Remove this computer object from this group
        groupedComputers.Remove(DeviceManager.ComputersDict[removeMe]);

        // Reduce the radius of the group
        radius -= increasePerComputer;

        // If the radius is less then the starting radius, then set it back to the start
        if(radius <= startRadius)
        {
            // Set the radius to the original radius
            radius = startRadius;
        }

        // Start scaling with a new number!
        currentScalingRoutine = StartCoroutine(ScaleLightRange(radius * 2f, smoothing));


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
        // We are dying now
        isDying = true;

        // Remove us from the group manager
        IPGroupManager.currentIpGroups.RemoveGroup(groupAddress);

        // If we are currently scaling, then stop
        if (currentScalingRoutine != null)
        {
            StopCoroutine(currentScalingRoutine);
        }

        // Wait for out light to hit 0
        currentScalingRoutine = StartCoroutine(ScaleLightRange(0f, 0.1f));
        
        // Wait for the light to go away
        yield return new WaitForSeconds(deathWaitTime);
    
        // Destroy this object
        Destroy(gameObject);
    }

    /// <summary>
    /// Smoothly lerp the radius of this object
    /// </summary>
    /// <param name="newRange">The desired radius of this</param>
    /// <returns></returns>
    private IEnumerator ScaleLightRange(float newRange, float smoothingAmount)
    {
        // If this range is what we have now then do nothing
        if(newRange == myPointLight.range)
        {
            yield break;
        }
        // If this range is Greater then what we have now, then scale up
        else if(newRange > myPointLight.range)
        {
            // While I am smaller then what I want to be
            while (myPointLight.range < newRange)
            {
                // Change the range value of this point ligtht
                myPointLight.range = Mathf.Lerp(myPointLight.range, newRange, smoothingAmount);

                // Yield for the end of the frame without generating garbage
                yield return null;
            }
        }
        // If this range is LESS then what we have now, then scale down
        else if(newRange < myPointLight.range)
        {
            // While I am smaller then what I want to be
            while (myPointLight.range > newRange)
            {
                // Change the range value of this point ligtht
                myPointLight.range = Mathf.Lerp(myPointLight.range, newRange, smoothingAmount);

                // Yield for the end of the frame without generating garbage
                yield return null;
            }
        }

        
    }


    private void OnEnable()
    {
        IPGROUP_COUNT++;
    }

    private void OnDisable()
    {
        IPGROUP_COUNT--;
    }

    /// <summary>
    /// To be called when one of the computer's in th network get's 
    /// and alert on them. This will calculate Health of our group.
    /// What is the health of the group exactly?
    /// 
    /// the mesh color of the computer's will show their average health...
    /// 
    /// There will be the attack types, but the max that is in each group. 
    /// Then use that to calculate the 
    /// 
    /// </summary>
    /// <param name="alert"></param>
    public void AddAlert(int alertIndex, int count)
    {
        // If this count is greater then the count that we currently have:
        if(count > maxAlertCountForGroup[alertIndex])
        {
            // set the max count for this IP group to that
            maxAlertCountForGroup[alertIndex] = count;

            // Calculate the health color based on that
            quadObjs[alertIndex].color = 
                Color.Lerp(
                healthyColor,
                hurtColor,
                (float)maxAlertCountForGroup[alertIndex] / (float)(DeviceManager.Instance.snortManager.maxAlertCounts[alertIndex] + 1));
        }
    }


    /// <summary>
    /// Toggles if we are showing this specific type of attack in our Ui
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="attackType"></param>
    public void ToggleAttackType(int attackType)
    {
        // If we don't have a snort manager, then return out of this method
        if (DeviceManager.Instance.snortManager == null)
        {
            return;
        }

        // Set the object's active to the opposite of what it currently is
        quadObjs[attackType].gameObject.SetActive(!quadObjs[attackType].gameObject.activeInHierarchy);
    }

}
