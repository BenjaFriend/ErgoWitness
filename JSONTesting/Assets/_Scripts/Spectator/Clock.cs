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

    private Text clockText;

    void Start()
    {
        // Get the text component
        clockText = GetComponent<Text>();
    }

    void Update()
    {
        // Set the text component of the UI to the current time
        clockText.text = System.DateTime.Now.ToShortTimeString();
    }
}
