using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;

/// <summary>
/// This is will manage all of my groups, so if an IP 
/// does not fit into a group then I will make a new one
/// </summary>
public class IPGroupManager : MonoBehaviour
{

    #region Fields

    public Color[] possibleColors;    // The possible colors that we want to assign the groups at random
    public Color[] blueTeamColors;      // The possible colors for the blue team
    public Color[] redTeamColors;        // The possible colors for the red team

    public static IPGroupManager currentIpGroups;   // A static reference to this manager

    public Dictionary<int, IPGroup> groupsDictionary;  // A dictionary of all the groups that we currently have
    public GameObject groupPrefab;  // The prefab for the IP group
    public float minDistanceApart = 15f;
    public float size = 100f;
    public float increaseAmountPerGroup = 10f;  // This will be added to the size every time that a new group is added
                                                // So that the radius gets bigger and bigger
    private int lastColorUsed;    // Keep track of the last color so that we don't assign it twice in a row

    private IPGroup newGroup;       // variable that I will use as a temp
    private GameObject temp;        // Temp reference to a gameObject
    private int attemptCount;

    private int[] redTeamIpIntArray;   // Integer array of the red team IP addresses
    private int[] blueTeamIntArray;    // Integer array of the blue team IP 

    #endregion

    /// <summary>
    /// Make sure that we only ahve one of these managers in our scene
    /// </summary>
    private void Awake()
    {
        // If there are no other objects that have been assigned this static reference yet... 
        if (currentIpGroups == null)
        {
            // Set the static reference
            currentIpGroups = this;
        }
        // There is another object assigned already, so destroiy this
        else if (currentIpGroups != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Set the static referce, initialize the list of groups
    /// </summary>
    void Start()
    {
        // Initalize the group dictiona
        groupsDictionary = new Dictionary<int, IPGroup>();

        // Set our attempt count to 0
        attemptCount = 0;

        // Read in the read team IP addresses
        redTeamIpIntArray = new int[0];
        // Read in all the blue team IP addresses, and store them in an array of integers
        blueTeamIntArray = new int[0];
    }

    /// <summary>
    /// Take in a file location, read that file using the StreamReader class,
    /// and return an integer array of bit shifted IP addresses
    /// </summary>
    /// <param name="fileLocation">Which file we are reading from</param>
    /// <returns>An integer array of bit-shifted IP addresses that were in the file</returns>
    private int[] ReadInIps(string fileLocation)
    {
        // A list of strings to keep track of the IP addresses
        List<string> ipStringsList = new List<string>();

        string line;
        int counter = 0;

        // Read the file and display it line by line.  
        System.IO.StreamReader file =
            new System.IO.StreamReader(fileLocation);

        while ((line = file.ReadLine()) != null)
        {
            // Read in the line that has the IP address
            ipStringsList.Add(line);
            counter++;
        }

        // Close te file reader
        file.Close();

        // Create a new integer array the size of however many objects that we have
        int[] ipIntArray = new int[counter];


        for (int i = 0; i < ipStringsList.Count; i++)
        {
            // Set the integer array value to the string value
            ipIntArray[i] = IpToInt(ipStringsList[i]);
        }

        // Return the result
        return ipIntArray;
    }

    public void SetIpsViaOptions(string[] ipAddress, int groupNum)
    {
        int[] ipIntArray = new int[ipAddress.Length];

        // Loop through the amount of strings that we need
        for (int i = 0; i < ipAddress.Length; i++)
        {
            // Convert string to integer
            ipIntArray[i] = IpToInt(ipAddress[i]);
        }

        // Assign either red or blue team the address
        switch (groupNum)
        {
            case 0:
                blueTeamIntArray = ipIntArray;
                break;
            case 1:
                redTeamIpIntArray = ipIntArray;
                break;
            default:
                Debug.Log("There is no array of that index! Group manager");
                break;
        }


    }

    /// <summary>
    /// Remove a computer from it's group
    /// </summary>
    /// <param name="compToRemove"></param>
    public void RemoveGroup(int groupToRemove)
    {
        // Remove this object from the group
        groupsDictionary.Remove(groupToRemove);
    }

    /// <summary>
    /// Loops through and checks if this IP fits into any of these groups
    /// If it does, then add it to the group. If not, create a new group 
    /// based on it
    /// </summary>
    /// <param name="ipToCheck">The IP that we want to check if it fits in a group</param>
    public void CheckGroups(int ipToCheck)
    {
        // This IP is not valid, so return out of this method
        if (ipToCheck == 0) return;

        // Get the first 3 numbers of this IP in an integer
        int ipFirstThree = GetFirstThreeIpInt(ipToCheck);

        // If the group dictionary has a group with the same first 3 numbers, and that group is not dying right now
        if (groupsDictionary.ContainsKey(ipFirstThree) && !groupsDictionary[ipFirstThree].IsDying)
        {
            // Add to that group
            groupsDictionary[ipFirstThree].AddToGroup(ipToCheck);

            // We are done here, nothing else needed
            return;
        }

        // If the IP does not fit into any groups, then make a new group
        MakeNewGroup(ipToCheck, ipFirstThree);
    }

    /// <summary>
    /// Create a new group for computers to be added to
    /// </summary>
    /// <param name="ipToCheck"></param>
    private void MakeNewGroup(int ipToCheck, int groupIPFirstThree)
    {
        // Instatiate a new instance of a group, and set it equal to temp so we can access it
        temp = (GameObject)Instantiate(groupPrefab, transform.position, Quaternion.identity);

        // Parent the object to the manager, so that we can scale it
        temp.transform.parent = this.transform;

        // If it doesnt fit then we have to make a new group object
        newGroup = temp.GetComponent<IPGroup>();

        // Set the group address to the first 3 numbers of the new IP
        newGroup.GroupAddress = groupIPFirstThree;

        // Set the position of this group to something random
        SetGroupPosition(temp);

        // Set the color of the group
        SetGroupColor(newGroup);

        // Add to the dictoinary of groups
        groupsDictionary.Add(groupIPFirstThree, newGroup);

        // Add the IP to that group
        newGroup.AddToGroup(ipToCheck);
    }

    /// <summary>
    /// Move the given game object to the group spot
    /// </summary>
    /// <param name="moveMe"></param>
    private void SetGroupPosition(GameObject moveMe)
    {
        attemptCount++;

        // I need to put it in a random spot...
        Vector3 temp = new Vector3(
            Random.Range(-size, size),
            Random.Range(-size, size),
            Random.Range(-size, size));


        // Check if I am colliding with any other groups
        Collider[] neighbors = Physics.OverlapSphere(temp, minDistanceApart);

        // There is something colliding with us, recursively call this function
        if (neighbors.Length > 0 && attemptCount <= 4)
        {
            // Try again   
            SetGroupPosition(moveMe);
        }
        else
        {
            // Set the transform to this random location
            moveMe.transform.localPosition = temp;

            // Reset the attempt count
            attemptCount = 0;

            // Increase the size of the groups
            size += increaseAmountPerGroup;
        }
    }

    /// <summary>
    /// This method will set the group color field to one 
    /// of the random colors in thepossible colors array
    /// </summary>
    /// <param name="groupToColor"></param>
    private void SetGroupColor(IPGroup groupToColor)
    {
        // Check and make sure taht we actually have some colors
        if (possibleColors == null || possibleColors.Length == 0)
        {
            // Return if we dont
            return;
        }

        // See if we are looking at blue team address or not
        int whichBlueTeam = isBlueTeam(groupToColor.GroupAddress);
        int whichRedTeam = IsRedTeam(groupToColor.GroupAddress);

        // Check if the IP is red team or blue
        if (whichRedTeam >= 0)
        {
            // This is a special team that we care about
            groupToColor.IsSpecialTeam = true;

            // Set the color to the red team specific color
            if (whichRedTeam >= redTeamColors.Length)
            {
                // Pick a random red team color
                groupToColor.GroupColor = redTeamColors[Random.Range(0, redTeamColors.Length)];
            }
            else
            {
                // Set it to a proper red team color
                groupToColor.GroupColor = redTeamColors[whichRedTeam];
            }
            return;
        }
        // This is blue team
        else if (whichBlueTeam >= 0)
        {
            // tell us that this is a special team that we care about
            groupToColor.IsSpecialTeam = true;

            // Set the color to the blue team specific color
            if (whichBlueTeam >= blueTeamColors.Length)
            {
                groupToColor.GroupColor = blueTeamColors[Random.Range(0, blueTeamColors.Length)];
            }
            else
            {
                groupToColor.GroupColor = blueTeamColors[whichBlueTeam];
            }

            return;
        }

        // If we only have one color
        else if (possibleColors.Length == 1)
        {
            // Set that color to the only one that we have
            groupToColor.GroupColor = possibleColors[0];
            // We are done so return
            return;
        }

        // Generate a random number that represents the index of possible color array
        int x = Random.Range(0, possibleColors.Length);

        // If this is the same as the last color used
        while (x == lastColorUsed)
        {
            // This is so that we never get the same color that we used last time
            x = Random.Range(0, possibleColors.Length);
        }

        // Set the group color to that
        groupToColor.GroupColor = possibleColors[x];

        // Store the last color that we used
        lastColorUsed = x;
    }

    /// <summary>
    /// Returns true if this is one of the blue teams subnets
    /// </summary>
    /// <param name="ipInt"></param>
    /// <returns></returns>
    private int isBlueTeam(int ipInt)
    {
        for (int i = 0; i < blueTeamIntArray.Length; i++)
        {
            if (ipInt == blueTeamIntArray[i])
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns -1 if this objet is NOT red team, but if it is then it will
    /// return the team number that it is based on the config file
    /// </summary>
    /// <param name="ipInt">The IP to check if it is red team</param>
    /// <returns>-1 if not red team, anything else is red team</returns>
    private int IsRedTeam(int ipInt)
    {
        for (int i = 0; i < redTeamIpIntArray.Length; i++)
        {
            // If this number matches one of the red team integers...
            if (ipInt == redTeamIpIntArray[i])
            {
                // Return the team index
                return i;
            }
        }
        // This is not a red team, return out
        return -1;
    }

    /// <summary>
    /// Return an integer that represents
    /// only the first three numbers of the given IP address
    /// </summary>
    /// <param name="ipToCheck">The IP that we are checking</param>
    /// <returns></returns>
    public int GetFirstThreeIpInt(int ipToCheck)
    {
        // Break out if the string is null
        if (ipToCheck == 0) return 0;

        // Send the IP to a string 
        string ipAsString = IpIntToString(ipToCheck);


        // Split the IP address at periods first
        string[] stringValues = ipAsString.Split('.');

        // Remove the last element off of that array
        stringValues = stringValues.Take(stringValues.Count() - 1).ToArray();

        // Build up a new string with a string builder
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < stringValues.Length; i++)
        {
            builder.Append(stringValues[i]);
            builder.Append('.');
        }

        // Remove the last period that we dont need
        builder.Remove(builder.Length - 1, 1);

        // Return the value of the string all put together, CONVERTED TO AN INT
        return IpToInt(builder.ToString());
    }

    /// <summary>
    /// Convert an integer IP value to a string
    /// </summary>
    /// <param name="ipAddrInt"></param>
    /// <returns></returns>
    private string IpIntToString(int ipAddrInt)
    {
        // Make sure that this ip is valid
        if (ipAddrInt == 0) return null;

        return new IPAddress(System.BitConverter.GetBytes(ipAddrInt)).ToString();
    }

    /// <summary>
    /// Convert a string IP address to an integer
    /// </summary>
    /// <param name="ipAddr"></param>
    /// <returns></returns>
    private int IpToInt(string ipAddr)
    {
        // Make sure that this address is valid
        if (ipAddr == null) return 0;
        IPAddress ipAddress;
        // See if this is a valid IP address first
        bool result = IPAddress.TryParse(ipAddr, out ipAddress);

        if (result)
        {
            return System.BitConverter.ToInt32(IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
        }

        return -1;
    }



    public IEnumerator HideAlertType(int alertType)
    {
        // Tell the IP groups as well

        for (int i = 0; i < groupsDictionary.Count; i++)
        {
            // Calculate all alerts for each computer 
            groupsDictionary.ElementAt(i).Value.ToggleAttackType(alertType);

            // Wait for the end of this frame
            yield return null;
        }


    }

}