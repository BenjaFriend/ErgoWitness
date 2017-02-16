using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Display a clock, because i think that this could eb useful and 
/// it takes like 2 seconds
/// </summary>
[RequireComponent(typeof(Text))]
public class Clock : MonoBehaviour {

    private Text clockText;     // The UI part of the clock
    private string oldText;     // The old text

    void Start()
    {
        // Get the text component
        clockText = GetComponent<Text>();
    }

    void Update()
    {
        // Only update the time if the text is different from before,
        // This avoids unnecessary string concatination
        if(System.DateTime.Now.ToShortTimeString() != oldText)
        {
            oldText = System.DateTime.Now.ToShortTimeString();
            clockText.text = System.DateTime.Now.ToShortTimeString();

        }
    }
}
