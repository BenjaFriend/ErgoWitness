using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keep track of the largest object in the scene
/// 
/// 
/// </summary>
public class OrbitManager : MonoBehaviour {

    public static OrbitManager Instance;

    public Transform CentralBody;

    private void Start()
    {
        Instance = this;                
    }

}
