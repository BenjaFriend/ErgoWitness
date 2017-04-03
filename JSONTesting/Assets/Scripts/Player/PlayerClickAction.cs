using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Detect if the user is looking at a PC, and if they are then show
/// the information of that PC in the HUD. This extends moniotr object
/// so that it can make web requests
/// </summary>
public class PlayerClickAction : MonitorObject {

    #region Fields
    private Json_Data dataObj;  // The type of data that we will query 

    private RaycastHit hitInfo; // The hit info of the raycast
    private Ray ray;            // The raycast object

    private string comp_ip;   // The ID of the computer we want to get info about

    private MeshRenderer computerMeshRend;

    #endregion

    /// <summary>
    /// Check if the user has clicked, if they have then raycast out from that position
    /// and see if they are clicking on a computer object
    /// </summary>
    void Update()
    {
        // If the user left clicks...
        if (Input.GetMouseButtonDown(0))
        {
            // Make a ray going out towards where the mouse is  
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If we hit something witht that ray AND it was a computer...
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.CompareTag("Comp"))
            {
                // Get the computer's source IP, and generate a term query based on that
                comp_ip = hitInfo.collider.gameObject.GetComponent<Computer>().SourceInfo.id_orig_h;

                // Start the monitor
                StartMonitor();

            }
        }
    }

    /// <summary>
    /// This will change what query we use for this
    /// </summary>
    /// <returns>A custom query that </returns>
    public override string BuildQuery()
    {
        // Build the query with the Top + ip of interest + the bottom
        return Query_Top + "\"" + comp_ip + "\"" + Query_Bottom;
    }

    public override void StartMonitor()
    {
        // Make sure that the FSM knows we are starting again
        base.StartMonitor();

        // Send it packetbeat data
        dataObj = new Json_Data();

        // Start the finite satate machine for the web request
        StartCoroutine(FSM(dataObj));
    }

    /// <summary>
    /// Cast the data how it needs to be for me to check it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public override void CheckRequestData<T>(T data)
    {
        if (typeof(T) == typeof(Json_Data))
        {
            // Cast the object as necessary
            dataObj = data as Json_Data;
            // Check the data that we want to
            CheckData(dataObj);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// This is where I will put the data in the UI
    /// </summary>
    /// <param name="dataObject"></param>
    private void CheckData(Json_Data dataObject)
    {
        // Make sure that our data is not null
        if (dataObject.hits.hits.Length == 0)
        {
            return;
        }


        // ============= Keep track of stuff to prevent duplicates ===============

        // Set our latest packetbeat time to the most recent one
        _latest_time = dataObject.hits.hits[0]._source.runtime_timestamp;

        // Send the data to the game controller for all of our hits
        for (int i = 0; i < dataObject.hits.hits.Length; i++)
        {
            // Display the data
            Debug.Log(dataObject.hits.hits[i]._id);
        }
        
        // Stop the monitor, because we only want 1 query
        StopMonitor();
    }

}
