using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This represents the 
/// </summary>
public class IPGroup : MonoBehaviour {

    public float radius = 5f;
    //public Transform groupPosition;
    public List<GameObject> groupedComputers;

    public int[] groupAddress;     // This is the IP address parsed into integers, with delimeters at the periods
    private string[] stringValues;      // Temp variable used to store an IP split at the '.'
    private int[] tempIntValues;        // Used for comparisons
    private GameObject tempObj;
    public int[] GroupAddress { get { return groupAddress; } set { groupAddress = value; } }


    private void Awake()
    {
        groupedComputers = new List<GameObject>();
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
            // Add it to our list
            groupedComputers.Add(GameController.currentGameController.ComputersDict[IpAddress]);
        }
    }
}
