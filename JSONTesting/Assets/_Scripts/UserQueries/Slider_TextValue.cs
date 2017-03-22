using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Have a UI text element and a method to set it, so that 
/// on value change of a slider I can simply change the value here
/// </summary>
[RequireComponent(typeof(UnityEngine.UI.Slider))]
public class Slider_TextValue : MonoBehaviour {

    public UnityEngine.UI.Text text;
    private UnityEngine.UI.Slider slider;

    void Start()
    {
        slider = GetComponent<UnityEngine.UI.Slider>();
        text.text = slider.value.ToString();
    }

    /// <summary>
    /// Set the text value of the sliders
    /// </summary>
    public void SetTextValue()
    {
        text.text = slider.value.ToString();
    }
}
