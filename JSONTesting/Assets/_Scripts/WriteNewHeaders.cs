using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// This is a class that will change the timestamp in 
/// the header files for my queries. This will allow me to always
/// have the most recent timestamp for my visualisation 
/// </summary>
public class WriteNewHeaders : MonoBehaviour {

    public static WriteNewHeaders currentWriter;
    public int lineToEdit = 5;

    private string sourceFile_Packetbeat;
    private string sourceFile_Filebeat;

    private string lastTimestamp_Packetbeat;
    private string lastTimstamp_Filebeat;

    /// <summary>
    /// Returns the string value,
    /// Or sets the string value and writes to the file
    /// </summary>
    public string LastTimestamp_Packetbeat
    {
        get { return lastTimestamp_Packetbeat; }
        set
        {
            // Set the value of the packetbeat timestamp
            lastTimestamp_Packetbeat = value;
            // Rewrite the file
            WriteHeaders(lastTimestamp_Packetbeat, sourceFile_Packetbeat, lineToEdit);
        }
    }

    /// <summary>
    /// Returns the string value,
    /// Or sets the string value and writes to the file
    /// </summary>
    public string LastTimestamp_Filebeat
    {
        get { return LastTimestamp_Filebeat; }
        set
        {
            lastTimstamp_Filebeat = value;
            WriteHeaders(lastTimstamp_Filebeat, sourceFile_Filebeat, lineToEdit);
        }
    }

    /// <summary>
    /// Set the file locations that we have 
    /// </summary>
    void Start ()
    {
        currentWriter = this;
        // Set the source file location
        sourceFile_Filebeat = Application.streamingAssetsPath + "/bro_Headers.json";
        sourceFile_Packetbeat = Application.streamingAssetsPath + "/packetbeat_Headers.json";
    }
	

    private void WriteHeaders(string newLineText, string fileLocation, int lineToEdit)
    {
        // Send the file to a string array, where each element is one line
        string[] arrLine = File.ReadAllLines(fileLocation);
        // set the line we want to edit to the new line of text
        arrLine[lineToEdit - 1] = "\"gt\":" + "\"" + newLineText + "\"";
        // Write that file to the specified location
        File.WriteAllLines(fileLocation, arrLine);
        
    }

}
