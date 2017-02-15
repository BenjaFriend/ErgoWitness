using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is will manage all of my groups, so if an IP 
/// does not fit into a group then I will make a new one
/// </summary>
public class IPGroupManager : MonoBehaviour {

    public List<IPGroup> groups;

	// Use this for initialization
	void Start ()
    {
        groups = new List<IPGroup>();
	}
	
    /// <summary>
    /// Loops through and checks if this IP fits into any of these groups
    /// If it does, then add it to the group. If not, create a new group 
    /// based on it
    /// </summary>
    /// <param name="ipToCheck"></param>
    public void CheckGroups(string ipToCheck)
    {
        if (ipToCheck == null) return;

        for(int i = 0; i < groups.Count; i++)
        {
            // if this IP fits into the group
            if (groups[i].FitsInGroup(ipToCheck))
            {
                // Return out of this method, we are done
                return;
            }
        }

        // If it doesnt fit then we have to make a new group object
        IPGroup newGroup = new IPGroup();

        // Add the new group to the list
        groups.Add(newGroup);

    }
}
