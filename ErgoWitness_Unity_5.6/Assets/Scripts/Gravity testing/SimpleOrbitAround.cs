using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All this class will do is have an object rotate around a target
/// at a distance
/// </summary>
public class SimpleOrbitAround : MonoBehaviour {


    public Transform target;

    public float orbitSpeed;
    public float orbitDistance;
	
	// Update is called once per frame
	void LateUpdate ()
    {
        Orbit();
	}

    private void Orbit()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + (transform.position - target.position).normalized * orbitDistance, 0.125f);
        transform.RotateAround(target.position, target.up, Time.deltaTime * orbitSpeed);
    }
}
