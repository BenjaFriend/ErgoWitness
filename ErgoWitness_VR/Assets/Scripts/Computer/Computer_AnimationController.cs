using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This small script will be in charge of the animations on the computer
/// objects. On enable, play the animation to make them pop up, and on disable
/// play the aniamtion of them shrinking down
/// </summary>
public class Computer_AnimationController : MonoBehaviour {
    
    private Animator anim;  // The animation component

    private string awakeAnimationTriggerName = "WakeUp";    // The name of the wake up trigger
    private string sleepAnimationTriggerName = "Sleep";     // The name of the sleep trigger

	/// <summary>
    /// Get the animator component for this object
    /// </summary>
	void Awake ()
    {
        // Get the animator component on this object
        anim = GetComponentInChildren<Animator>();
    }
	
    /// <summary>
    /// Play the awake animation
    /// </summary>
    void OnEnable()
    {        
        // Set the trigger for the animaton to play
        anim.SetTrigger(awakeAnimationTriggerName);
    }

    /// <summary>
    /// Play the sleep animation on this object
    /// </summary>
    public void PlaySleepAnim()
    {
        // Set the trigger for the animation to play
        anim.SetTrigger(sleepAnimationTriggerName);
    }

}
