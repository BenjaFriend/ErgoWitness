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
            if (hitInfo.collider.CompareTag("Comp"))
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
    /// Set ip the values of the text for the UI. The if statements
    /// in this are so that I change the text as little as possible, 
    /// ecause strings are immutable and it takes less memory allocation
    /// this way
    /// </summary>
    /// <param name="data">The data that we are representing</param>
    public void SetValues(Computer data)
    {
        // Set the source IP text
        if(sourceIpText.text != data.SourceInfo.id_orig_h.ToString())
        {
            sourceIpText.text = data.SourceInfo.id_orig_h.ToString();
        }

        // Source port
        if(sourcePortText.text != data.SourceInfo.id_orig_p.ToString())
        {
            sourcePortText.text = data.SourceInfo.id_orig_p.ToString();
        }

        // If our destination IP is null, then say that
        // Only change the words if we need to
        if (data.destinationIps.Count == 0 && destIpText.text != "None")
        {
            destIpText.text = "None";
        }
        else if (data.destinationIps.Count == 1 && destIpText.text != data.destinationIps[0])
        {
            destIpText.text = data.destinationIps[0];
        }
        else if(data.destinationIps.Count >= 2)
        {
            // Otherwise just put the protocal
            destIpText.text = "";
            for (int i = 0; i < data.destinationIps.Count; i++)
            {
                destIpText.text += "  " + data.destinationIps[i];
            }
        }

        // Check if our destination port is useful or not
        if (data.portsUsed.Count == 0)
        {
            destPortText.text = "None";
        }
        else
        {
            destPortText.text = data.SourceInfo.id_resp_p.ToString();
        }

        // If our protocl is null then show that in the text
        if (data.protocolsUsed.Count == 0 && protoText.text != "None")
        {
            protoText.text = "None";          
        }
        else if(data.protocolsUsed.Count == 1 && protoText.text != data.protocolsUsed[0])
        {
            protoText.text = data.protocolsUsed[0];
        }
        else if(data.protocolsUsed.Count >= 2)
        {
            protoText.text = "";
            // Otherwise just put the protocal
            for (int i = 0; i < data.protocolsUsed.Count; i++)
            {
                protoText.text += "  " + data.protocolsUsed[i];
            }
        }

    }
}
