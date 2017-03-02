using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This represents a group of IP address that have the same
/// first 3 numbers. I.E. 192.168.1.XXX, all IP's with "192.168.1"
/// would be in this group
/// </summary>
public class IPGroup : MonoBehaviour {

    public Material groupColor;        // The color of the group
    public float increasePerComputer = 0.5f;
    public float radius = 5f;
    public float minDistanceApart = 1f;

    public List<Computer> groupedComputers;

    public int groupAddress;          // This is the IP address parsed into integers, with delimeters at the periods
    private string[] stringValues;      // Temp variable used to store an IP split at the '.'
    private int[] tempIntValues;        // Used for comparisons
    private Computer tempObj;         // Use to store a gameobject that I may need
    private int attemptCount;           // This is so that we can break out of finding a position if we try this many times

    private Vector3 temp;           // Store a temp positoin for calcuations
    private Collider[] neighbors;   // Store a temp array of colliders for calculations
    public int GroupAddress { get { return groupAddress; } set { groupAddress = value; } }


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
        if (GameController.currentGameController.CheckDictionary(IpAddress))
        {
            // Cache the object here
            tempObj = GameController.ComputersDict[IpAddress];

            // Add it to our list
            groupedComputers.Add(tempObj);

            // Increaset the radius of the group
            radius += increasePerComputer;

            // Move the object to our spot
            MoveToGroupSpot(tempObj);

            // Assign the the group color to this object
            SetColor(tempObj);
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

        // I need to put it in a random spot...
        temp = new Vector3(
            UnityEngine.Random.Range(transform.position.x - radius, transform.position.x + radius),
            UnityEngine.Random.Range(transform.position.y - radius, transform.position.y + radius),
            UnityEngine.Random.Range(transform.position.z - radius, transform.position.z + radius));

        // Check if I am colliding with any other groups
        neighbors = Physics.OverlapSphere(temp, minDistanceApart);

        // There is something colliding with us, recursively call this function
        if (neighbors.Length > 0 && attemptCount <= 10)
        {
            // Try again
            MoveToGroupSpot(thingToMove);
        }
        else
        {
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
}
