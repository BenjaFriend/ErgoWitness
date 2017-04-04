using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All this script will do is load level 1.
/// This will create pre-loading of assets so that the main scene doesn't
/// have to do that on load.
/// </summary>
public class LoadAssets : MonoBehaviour {



	void Awake ()
    {
        // Load in the main scene
        Application.LoadLevel(1);
    }
	

}
