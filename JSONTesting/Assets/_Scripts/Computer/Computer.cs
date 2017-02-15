using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// This class holds the Data that this computer has, and a list
/// of computers that it is conenct to
/// </summary>
public class Computer : MonoBehaviour {


    #region Fields
    public Source sourceInfo;            // The info that bro gives you

    /// <summary>
    /// Use a list for this because it is better for insertion
    /// but the same for searching, there are only benefits to this
    /// </summary>
    public List<GameObject> connectedPCs;
    public List<string> protocolsUsed;
    public List<string> destinationIps;
    public List<int> portsUsed;
    private IncreaseEmission particleController;
    private Computer connectedComputer;
    #endregion

    #region Mutators

    public Source SourceInfo {
        get { return sourceInfo; }

        set
        {
            sourceInfo = value;
        }
    }
    
    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Set up the components that I need
    /// </summary>
    void Start()
    {
        // Get the particle controller for this object
        particleController = GetComponent<IncreaseEmission>();
        
        // Create a new list object
        connectedPCs = new List<GameObject>();
        protocolsUsed = new List<string>();
        destinationIps = new List<string>();
        portsUsed = new List<int>();

        if(SourceInfo.proto != null)
            protocolsUsed.Add(sourceInfo.proto);

        destinationIps.Add(sourceInfo.id_resp_h);
        portsUsed.Add(sourceInfo.id_orig_p);
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To add the given computer to my
    /// list of connected PC's
    /// </summary>
    /// <param name="connectedToMe">the PC that is connected to me</param>
    public void AddConnectedPC(GameObject connectedToMe)
    {
        // Get the computer component of this object, to ensure a couple things
        // 1. That what we are trying to connect to is actually a computer
        // 2. We can add things to our lists of protocols and what not
        connectedComputer = connectedToMe.GetComponent<Computer>();

        // If the object that we are given is null or it is myself, then stop
        if (connectedToMe == null || connectedToMe == gameObject || connectedComputer)
        {
            return;
        }

        // If I do not already know of this PC, and it's not myself...
        if (!connectedPCs.Contains(connectedToMe))
        {
            // Increase the immision of the particle system
            particleController.AddHit();

            // Add the connection to my linked list
            connectedPCs.Add(connectedToMe);
        }


        // Check if there is any new info, like a different protocol, different port, different destination, etc
        // If we have never seen this destination before, then add it to our list. 
        if (!destinationIps.Contains(connectedComputer.sourceInfo.id_orig_h))
        {
            destinationIps.Add(connectedComputer.sourceInfo.id_orig_h);
        }

        // If we have never seen this protocl before, then add it to the list
        if (!protocolsUsed.Contains(connectedComputer.sourceInfo.proto))
        {
            protocolsUsed.Add(connectedComputer.sourceInfo.proto);
        }

        // If we have never used this port before, then add it to our list
        if (!portsUsed.Contains(connectedComputer.sourceInfo.id_orig_p))
        {
            portsUsed.Add(connectedComputer.sourceInfo.id_orig_p);
        }
    }

}
