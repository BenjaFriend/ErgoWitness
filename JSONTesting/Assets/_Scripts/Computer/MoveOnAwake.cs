using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnAwake : MonoBehaviour {

    public Vector3 movement;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(movement * Time.deltaTime);
	}
}
