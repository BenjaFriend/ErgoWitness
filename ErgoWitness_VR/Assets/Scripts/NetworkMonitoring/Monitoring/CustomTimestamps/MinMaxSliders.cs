using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinMaxSliders : MonoBehaviour {

    public Slider minSlider;
    public Slider maxSlider;

    public int MinValue { get { return (int)minSlider.value; } }
    public int MaxValue { get { return (int)maxSlider.value; } }
    
	/// <summary>
    /// This is a method to make sure that the two values are correctly
    /// confined
    /// </summary>
	public void CheckValues()
    {
        if(minSlider.value >= maxSlider.value)
        {
            minSlider.value--;
        }

        if(maxSlider.value <= minSlider.value)
        {
            maxSlider.value++;
        }
    }
}
