using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// This class holds the Data that this computer has, and a list
/// of computers that it is conenct to
/// </summary>
[RequireComponent(typeof(IncreaseEmission))]
public class Computer : MonoBehaviour
{
    #region Fields
    public Source sourceInfo;            // The info that bro gives you
    public Material connectionColor;
    /// <summary>
    /// Use a list for this because it is better for insertion
    /// but the same for searching, there are only benefits to this
    /// </summary>
    public List<Computer> connectedPcs;
    public List<string> protocolsUsed;
    public List<string> destinationIps;
    public List<int> portsUsed;
    private IncreaseEmission particleController;

	// This is to keep track of how many times we have seen this PC
	private int hits = 0;
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
    void OnEnable()
    {
        // Get the particle controller for this object
        particleController = GetComponent<IncreaseEmission>();
        
        // Create a new list object
        connectedPcs = new List<Computer>();

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
    public void AddConnectedPC(Computer connectedToMe)
    {
        // If the object that we are given is null or it is this game object:
        if (connectedToMe == null || connectedToMe == gameObject)
        {
            // Return out of this method
            return;
        }

        // If this device is NOT in my list already:
        if (!connectedPcs.Contains(connectedToMe))
        {
            // Increase the emmision of my the particle system
            particleController.AddHit();

            // Add the connection to my linked list
            connectedPcs.Add(connectedToMe);
        }

        // Check if there is any new info, like a different protocol, different port, different destination, etc
        // If we have never seen this destination before, then add it to our list. 
        if (!destinationIps.Contains(connectedToMe.sourceInfo.id_orig_h))
        {
            destinationIps.Add(connectedToMe.sourceInfo.id_orig_h);
        }

        // If we have never seen this protocl before, then add it to the list
        if (!protocolsUsed.Contains(connectedToMe.sourceInfo.proto))
        {
            protocolsUsed.Add(connectedToMe.sourceInfo.proto);
        }

        // If we have never used this port before, then add it to our list
        if (!portsUsed.Contains(connectedToMe.sourceInfo.id_orig_p))
        {
            portsUsed.Add(connectedToMe.sourceInfo.id_orig_p);
        }

		// Tell the streaming information that we got another hit on this IP
		hits++;

        StreamingInfo_UI.currentStreamInfo.CheckTopHits (sourceInfo.id_orig_h, hits);

    }

    /// <summary>
    /// I will use this to draw lines for right now
    /// </summary>
  /*  private void OnRenderObject()
    {
        // Set the material to be used for the first line
        connectionColor.SetPass(0);

        //Draw one line
        GL.Begin(GL.LINES);
        for (int i = 0; i < connectedPcs.Count; i++)
        {
            GL.Vertex(transform.position);        // The beginning spot of the draw line
            GL.Vertex(connectedPcs[i].transform.position);         // The endpoint of the draw line
        }

        GL.End();
    }*/

}
