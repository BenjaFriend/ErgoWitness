using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// This class holds the Data that this computer has, and a list
/// of computers that it is conenct to
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Computer : MonoBehaviour {

    #region Fields
    public Source computerSourceInfo;          // The data that I care about for each PC
    public List<GameObject> connectedComputers;  // This is the list of children
    private double numHits;     // How many times have we seen this IP get hit?
    private LineRenderer lineRend;  // The line renderer
    #endregion

    #region Getters and Setters
    public Source ComputerSourceInfo { get { return computerSourceInfo; } set { computerSourceInfo = value; } }
    public List<GameObject> ConnectedComputers { get { return connectedComputers; } }
    public double NumHits { get { return numHits; } set { numHits = value; } }
    #endregion

    // Constructor
     void Start()
    {
        numHits = 1;
        connectedComputers = new List<GameObject>();
        lineRend = GetComponent<LineRenderer>();
        lineRend.SetPosition(0, Vector3.zero);
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To add the given computer to my
    /// list of connected PC's
    /// </summary>
    /// <param name="connectedToMe">the PC that is connected to me</param>
    public void AddConnectedPC(GameObject connectedToMe)
    {
        if (!connectedComputers.Contains(connectedToMe) && connectedToMe != gameObject)
        {
            connectedComputers.Add(connectedToMe);

            // Add the position to the line renderer    
            lineRend.SetPosition(1, transform.InverseTransformPoint(connectedToMe.transform.position));        
        }
    }

}
