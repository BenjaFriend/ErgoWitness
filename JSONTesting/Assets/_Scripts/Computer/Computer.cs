using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// This class holds the Data that this computer has, and a list
/// of computers that it is conenct to
/// </summary>
[RequireComponent(typeof(Fade_UI))]
public class Computer : MonoBehaviour {


    #region Fields
    public Bro_Json broInfo;            // The info that bro gives you
    public Color tcpColor;
    public Color udpColor;
    public Color httpColor;
    public Color httpsColor;

    /// <summary>
    /// Use a linked list for this because it is better for insertion
    /// but the same for searching, there are only benefits to this
    /// </summary>
    public LinkedList<GameObject> connectedPCs;
    private Fade_UI UI;         // The UI for me to use
    private IncreaseEmission particleController;
    #endregion

    #region Mutators
    public Bro_Json BroInfo { get { return broInfo; } set { broInfo = value; } }
    #endregion

    /// <summary>
    /// Author: Ben Hoffman
    /// Set up the components that I need
    /// </summary>
    void Awake()
    {
        particleController = GetComponent<IncreaseEmission>();
        connectedPCs = new LinkedList<GameObject>();
        UI = GetComponent<Fade_UI>();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// This will let me set my UI only when I have data
    /// </summary>
    /// <param name="myData"></param>
    public void SetData( Bro_Json broData)
    {
        broInfo = broData;

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
        if(connectedToMe == null || connectedToMe == gameObject)
        {
            return;
        }

        // If I do not already know of this PC, and it's not myself...
        if (!connectedPCs.Contains(connectedToMe))
        {
            Debug.Log("Connected pc!");
            // Increase the immision of the particle system
            particleController.AddHit();

            // Add the connection to my linked list
            connectedPCs.AddLast(connectedToMe);
        }
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Purpose of method: To update the UI of this object on 
    /// start and when a new connection is added
    /// </summary>
    private void UpdateUI()
    {
        // Set all my UI data
        if(broInfo != null)
            UI.SetValues(broInfo);

        // Change the dolor of the particle system
        switch (broInfo.proto)
        {
            case ("tcp"):
                // Light Gray color
                particleController.ChangeColor(tcpColor);
                break;
            case ("udp"):
                // Light gray because we really dont care that much
                particleController.ChangeColor(udpColor);
                break;
            case ("http"):
                // Light green
                particleController.ChangeColor(httpColor);
                break;
            case ("https"):
                // Vibrant green
                particleController.ChangeColor(httpsColor);
                break;
        }
    }

}
