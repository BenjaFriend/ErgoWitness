using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// This class is a representation of the JSON data that I get from my ELK
/// server. For Unity's JsonUtility to work properly, you need to set up a C# 
/// equivelent of the JSON data file. All classes must be serializable for the
/// Unity serializer to be able to work. This is much faster then most other 
/// JSON libraries because of the way that it allocates memory.
/// </summary>
[System.Serializable]
public class Json_Data
{
    public HitsParent hits;       // The data that actually matters
}

[System.Serializable]
public class HitsParent
{
    public HitsData[] hits;  // Array of all the hits that we gathered since last time
}

[System.Serializable]
public class HitsData
{
    public string _type;    // The type of data that this is
    public string _id;      // unique ID of the ojbect in our data base
    public Source _source; 
}

// This is really the information that I care about right now
[System.Serializable]
public class Source
{
    public string runtime_timestamp;    // the timestamp of the object
    public int id_orig_p;               // source port
    public string id_orig_h;            // source ip
    public string id_resp_h;            // dest ip
    public int id_resp_p;               // dest port

    public string service;      // dns, dhcp, etc.
    public string proto;        // udp, tcp, so on

    public int sourceIpInt;     // Bit converted integer for the source IP
    public int destIpInt;       // Bit converted integer for the dest IP
}

