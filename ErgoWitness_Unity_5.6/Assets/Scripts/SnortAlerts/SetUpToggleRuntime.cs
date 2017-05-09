using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Set up the toggle at runtime because when using a prefab, there is no 
/// target graphics set up for some reason.
/// 
/// Author: Ben Hoffman
/// </summary>
public class SetUpToggleRuntime : MonoBehaviour
{

    private Toggle myToggle;

    private Image myBackground;
    private Image myCheckmark;

	// Use this for initialization
	void Start ()
    {
        myToggle = GetComponentInParent<Toggle>();
        myBackground = GetComponent<Image>();
        Image[] imagesInChildren = GetComponentsInChildren<Image>();

        // Make sure that this image is the right one and not jsut the background again
        for(int i = 0; i < imagesInChildren.Length;i++)
        {
            // If this image is not my background...
            if(imagesInChildren[i] != myBackground)
            {
                // Then it must be my checkmark, so use it
                myCheckmark = imagesInChildren[i];
            }
        }

        myToggle.transition = Selectable.Transition.ColorTint;
        myToggle.targetGraphic = myBackground;
        myToggle.graphic = myCheckmark;

        
    }
}
