using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast_ExtraInfo : MonoBehaviour {

    private RaycastHit hitInfo;
    private GameObject target;
    public Animator anim;

	// Update is called once per frame
	void Update ()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo))
        {
            if (hitInfo.collider.tag == "Comp")
            {
                // Show the info on that particular on that computer
                target = hitInfo.collider.gameObject;
                target.GetComponent<Fade_UI>().FadeIn();
                anim.SetBool("isLooking", true);
                //anim.SetTrigger("Expand");
            }
            else if (target != null)
            {
                target.GetComponent<Fade_UI>().FadeOut();
                target = null;
                anim.SetBool("isLooking", false);
                //anim.SetTrigger("Close");
            }
        }
    }
}
