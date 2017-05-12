using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move this object from the source to the destination. The movement
/// begins after setting the destination. 
/// 
/// Control the color of this netflow data, which represents the type
/// of traffic that it is
/// </summary>
[RequireComponent(typeof(TrailRenderer))]
public class NetflowObject : MoveFromSourceToTarget
{
    #region Fields

    public ParticleSystem trailPartical;         // The emission module of this particle system
    public float lifeAfterDestination = 5f; // How long this object will stay active


    private ParticleSystemRenderer headParticle; //The Particle system on this object

    private TrailRenderer _trailRend;

    private float timePaused;
    private float orgininalTime;

    #endregion


    #region Properties

    public Material HeadParticleMaterial
    {
        get
        {
            return headParticle.material;
        }

        set
        {
            headParticle.material = value;
        }
    }

    public Gradient TrailColor
    {
        set
        {
            // Set the tint color of the material
            _trailRend.colorGradient = value;
        }
    }

    #endregion


    /// <summary>
    /// Get a reference to the particles
    /// </summary>
    void Awake()
    {
        // Get the pulsing particles
        headParticle = GetComponent<ParticleSystemRenderer>();

        // Get the trail renderer componenet
        _trailRend = GetComponent<TrailRenderer>();

        // Keep track of the 
        orgininalTime = _trailRend.time;
    }

    private void Update()
    {
        // If we are paused, then keep track of the amount of time that we have been paused
        if (NetflowPauseController.IsPaused)
        {
            //timePaused += Time.deltaTime;
            // Add to the amount of time that the trail renderer stays
            _trailRend.time += Time.deltaTime;
        }
    }

    /// <summary>
    /// This will disable this game object after it reaches its destiantion
    /// 
    /// Author: Ben hoffman
    /// </summary>
    public override void ReachedDestination()
    {
        base.ReachedDestination();

        Invoke("Destroy", 2f);
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the color of the trail particles to this gradient
    /// </summary>
    /// <param name="changeTo"></param>
    public void SetColor(Gradient changeTo)
    {
        // Get a reference to the main
        ParticleSystem.MainModule main = trailPartical.main;

        // Change the color to the gradient
        main.startColor = changeTo;
    }

    private void OnDisable()
    {
        // Reset the trail renderer time when this object is disabled
        _trailRend.time = orgininalTime;
    }
}
