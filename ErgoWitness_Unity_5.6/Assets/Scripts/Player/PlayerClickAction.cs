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

    public Text ip;
    public Text port;
    public Text dest;
    public Text proto;   

    private Json_Data dataObj;  // The type of data that we will query 

    private RaycastHit hitInfo; // The hit info of the raycast
    private Ray ray;            // The raycast object

    private string comp_ip;     // The ID of the computer we want to get info about

    #endregion

    /// <summary>
    /// Check if the user has clicked, if they have then raycast out from that position
    /// and see if they are clicking on a computer object
    /// </summary>
    void Update()
    {
        // If the user left clicks ...
        if (Input.GetMouseButtonDown(0))
        {
            // Make a ray going out towards where the mouse is  
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If we hit something witht that ray AND it was a computer...
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.CompareTag("Comp"))
            {
                // Toggle the alert messages
                hitInfo.collider.gameObject.GetComponent<Computer>().ToggleAlertMessages();
                // Clear the text of the UI elements
                ClearText();

                // Get the string conversion of that IP
                comp_ip = IpIntToString(hitInfo.collider.gameObject.GetComponent<Computer>().SourceInt); 
                // Set the text to tell the user which IP this is
                ip.text = comp_ip;

                // Start the monitor
                StartMonitor();
            }
        }
    }

    public override void StartMonitor()
    {
        // Make sure that the FSM knows we are starting again
        base.StartMonitor();

        // Send it packetbeat data
        dataObj = new Json_Data();

        // Start the finite satate machine for the web request
        StartCoroutine(FSM(dataObj));

        // TODO: Start the loading animation
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

    /// <summary>
    /// Cast the data how it needs to be for me to check it
    /// </summary>
    /// <typeparam name="T">The type of JSON data that we want to look at</typeparam>
    /// <param name="data">The serialized JSON data</param>
    public override void CheckRequestData<T>(T data)
    {
        // If the type of this data is of the type that we are interested in...
        if (typeof(T) == typeof(Json_Data))
        {
            // Cast the object as necessary
            dataObj = data as Json_Data;
            // Check the data that we want to
            CheckData(dataObj);
        }
        // If this data is not the type that we care about...
        else
        {
            // Stop the monitor
            StopMonitor();
            // Return out of this method
            return;
        }
    }

    /// <summary>
    /// This is where I will put the data in the UI
    /// </summary>
    /// <param name="dataObject"></param>
    private void CheckData(Json_Data dataObject)
    {
        // If the datat hat we got has nothing in it...
        if (dataObject.hits.hits.Length == 0)
        {
            // Dispaly error text
            proto.text = "Not Found";
            port.text = "Not Found";
            dest.text = "Not Found";
            // Return out of this
            return;
        }

        // Stop the Loading animation if we need one

        // Set our latest packetbeat time to the most recent one
        _latest_time = dataObject.hits.hits[0]._source.runtime_timestamp;

        // Send the data to the game controller for all of our hits
        for (int i = 0; i < dataObject.hits.hits.Length; i++)
        {
            // Display the data
            proto.text = dataObject.hits.hits[i]._source.proto;
            //port.text = dataObject.hits.hits[i]._source.id_orig_p.ToString();
            dest.text = dataObject.hits.hits[i]._source.id_resp_h;
        }
        
        // Stop the monitor, because we only want 1 query
        StopMonitor();
    }

    /// <summary>
    /// Ovverride of the normal log data method in order to tell the
    /// user that the specific IP was not found in our bro database
    /// </summary>
    /// <param name="log">The that we got from something</param>
    /// <param name="filelocation">The file locatoin of where we want to log that data</param>
    public override void LogData(string log, string filelocation)
    {
        base.LogData(log, filelocation);

        // Dispaly error text
        proto.text = "Not Found";
        port.text = "Not Found";
        dest.text = "Not Found";
    }

    /// <summary>
    /// Clears the text elements
    /// </summary>
    private void ClearText()
    {
        proto.text = "";
        port.text = "";
        dest.text = "";
    }

    /// <summary>
    /// Take an integer in, and return it as an IP address
    /// in string format.
    /// </summary>
    /// <param name="ipAddrInt">The integer version of the IP we want</param>
    /// <returns></returns>
    private string IpIntToString(int ipAddrInt)
    {
        // Make sure that this ip is valid
        if (ipAddrInt == 0) return null;
        // Return the bit converted string
        return new System.Net.IPAddress(System.BitConverter.GetBytes(ipAddrInt)).ToString();
    }

}
