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
    private Source computerSourceInfo;          // The data that I care about for each PC
    private List<Computer> connectedComputers;  // This is the list of children
    #endregion

    #region Getters and Setters
    public Source ComputerSourceInfo { get { return computerSourceInfo; } set { computerSourceInfo = value; } }
    public List<Computer> ConnectedComputers { get { return connectedComputers; } }
    #endregion

    // Constructor
     void start()
    {
        connectedComputers = new List<Computer>();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To add the given computer to my
    /// list of connected PC's
    /// </summary>
    /// <param name="connectedToMe"></param>
    public void AddConnectedPC(Computer connectedToMe)
    {
        connectedComputers.Add(connectedToMe);
    }
	
}
