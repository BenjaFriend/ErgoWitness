using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast_ExtraInfo : MonoBehaviour {

    private RaycastHit hitInfo;

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            if(Physics.Raycast(transform.position, transform.forward, out hitInfo))
            {
                if(hitInfo.collider.tag == "Comp")
                {
                    //hitInfo.collider.GetComponent<Fade_UI>().ToggleExtra();
                }
            }
        }
	}
}
