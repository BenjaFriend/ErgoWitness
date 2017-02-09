using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast_ExtraInfo : MonoBehaviour {

    private RaycastHit hitInfo;
    private Ray ray;
    private GameObject target;
    public Animator anim;
    private Vector3 screenCenter;

    private void Start()
    {
        screenCenter.x = Screen.width / 2;
        screenCenter.y = Screen.height / 2;
        screenCenter.z = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        // Make a ray that is coming out of the center of the camera
        ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.tag == "Comp")
            {            
                // Show the info on that particular on that computer
                target = hitInfo.collider.gameObject;
                target.GetComponent<Fade_UI>().FadeIn();
                anim.SetBool("isLooking", true);
            }
            else if (target != null)
            {
                target.GetComponent<Fade_UI>().FadeOut();
                target = null;
                anim.SetBool("isLooking", false);
            }
        }
    }
}
