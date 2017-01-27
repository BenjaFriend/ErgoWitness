using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// Purpose of class: This will show the info of whatever
/// computer you are looking at when you look at it
/// </summary>
public class ShowInfoOnLook : MonoBehaviour {

    private RaycastHit hit;

	// Update is called once per frame
	void Update ()
    {
		// Raycast out a short distance and check if I am
        // hitting a a computer game object
        if(Physics.Raycast(transform.position, transform.forward,out hit, 20f))
        {
            // Hide the canvas somehow, or transition it in a cool way but i have to look that up
        }
	}
}
