using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Detect if the user is looking at a PC, and if they are then show
/// the information of that PC in the HUD
/// </summary>
public class Raycast_ExtraInfo : MonoBehaviour {

    #region Fields
    public Text sourceIpText;
    public Text sourcePortText;
    public Text destIpText;
    public Text destPortText;
    public Text protoText;
    public Animator anim;

    private RaycastHit hitInfo;
    private Ray ray;
    private GameObject target;
    private Vector3 screenCenter;
    private Computer compInfo;
    #endregion

    private void Start()
    {
        // Get the center of the screen so taht we can raycast from there
        screenCenter.x = Screen.width / 2;
        screenCenter.y = Screen.height / 2;
        screenCenter.z = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        // Make a ray that is coming out of the center of the camera
        ray = Camera.main.ScreenPointToRay(screenCenter);
        // If we hit something
        if (Physics.Raycast(ray, out hitInfo))
        {
            // Is this object a computer?
            if (hitInfo.collider.tag == "Comp")
            {            
                // Show the info on that particular on that computer
                target = hitInfo.collider.gameObject;

                // Set the reticle to open
                anim.SetBool("isLooking", true);

                // Get the information on that PC and display it in my hud
                SetValues(target.GetComponent<Computer>());

            }
            // If we are not hitting a computer anymore, but we are hitting something
            else if (target != null)
            {
                // Release the target variable
                target = null;

                // Set the reteicle to close
                anim.SetBool("isLooking", false);
            }
        }
    }

    /// <summary>
    /// Set ip the values of the text for the UI
    /// </summary>
    /// <param name="data">The data that we are representing</param>
    public void SetValues(Computer data)
    {
        // Set the source IP text
        sourceIpText.text = "Source IP: " + data.SourceInfo.id_orig_h.ToString();
        // Source port
        sourcePortText.text = "Source Ports: " + data.SourceInfo.id_orig_p.ToString();

        
        // If our destination IP is null, then say that
        if (data.SourceInfo.id_resp_h == null)
        {
            destIpText.text = "Dest. IPs: None";
        }
        else
        {
            // Otherwise just put the protocal
            destIpText.text = "Dest. IPs: " + data.SourceInfo.id_resp_h.ToString();
        }

        // Check if our destination port is useful or not
        if (data.portsUsed.Count == 0)
        {
            destPortText.text = "Dest. Ports: None";
        }
        else
        {
            destPortText.text = "Dest. Ports: " + data.SourceInfo.id_resp_p.ToString();
        }

        // If our protocl is null then show that in the text
        if (data.protocolsUsed.Count == 0)
        {
            protoText.text = "Protocols: None";          
        }
        else
        {
            // Otherwise just put the protocal
            protoText.text = "Protocols: ";
            for(int i = 0; i < data.protocolsUsed.Count; i++)
            {
                protoText.text += "  " + data.protocolsUsed[i];
            }
        }

    }
}
