using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;

/// <summary>
/// This is will manage all of my groups, so if an IP 
/// does not fit into a group then I will make a new one
/// </summary>
public class IPGroupManager : MonoBehaviour {

    public Material[] possibleColors;      // The possible colors that we want to assign the groups at random
    public Material blueTeamMat;
    public Material redTeamMat;

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

    private int redTeamIpInt;
    private int blueTeamIpInt;

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

        redTeamIpInt = IpToInt(System.IO.File.ReadAllText(Application.streamingAssetsPath + "/TeamIPs/redTeam.txt"));
        blueTeamIpInt = IpToInt(System.IO.File.ReadAllText(Application.streamingAssetsPath + "/TeamIPs/blueTeam.txt"));
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

        // Check if the IP is red team or blue
        if(groupToColor.GroupAddress == redTeamIpInt)
        {
            // Set the color to the red team specific color
            groupToColor.GroupColor = redTeamMat;
            return;
        }
        else if(groupToColor.GroupAddress == blueTeamIpInt)
        {
            // Set the color to the blue team specific color
            groupToColor.GroupColor = blueTeamMat;
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

        return System.BitConverter.ToInt32(IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
    }

}
