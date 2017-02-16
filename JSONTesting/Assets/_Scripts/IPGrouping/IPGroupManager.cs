using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is will manage all of my groups, so if an IP 
/// does not fit into a group then I will make a new one
/// </summary>
public class IPGroupManager : MonoBehaviour {

    public static IPGroupManager currentIpGroups;   // A static reference to this manager
    public List<IPGroup> groups;    // A list of all the groups that we have
    public GameObject groupPrefab;  // The prefab for the IP group
    public float minDistanceApart = 15f;
    public float size = 100f;
    public float increaseAmountPerGroup = 10f;  // This will be added to the size every time that a new group is added
                                                // So that the radius gets bigger and bigger

    private IPGroup newGroup;       // variable that I will use as a temp
    private GameObject temp;        // Temp reference to a gameObject
    private int attemptCount;

    /// <summary>
    /// Set the static referce, initialize the list of groups
    /// </summary>
    void Start ()
    {
        currentIpGroups = this;
        groups = new List<IPGroup>();
        attemptCount = 0;
    }
	
    /// <summary>
    /// Loops through and checks if this IP fits into any of these groups
    /// If it does, then add it to the group. If not, create a new group 
    /// based on it
    /// </summary>
    /// <param name="ipToCheck">The IP that we want to check if it fits in a group</param>
    public void CheckGroups(string ipToCheck)
    {
        if (ipToCheck == null) return;

        for(int i = 0; i < groups.Count; i++)
        {
            // if this IP fits into the group
            if (groups[i].CheckIPv4(ipToCheck))
            {
                // Return out of this method, we are done
                return;
            }
        }

        // If the IP does not fit into any groups, then make a new group
        MakeNewGroup(ipToCheck);
    }

    /// <summary>
    /// Create a new group for computers to be added to
    /// </summary>
    /// <param name="ipToCheck"></param>
    private void MakeNewGroup(string ipToCheck)
    {
        // As long as we can use this, then make a new object
        if (!CheckIPv4IsValid(ipToCheck, 2))
        {
            return;
        }

        // Instatiate a new instance of a group, and set it equal to temp so we can access it
        temp = (GameObject)Instantiate(groupPrefab, transform.position, Quaternion.identity);
    
        // If it doesnt fit then we have to make a new group object
        newGroup = temp.GetComponent<IPGroup>();

        newGroup.GroupAddress = newGroup.ParseIPv4(ipToCheck);

        // Add the IP to that group
        newGroup.AddToGroup(ipToCheck);

        // Set the position of this group to something random
        SetGroupPosition(temp);

        // Add the new group to the list of groups
        groups.Add(newGroup);
    }

    /// <summary>
    /// Split the ip into an array of strings at every '.'
    /// If there array length is less then the specified minimum length,
    /// then return false. Otherwise return true. This is to avoid creating a new instace
    /// of the objet and then having to destroy it anyway
    /// </summary>
    /// <param name="ipToCheck">IP to check</param>
    /// <param name="minLength">The minimun length of the split array of strings</param>
    /// <returns></returns>
    public bool CheckIPv4IsValid(string ipToCheck, int minLength)
    {
        // Break out if the string is null
        if (ipToCheck == null)
        {
            return false;
        }

        // Split the IP address at periods first
        string[] stringValues = ipToCheck.Split('.');

        // As long as this array actually has useful info about an ip in it
        if (stringValues.Length <= minLength)
        {
            return false;
        }

        return true;
   
    }

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
        }         
    }

}
