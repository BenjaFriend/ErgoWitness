using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseEmission : MonoBehaviour {

    #region Fields
    public ParticleSystem particles;
    public float minHits = 1f;
    public float maxHits = 1000f;
    public float degradeRate = 0.1f;
    public float numHits = 0f;
    private ParticleSystem.EmissionModule em;
    #endregion

    void Start()
    {
        AddHit();
    }

    void Update()
    {
        // Start degrading this because it hasnt been hit in a while
        numHits = Mathf.Clamp(numHits - degradeRate * Time.deltaTime, minHits, maxHits);
        UpdateParticles();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Add to the computer's emission rate, and 
    /// reset the time to degrade
    /// </summary>
    public void AddHit()
    {
        numHits++;
        UpdateParticles();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Actually do the update for the particle system
    /// </summary>
    public void UpdateParticles()
    {
        em = particles.emission;

        em.enabled = true;
        em.rateOverTime = numHits;
    }
}