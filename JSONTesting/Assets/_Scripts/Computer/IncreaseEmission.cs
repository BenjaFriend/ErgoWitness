using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseEmission : MonoBehaviour {

    #region Fields
    public ParticleSystem particles;
    
    public float minHits = 1f;
    public float maxHits = 1000f;
    public float degradeRate = 10f;
    public float increaseAmount = 100f;
    public float startSize = 0f;

    private ParticleSystem.EmissionModule em;
    #endregion

    void Start()
    {
        startSize = maxHits;
        AddHit();
    }

    void Update()
    {
        // Start degrading this because it hasnt been hit in a while
        startSize = Mathf.Clamp(startSize - degradeRate * Time.deltaTime, minHits, maxHits);
        UpdateParticles();
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Add to the computer's emission rate, and 
    /// reset the time to degrade
    /// </summary>
    public void AddHit()
    {
        startSize += increaseAmount;
        startSize = Mathf.Clamp(startSize, minHits, maxHits);
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
        em.rateOverTime = startSize;
    }

    /// <summary>
    /// Author: Ben Hoffman
    /// Just change the color of the particle system to something
    /// </summary>
    /// <param name="changeTo"></param>
    public void ChangeColor(Color changeTo)
    {
        var ma = particles.main;

        ma.startColor = changeTo;
    }
}