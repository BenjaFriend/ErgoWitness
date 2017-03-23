using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
using System.IO;
/// <summary>
/// This is will manage all of my groups, so if an IP 
/// does not fit into a group then I will make a new one
/// </summary>
public class IPGroupManager : MonoBehaviour {

    public Material[] possibleColors;      // The possible colors that we want to assign the groups at random
    public Material[] blueTeamMats;
    public Material[] redTeamMat;

    public static IPGroupManager currentIpGroups;   // A static reference to this manager
    public List<IPGroup> groups;    // A list of all the groups that we have
    public Dictionary<int, IPGroup> groupsDictionary;
    public GameObject groupPrefab;  // The prefab for the IP group
    public float minDistanceApart = 15f;
    public float size = 100f;
    public float increaseAmountPerGroup = 10f;  // This will be added to the size every time that a new group is added
                                                // So that the radius gets bigger and bigger
    private int lastColorUsed;    // Keep track of the last color so that we don't assign it twice in a row
    private IPGroup newGroup;       // variable that I will use as a temp
    private GameObject temp;        // Temp reference to a gameObject
    private int attemptCount;


    private int[] redTeamIpIntArray;
    private int[] blueTeamIntArray;

    private void Awake()
    {
        if (currentIpGroups == null)
        {
            currentIpGroups = this;
        }
        else if (currentIpGroups != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Set the static referce, initialize the list of groups
    /// </summary>
    void Start ()
    {
        groupsDictionary = new Dictionary<int, IPGroup>();
        currentIpGroups = this;
        groups = new List<IPGroup>();
        attemptCount = 0;

        // Read in the red team IPs
        ReadInRedTeamIps();

        // Read in all the blue team IP addresses
        ReadInBlueTeamIps();
    }

    /// <summary>
    /// This will read in the red team IP addresses
    /// </summary>
    private void ReadInRedTeamIps()
    {
        // A list of strings to keep track of the IP addresses
        List<string> readTeamIpStrings = new List<string>();
        string line;
        int counter = 0;

        // Read the file and display it line by line.  
        System.IO.StreamReader file =
            new System.IO.StreamReader(Application.streamingAssetsPath + "/TeamIPs/redTeam.txt");

        while ((line = file.ReadLine()) != null)
        {
            // Read in the line that has the IP address
            readTeamIpStrings.Add(line);
            counter++;
        }

        // Close te file reader
        file.Close();

        // Create a new red team array that is the right lenght
        redTeamIpIntArray = new int[counter];

        for (int i = 0; i < readTeamIpStrings.Count; i++)
        {
            // Set the integer array value to the string value
            redTeamIpIntArray[i] = IpToInt(readTeamIpStrings[i]);
        }
    }

    /// <summary>
    /// This method will read in the blue team IP address,s
    /// </summary>
    private void ReadInBlueTeamIps()
    {       
        // A list of strings to keep track of the IP addresses
        List<string> blueTeamIpString = new List<string>();
        string line;
        int counter = 0;

        // Read the file and display it line by line.  
        System.IO.StreamReader file =
            new System.IO.StreamReader(Application.streamingAssetsPath + "/TeamIPs/blueTeamIPs.txt");
        while ((line = file.ReadLine()) != null)
        {
            // Read in the line that has the IP address
            blueTeamIpString.Add(line);
            counter++;
        }

        // Close te file reader
        file.Close();

        // Create a new blue team array that is the right lenght
        blueTeamIntArray = new int[counter];

        for (int i = 0; i < blueTeamIpString.Count; i++)
        {
            // Set the integer array value to the string value
            blueTeamIntArray[i] = IpToInt(blueTeamIpString[i]);
        }

    }

    /// <summary>
    /// Remove a computer from it's group
    /// </summary>
    /// <param name="compToRemove"></param>
    public void RemoveGroup(int groupToRemove)
    {
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

        // If the group dictionary has a group with the same first 3 numbers
        if (groupsDictionary.ContainsKey(ipFirstThree))
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

        // Increase the size of the groups
        size += increaseAmountPerGroup;

        // I need to put it in a random spot...
        Vector3 temp = new Vector3(
            Random.Range(-size, size),
            Random.Range(-size, size),
            Random.Range(-size, size));

        // Check if I am colliding with any other groups
        Collider[] neighbors = Physics.OverlapSphere(temp, minDistanceApart);

        // There is something colliding with us, recursively call this function
        if (neighbors.Length > 0 && attemptCount <= 10)
        {        
            // Try again   
            SetGroupPosition(moveMe);
        }
        else
        {
            // Set the transform to this random location
            moveMe.transform.position = temp;
            // Reset the attempt count
            attemptCount = 0;

            // Have the camera look at my final position
            //Automated_Camera.currentAutoCam.ChangeTarget(moveMe.transform);
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
            // Set the color to the red team specific color
            if(whichRedTeam >= redTeamMat.Length)
            {
                // Pick a random red team color
                groupToColor.GroupColor = redTeamMat[Random.Range(0, redTeamMat.Length)];
            }
            else
            {
                // Set it to a proper red team color
                groupToColor.GroupColor = redTeamMat[whichRedTeam];
            }
            return;
        }
        // This is blue team
        else if(whichBlueTeam >= 0)
        {
            groupToColor.IsBlueTeam = true;
            // Set the color to the blue team specific color
            if(whichBlueTeam >= blueTeamMats.Length)
            {
                groupToColor.GroupColor = blueTeamMats[Random.Range(0,blueTeamMats.Length)];

            }
            else
            {
                groupToColor.GroupColor = blueTeamMats[whichBlueTeam];
            }

            return;
        }

        // If we only have one color
        else if(possibleColors.Length == 1)
        {
            // Set that color to the only one that we have
            groupToColor.GroupColor = possibleColors[0];
            // We are done so return
            return;
        }

        // Generate a random number that represents the index of possible color array
        int x = Random.Range(0, possibleColors.Length);

        // If this is the same as the last color used
        while(x == lastColorUsed)
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
        for(int i = 0; i < blueTeamIntArray.Length; i++)
        {
            if(ipInt == blueTeamIntArray[i])
            {
                return i;
            }
        }

        return -1;
    }

    private int IsRedTeam(int ipInt)
    {
        for (int i = 0; i < redTeamIpIntArray.Length; i++)
        {
            if (ipInt == redTeamIpIntArray[i])
            {
                return i;
            }
        }

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

        // Build up a new string
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for(int i = 0; i < stringValues.Length; i++)
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

}
