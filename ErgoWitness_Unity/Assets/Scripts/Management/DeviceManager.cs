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
[RequireComponent(typeof(ObjectPooler))]
public class DeviceManager : MonoBehaviour {

    #region Fields
    public static DeviceManager currentDeviceManager;

    public StreamingInfo_UI streamingInfo;
    public Text deviceCountText;        // How many devices are there currently?
    public ObjectPooler computerPooler; // The object pooler for the computer prefab

    private static Dictionary<int, Computer> computersDict; // A dictionary of all the computers I have

    public static Dictionary<int, Computer> ComputersDict { get { return computersDict; } }
    #endregion

    /// <summary>
    /// Make sure that this is the only object of this type in the scene
    /// </summary>
    void Awake ()
    {
        // Make sure tha thtis is the only one of these objects in the scene
        if (currentDeviceManager == null)
        {
            // Set the currenent referece
            currentDeviceManager = this;
        }
        else if (currentDeviceManager != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        // Initialize the dictionary
        computersDict = new Dictionary<int, Computer>();

        // Set the text
        deviceCountText.text = "0";

        // Get the object pooler
        computerPooler = GetComponent<ObjectPooler>();
    }

    /// <summary>
    /// Checks if we know of this device already. 
    /// If we do NOT, then call NewComputer()
    /// If we DO, call CheckConnections()
    /// </summary>
    /// <param name="jsonSourceData">The source data that we are checking</param>
    public void CheckIp(Source jsonSourceData)
    {
        // If we know of the source IP already:
        if (CheckDictionary(jsonSourceData.sourceIpInt))
        {
            // I want to check if there is a connection that I should add
            CheckConnection(jsonSourceData);

            // Add more life to the computer that we saw
            computersDict[jsonSourceData.sourceIpInt].AddHit();
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
        // Get a new computer device from the object pooler
        Computer newDevice = computerPooler.GetPooledObject().GetComponent<Computer>();

        // Set the DATA on this gameobject to the data from the JSON data
        //newDevice.SourceInfo = jsonSourceData;
        newDevice.SourceInt = jsonSourceData.sourceIpInt;
        newDevice.DestInt = jsonSourceData.destIpInt;

        // Set this object as active in the hierachy so that you can actually see it
        newDevice.gameObject.SetActive(true);

        // Add the object to the dictionary
        computersDict.Add(jsonSourceData.sourceIpInt, newDevice);

        // Check the connections to this, if there are connections then add them to it's list
        CheckConnection(jsonSourceData);

        // Check if we can add it to a group
        IPGroupManager.currentIpGroups.CheckGroups(jsonSourceData.sourceIpInt);

        // Update the UI that tells us how many devices there are
        deviceCountText.text = computersDict.Count.ToString();

        // Send it to the streaming UI thing
        streamingInfo.AddInfo(jsonSourceData);

        // If there is a service runnign on this, then send it to the netflow controller to visualize it
        if (jsonSourceData.service != null)
        {
            ConnectionController.currentNetflowController.CheckPacketbeatData(jsonSourceData.sourceIpInt, jsonSourceData.destIpInt, jsonSourceData.service);
        }
    }

    /// <summary>
    /// Check if the destination IP of this source data is in our dictionary.
    /// If it is, then add the connection to each computer's list.
    /// If not, then add it as a new computer. 
    /// </summary>
    /// <param name="source">This source data has a SOURCE IP that we know
    /// of already, and we want to check the destination to see if we know
    /// of that or not.</param>
    private void CheckConnection(Source source)
    {
        // If this source is 0, then we want to get rid of it
        if(source.sourceIpInt == 0 || source.destIpInt == 0)
        {
            return;
        }

        // If we do NOT know of the destination computer, then add that
        if (!CheckDictionary(source.destIpInt))
        {
            // We DO NOT have the responding IP on our network, so add it.
            // Make a new source object
            Source newSource = new Source();

            // Set the NEW source's original IP to the response IP of the other one
            newSource.id_orig_h = source.id_resp_h;
            newSource.sourceIpInt = source.destIpInt;

            // Set the NEW source's orig. Port to the response port of the other one
            newSource.destIpInt = 0;

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
        // If we contrain this IP address, then return the transform
        if(computersDict.ContainsKey(IP))
            return computersDict[IP].transform;

        // Otherwise return null
        return null;
    }

    /// <summary>
    /// Check if our dictionary comtaints this IP
    /// </summary>
    /// <param name="IP">The IP that we want to know if we have</param>
    /// <returns>True if we have this key</returns>
    public bool CheckDictionary(int IpInt)
    {
        // return if we contrain this address or not
        return computersDict.ContainsKey(IpInt);
    }

}
