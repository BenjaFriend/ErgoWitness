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
public class Packetbeat_Json_Data
{
    public HitsParent_Packet hits;       // The data that actually matters
}

[System.Serializable]
public class HitsParent_Packet
{
    public HitsData_Packet[] hits;  // Array of all the hits that we gathered since last time
}

[System.Serializable]
public class HitsData_Packet
{
    public string _id;
    public string _type;

    //public string @timestamp;
    public Source_Packet _source;
}

// This is really the information that I care about right now
[System.Serializable]
public class Source_Packet
{
    // Packetbeat stuff
    public string runtime_timestamp;
    public string transport;
    public DestinationData_Packetbeat dest;
    public SourceData_Packetbeat packet_source;

    public string service;
    public string proto;

    public int sourceIpInt;
    public int destIpInt;
}

[System.Serializable]
public class SourceData_Packetbeat
{
    public int port;
    public string ip;
    public string mac;
}

[System.Serializable]
public class DestinationData_Packetbeat
{
    public int port;
    public string ip;
    public string mac;
}
