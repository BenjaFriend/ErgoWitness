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

    private IncreaseEmission particleController;
    #endregion

    #region Mutators

    public Source SourceInfo { get { return sourceInfo; } set { sourceInfo = value; } }
    
    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Set up the components that I need
    /// </summary>
    void Awake()
    {
        // Get the particle controller for this object
        particleController = GetComponent<IncreaseEmission>();
        
        // Create a new list object
        connectedPCs = new List<GameObject>();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To add the given computer to my
    /// list of connected PC's
    /// </summary>
    /// <param name="connectedToMe">the PC that is connected to me</param>
    public void AddConnectedPC(GameObject connectedToMe)
    {
        // If the object that we are given is null or it is myself, then stop
        if(connectedToMe == null || connectedToMe == gameObject)
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
    }

}
