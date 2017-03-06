using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Ben Hoffman
/// This class will do a lot of important things. 
/// It will have a way to STORE all my current 'computers' on the network
/// It will have a method to check if a
/// </summary>
public class DeviceManager : MonoBehaviour {

    #region Fields
    public static DeviceManager currentDeviceManager;

    public StreamingInfo_UI streamingInfo;
    public Text deviceCountText;        // How many devices are there currently?
    public ObjectPooler computerPooler; // The object pooler for the computer prefab

    //private GameObject obj;             // Use this as a temp calculation variable for better memory
    private static Dictionary<int, Computer> computersDict; // A dictionary of all the computers I have

    public static Dictionary<int, Computer> ComputersDict { get { return computersDict; } }
    #endregion

    void Awake ()
    {
        // Set the static reference
        currentDeviceManager = this;

        // Initialize the dictionary
        computersDict = new Dictionary<int, Computer>();

        // Set the text
        deviceCountText.text = "0";
    }

    /// <summary>
    /// Checks if we know of this device already. 
    /// If we do NOT, then call NewComputer()
    /// If we DO, call CheckConnections()
    /// </summary>
    /// <param name="jsonSourceData">The source data that we are checking</param>
    public void CheckIp(Source jsonSourceData)
    {
        // Make sure that we have something in the array
        if (jsonSourceData == null || jsonSourceData.sourceIpInt == 0)
        { 
            return;
        }

        // If we know of the source IP already:
        if (CheckDictionary(jsonSourceData.sourceIpInt))
        {
            // I want to check if there is a connection that I should add
            CheckConnections(jsonSourceData);
        }
        else
        {
            // If I do NOT have this IP in my dictionary, then make a new computer        
            NewComputer(jsonSourceData);
        }
    }


    /// <summary>
    /// We have NOT seen this IP before, and we want to make
    /// a new one
    /// Author: Ben Hoffman
    /// </summary>
    public void NewComputer(Source jsonSourceData)
    {
        // If this is null or we already have the IP somehow...
        if(jsonSourceData.id_orig_h == null || computersDict.ContainsKey(jsonSourceData.sourceIpInt))
        {
            return;
        }

        // Get a temporary object from the object pooler
        GameObject newTempObj = computerPooler.GetPooledObject();
    
        // If the object is null then break out of this
        if (newTempObj == null) return;

        Computer newDevice = newTempObj.GetComponent<Computer>();

        // Set the DATA on this gameobject to the data from the JSON data
        newDevice.SourceInfo = jsonSourceData;

        // Set this object as active in the hierachy so that you can actually see it
        newTempObj.SetActive(true);

        // Add the object to the dictionary
        computersDict.Add(jsonSourceData.sourceIpInt, newDevice);

        // Check the connections to this, if there are connections then add them to it's list
        CheckConnections(jsonSourceData);

        // Check if we can add it to a group
        IPGroupManager.currentIpGroups.CheckGroups(jsonSourceData.sourceIpInt);


        // Update the UI that tells us how many devices there are
        deviceCountText.text = computersDict.Count.ToString();

        // Send it to the streaming UI thing
        streamingInfo.AddInfo(jsonSourceData);
    }


    /// <summary>
    /// Check if the destination IP of this source data is in our dictionary.
    /// If it is, then add the connection to each computer's list.
    /// If not, then add it as a new computer. 
    /// </summary>
    /// <param name="source">This source data has a SOURCE IP that we know
    /// of already, and we want to check the destination to see if we know
    /// of that or not.</param>
    private void CheckConnections(Source source)
    {
        // I need to check if my destination is a source  address, which would mean that it is in my dictionary already
        if (source.id_resp_h == null || source.id_resp_h == "Null")
        {
            return;
        }

        // If we fail to connect them, then create a new computer using the destination
        if (!ConnectComputers(source.sourceIpInt, source.destIpInt))
        {
            // We DO NOT have the responding IP on our network, so add it.
            // Make a new source object
            Source newSource = new Source();

            // Set the NEW source's original IP to the response IP of the other one
            newSource.id_orig_h = source.id_resp_h;
            // Set the NEW source's orig. Port to the response port of the other one
            newSource.id_orig_p = source.id_resp_p;

            // Set the integer data
            ManageMonitors.currentMonitors.SetIntegerValues(newSource);

            // Add this new computer to the network
            NewComputer(newSource);
        }
    }

    /// <summary>
    /// Simply get the transform of the given IP
    /// </summary>
    /// <param name="IP">The IP of the computer that we want to find</param>
    /// <returns>The transform of the computer</returns>
    public Transform GetTransform(int IP)
    {
        if (IP == 0)
        {
            return null;
        }

        if(computersDict.ContainsKey(IP))
            return computersDict[IP].transform;

        return null;
    }

    /// <summary>
    /// Check if our dictionary comtaints this IP
    /// </summary>
    /// <param name="IP">The IP that we want to know if we have</param>
    /// <returns>True if we have this key</returns>
    public bool CheckDictionary(int IpInt)
    {
        if(IpInt == 0)
        {
            return false;
        }

        return computersDict.ContainsKey(IpInt);
    }

    /// <summary>
    /// Add each computer to one another's connection if they exist in our
    /// dictionary
    /// </summary>
    /// <param name="ipA">The first IP address</param>
    /// <param name="ipB">The second IP address</param>
    public bool ConnectComputers(int ipA, int ipB)
    {
        if(CheckDictionary(ipA) && CheckDictionary(ipB))
        {
            // Add a connection from source to destination
            computersDict[ipA].GetComponent<Computer>().AddConnectedPC(computersDict[ipB]);
            // Add a connection from destination to source
            computersDict[ipB].GetComponent<Computer>().AddConnectedPC(computersDict[ipA]);
            // We did it successfuly, so return true
            return true;
        }

        // The computer's do not exist, so return false
        return false;

    }

}
