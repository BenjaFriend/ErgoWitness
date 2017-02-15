using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This represents the 
/// </summary>
public class IPGroup : MonoBehaviour {

    private int[] groupAddress;     // This is the IP address parsed into integers, with delimeters at the periods
    private string[] stringValues;
    private int[] tempIntValues;        // Used for comparisons
    public int[] GroupAddress { get { return groupAddress; } set { groupAddress = value; } }


    public bool FitsInGroup(string ipToCheck)
    {
        // Return false if the IP is null
        if (ipToCheck == null) return false;

        // Split the IP address at periods first
        stringValues = ipToCheck.Split('.');

        tempIntValues = new int[stringValues.Length];
        
        for(int i = 0; i < stringValues.Length; i++)
        {
            int.TryParse(stringValues[i], out tempIntValues[i]);
        }


        // If the first 3 numbers are the same, then these IP's are of the same group

        return false;
    }
}
