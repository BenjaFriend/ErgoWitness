using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Text sourceIpText;
    public Text destIpText;
    private double numHits;     // How many times have we seen this IP get hit?
    private LineRenderer lineRend;  // The line renderer
    #endregion

    #region Mutators
    public Source ComputerSourceInfo { get { return computerSourceInfo; } set { computerSourceInfo = value; } }
    public List<GameObject> ConnectedComputers { get { return connectedComputers; } }
    public double NumHits { get { return numHits; } set { numHits = value; } }
    #endregion


    void Start()
    {
        numHits = 1;
        connectedComputers = new List<GameObject>();
        lineRend = GetComponent<LineRenderer>();
        lineRend.SetPosition(0, Vector3.zero);
        UpdateUI();
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

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To update the UI of this object on 
    /// start and when a new connection is added
    /// </summary>
    private void UpdateUI()
    {
        sourceIpText.text = "Source IP: " + computerSourceInfo.source.ip;
        destIpText.text = "Dest. IP: " + computerSourceInfo.dest.ip;

        // Change the color of the  line renderer material based on the protocol
        switch (computerSourceInfo.transport)
        {
            case ("tcp"):
                // Light Gray color
                lineRend.material.color = Color.gray;
                break;
            case ("udp"):
                // Light blue
                lineRend.material.color = Color.blue;
                break;
            case ("http"):
                // Light green
                lineRend.material.color = Color.green;
                break;
            case ("https"):
                // Vibrant green
                lineRend.material.color = Color.green;              
                break;
        }
    }

}
