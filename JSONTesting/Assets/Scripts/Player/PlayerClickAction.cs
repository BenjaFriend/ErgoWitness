using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Detect if the user is looking at a PC, and if they are then show
/// the information of that PC in the HUD
/// </summary>
public class PlayerClickAction : MonoBehaviour {

    #region Fields

    private RaycastHit hitInfo;
    private Ray ray;

    private Computer compInfo;

    private MeshRenderer computerMeshRend;

    #endregion

    /// <summary>
    /// Check if the user has clicked, if they have then raycast out from that position
    /// and see if they are clicking on a computer object
    /// </summary>
    void Update ()
    {
        // If the user left clicks...
        if (Input.GetMouseButtonDown(0))
        {
            // Make a ray going out towards where the mouse is  
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If we hit something witht that ray AND it was a computer...
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.CompareTag("Comp"))
            {
                // Get that object's computer componenet
                Debug.Log("You hit a computer!");
            }
        }
        
    }

}
