using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This represents a group of IP address that have the same
/// first 3 numbers. I.E. 192.168.1.XXX, all IP's with "192.168.1"
/// would be in this group
/// </summary>
public class IPGroup : MonoBehaviour {

    public Color groupColor;        // The color of the group

    public float radius = 5f;
    public float minDistanceApart = 1f;

    public List<GameObject> groupedComputers;

    public int[] groupAddress;     // This is the IP address parsed into integers, with delimeters at the periods
    private string[] stringValues;      // Temp variable used to store an IP split at the '.'
    private int[] tempIntValues;        // Used for comparisons
    private GameObject tempObj;         // Use to store a gameobject that I may need
    private int attemptCount;           // This is so that we can break out of finding a position if we try this many times
    public int[] GroupAddress { get { return groupAddress; } set { groupAddress = value; } }

    /// <summary>
    /// Instantiate the list of grouped computers, 
    /// set the position of this
    /// </summary>
    private void Awake()
    {
        // Initialize the list
        groupedComputers = new List<GameObject>();
        // Set the attempt count
        attemptCount = 0;
    }

    /// <summary>
    /// This coroutine will take the string, split it apart at 
    /// at the delimeter of '.' and the parse each of those values
    /// to an integer and store them in the tempIntValues field
    /// </summary>
    /// <param name="ipToCheck">An IPV4 address to parse</param>
    /// <returns></returns>
    public int[] ParseIPv4(string ipToCheck)
    {
        // Break out if the string is null
        if (ipToCheck == null)
        {
            return null;
        }

        // Two variables the make sure that what we parse is actually an integer
        bool check;
        int x;

        // Split the IP address at periods first
        stringValues = ipToCheck.Split('.');

        // As long as this array actually has useful info about an ip in it
        if(stringValues.Length <= 2)
        {
            return null;
        }

        // Make an integer array that is the same size as what we split
        tempIntValues = new int[stringValues.Length];


        // Loop through the string array and parse the strings to integers
        for (int i = 0; i < stringValues.Length; i++)
        {
            // Parse string to int, storing what I get the in integer array
            check = int.TryParse(stringValues[i], out x);

            // If what we parsed is an integer and it worked:
            if (check)
            {
                // Set the value in the integer array
                tempIntValues[i] = x;
            }
            else
            {
                // What we parsed was not an int, break out of the loop
                break;
            }
        }

        return tempIntValues;

    }

    /// <summary>
    /// This method will compare the string and see if it is part of
    /// our group. If it is return true, if not return false
    /// </summary>
    /// <param name="IpAddress">The IP address that we want to compare</param>
    /// <returns></returns>
    public bool CheckIPv4(string IpAddress)
    {

        tempIntValues = ParseIPv4(IpAddress);

        // Retrun false if the temp values are null
        if (tempIntValues == null) return false;

        // Now we can move on to the comparison
        for (int i = 0; i < groupAddress.Length - 1; i++)
        {
            if(groupAddress[i] != tempIntValues[i])
            {
                // The numbers don't match, return false
                return false;
            }
        }

        // Add this IP to the group
        AddToGroup(IpAddress);

        return true;
    }

    /// <summary>
    /// Using the given IP address we will add it to this group
    /// </summary>
    /// <param name="IpAddress"></param>
    public void AddToGroup(string IpAddress)
    {
        // If our dictionary contains this...
        if (GameController.currentGameController.CheckDictionary(IpAddress))
        {
            // Cache the object here
            tempObj = GameController.currentGameController.ComputersDict[IpAddress];

            // Add it to our list
            groupedComputers.Add(tempObj);

            // Move the object to our spot
            MoveToGroupSpot(tempObj);

            // Assign the the group color to this object
            SetColor(tempObj);
        }
    }

    /// <summary>
    /// This method will move a gameobject to the group position
    /// </summary>
    private void MoveToGroupSpot(GameObject thingToMove)
    {
        attemptCount++;

        // Make the this group the parent of the computer object
        thingToMove.transform.parent = gameObject.transform;

        // I need to put it in a random spot...
        Vector3 temp = new Vector3(
            Random.Range(transform.position.x - radius, transform.position.x + radius),
            Random.Range(transform.position.y - radius, transform.position.y + radius),
            Random.Range(transform.position.z - radius, transform.position.z + radius));

        // Check if I am colliding with any other groups
        Collider[] neighbors = Physics.OverlapSphere(temp, minDistanceApart);

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
    private void SetColor(GameObject thingToChange)
    {
        // Get the component that has the method to change the color for the particle system
        IncreaseEmission temp = thingToChange.GetComponent<IncreaseEmission>();

        // if this is null, then return
        if(temp == null)        
            return;

        // Change the color to the group color
        temp.ChangeColor(groupColor);
    }
}
