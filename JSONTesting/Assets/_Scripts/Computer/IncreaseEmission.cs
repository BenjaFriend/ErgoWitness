using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseEmission : MonoBehaviour {

    #region Fields
    public ParticleSystem particles; // What particle system are we using?
    
    public float minHits = 1f;      // The smallest amount of particles that we can emit
    public float maxHits = 100;   // The max number of particles that we an emit
    public float degradeRate = 10f; // How fast will we degrade?

    private float currentSize = 0f;     // The current emission rate over time
    private ParticleSystem.EmissionModule em;   // The emission module of this particle system
    #endregion

    void Start()
    {
        // Set the start size to the max
        currentSize = maxHits;
        // Set the emission rate to the current one
        AddHit();
    }

    void Update()
    {
        // Start degrading this because it hasnt been hit in a while
        currentSize = Mathf.Clamp(currentSize - degradeRate * Time.deltaTime, minHits, maxHits);
        // Change the emission of particles
        UpdateParticles();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Add to the computer's emission rate, and 
    /// reset the time to degrade
    /// </summary>
    public void AddHit()
    {
        // Set the current size to how we started because we are active again
        currentSize = maxHits;
        // Clamp that emmision rate to the max
        currentSize = Mathf.Clamp(currentSize, minHits, maxHits);
        // Actually change the emission of particles
        UpdateParticles();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Actually do the update for the particle system
    /// </summary>
    public void UpdateParticles()
    {
        // Get a reference to the emitter
        em = particles.emission;
        // Make sure that it is enabled
        em.enabled = true;
        // Set the rate
        em.rateOverTime = currentSize;
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Just change the color of the particle system to something
    /// </summary>
    /// <param name="changeTo"></param>
    public void ChangeColor(Color changeTo)
    {
        // Get a reference to the color
        var ma = particles.main;
        // Change that color
        ma.startColor = changeTo;
    }
}