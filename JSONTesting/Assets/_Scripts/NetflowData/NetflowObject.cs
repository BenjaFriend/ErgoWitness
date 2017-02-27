using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move this object from the source to the destination. The movement
/// begins after setting the destination. 
/// 
/// Controll the color of this netflow data, which represents the type
/// of traffic that it is
/// </summary>
[RequireComponent(typeof(TrailRenderer))]
public class NetflowObject : MoveFromSourceToTarget
{

    #region Fields
    // The different colors for the protocols
    public Material tcpMat;
    public Material udpColor;
    public Material httpColor;
    public Material httpsColor;
    public Material defaultColor;

    //private Transform source;           // Our starting point
   // private Transform destinaton;       // The spot that we want to be at
    private string protocol;            // Our protocol that we represent
    private TrailRenderer trailRend;    // The trail renderer comonent
    #endregion


    #region Properties

    /// <summary>
    /// This sets the color, and if you set it then it changes the material of the line renderer
    /// </summary>
    public string Protocol
    {
        get { return protocol; }
        set
        {
            protocol = value; SetColor();
        }
    }

    #endregion


    /// <summary>
    /// Get the line renderer component
    /// </summary>
    void Awake ()
    {
        // Get the trail renderer component
        trailRend = GetComponent<TrailRenderer>();
    }   


    /// <summary>
    /// Set the color of this based on our protocol
    /// </summary>
    private void SetColor()
    {
        // Change to the proper material
        switch (protocol)
        {
            case ("tcp"):
                trailRend.material = tcpMat;
                break;
            case ("udp"):
                trailRend.material = udpColor;
                break;
            case ("http"):
                trailRend.material = httpColor;
                break;
            case ("https"):
                trailRend.material = httpsColor;
                break;
            default:
                trailRend.material = defaultColor;
                break;
        }
    }

}
