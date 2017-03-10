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
    public Material connectionColor;
    public Gradient startColor;
    private ParticleSystemRenderer particles;
    private Material protoMaterial;
    private Material lineMaterial;
    #endregion

    #region Properties

    public Material ProtoMaterial { get { return protoMaterial; } set { particles.material = value; } }

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
    #endregion

    void Awake()
    {
        particles = GetComponent<ParticleSystemRenderer>();
    }


    void OnEnable()
    {
        lineMaterial = new Material(connectionColor);
    }

    /// <summary>
    /// If we arrived at the destination then
    /// fade out the line material
    /// </summary>
    void Update()
    {
        if (HasArrived)
        {
            // Start the coroutine for fading out the color
            Color newColor = lineMaterial.GetColor("_TintColor");
            newColor.a = Mathf.Lerp(newColor.a, 0, Time.deltaTime);
            lineMaterial.SetColor("_TintColor", newColor);
        }
    }

    /// <summary>
    /// I will use this to draw lines for right now
    /// </summary>
    private void OnRenderObject()
    {
        // Set the material to be used for the first line
        lineMaterial.SetPass(0);

        //Draw one line
        GL.Begin(GL.LINES);

        GL.Vertex(transform.position);        // The beginning spot of the draw line
        GL.Vertex(SourcePos.position);         // The endpoint of the draw line

        GL.End();
    }

}
