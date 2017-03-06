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
            protocol = value; 
        }
    }

    public Material TrailMaterial { get { return trailRend.material; } set { trailRend.material = value; } }

    #endregion


    /// <summary>
    /// Get the line renderer component
    /// </summary>
    void Awake ()
    {
        // Get the trail renderer component
        trailRend = GetComponent<TrailRenderer>();
    }   

}
