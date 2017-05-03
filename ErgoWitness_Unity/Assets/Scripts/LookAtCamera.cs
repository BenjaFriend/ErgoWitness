using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    /// <summary>
    /// Look at the main camera
    /// 
    /// Author: Ben Hoffman
    /// </summary>
	void Update ()
    {
        transform.LookAt(Camera.main.transform);	
	}
}
