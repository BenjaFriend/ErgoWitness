using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// This will have an OnTrigger enter event os that whenever the user
/// clicks on it.
/// 
/// Author: Ben Hoffman
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class InteractiveButton : MonoBehaviour {

    public UnityEvent CustomEvents;

    [Tooltip("Tint the sprite when the controller is 'hovering' on it")]
    public Color highlightColor;
    private AudioSource audioSource;

    private Image buttonImage;
    private bool isHighlighted;


    private void Start()
    {
        // Get the image componenet on this object
        buttonImage = GetComponent<Image>();

        // Get teh audio component
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This method will be called whenever I want to activate this button. 
    /// Thus givine me the ability to add however many events I want to this
    /// one object, and i can easily implement it into SteamVR controller stuff that I already
    /// have.
    /// 
    /// Author: Ben Hoffman
    /// </summary>
	public void ButtonAction()
    {
        // Invoke the events that we want
        CustomEvents.Invoke();

        // Play the button click sound
        audioSource.PlayOneShot(audioSource.clip);
    }

    /// <summary>
    /// This will change the color of the image of this object
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    public void Highlight()
    {
        // If we are not highlighting already...
        if (!isHighlighted)
        {
            // Change the color of the button image
			buttonImage.color += highlightColor;
            // Keep track that we are highlighted
            isHighlighted = true;
        }
    }

    /// <summary>
    /// Method will un-highlight the sprite color
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    public void UnHighlight()
    {
        // If we are highlighted...
        if (isHighlighted)
        {
            // Then remove the color of highlighting
            buttonImage.color -= highlightColor;
            // This is no longer highlighted
            isHighlighted = false;
        }
    }

}


