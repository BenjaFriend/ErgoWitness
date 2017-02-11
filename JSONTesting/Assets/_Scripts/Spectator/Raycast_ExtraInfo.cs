using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Detect if the user is looking at a PC, and if they are then show
/// the information of that PC in the HUD
/// </summary>
public class Raycast_ExtraInfo : MonoBehaviour {

    public Text sourceIpText;
    public Text sourcePortText;
    public Text destIpText;
    public Text destPortText;
    public Text protoText;

    private RaycastHit hitInfo;
    private Ray ray;
    private GameObject target;
    public Animator anim;
    private Vector3 screenCenter;
    private Computer compInfo;

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
                //target.GetComponent<Fade_UI>().FadeIn();
                // Set the reticle to open
                anim.SetBool("isLooking", true);

                // Get the information on that PC and display it in my hud
                SetValues(target.GetComponent<Computer>().sourceInfo);

            }
            else if (target != null)
            {
                //target.GetComponent<Fade_UI>().FadeOut();
                target = null;
                // Set the reteicle to close
                anim.SetBool("isLooking", false);
            }
        }
    }


    public void SetValues(Source data)
    {
        sourceIpText.text = "Source IP: " + data.id_orig_h.ToString();
        // Source port
        sourcePortText.text = "Source Port: " + data.id_orig_p.ToString();


        destIpText.text = "Dest. IP: " + data.id_resp_h.ToString();
        // Dest port
        destPortText.text = "Dest. Port: " + data.id_resp_p.ToString();

        protoText.text = "Protocol: " + data.proto.ToString();

    }
}
