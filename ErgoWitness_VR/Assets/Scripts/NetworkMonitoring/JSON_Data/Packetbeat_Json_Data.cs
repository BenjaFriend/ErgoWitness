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
    public Source_Packet _source;   
}

// This is really the information that I care about right now
[System.Serializable]
public class Source_Packet
{
    // Packetbeat stuff
    public string runtime_timestamp;    // The timestamp of this object
    public string transport;    // This is where udp/tcp is specified

    public DestinationData_Packetbeat dest; 
    public SourceData_Packetbeat packet_source;

    public int sourceIpInt; // A bit-conversion of the source ip string
    public int destIpInt;   // A bit conversion of the dest ip string
}

[System.Serializable]
public class SourceData_Packetbeat
{
    public int port;        // Source port
    public string ip;       // Source IP
}

[System.Serializable]
public class DestinationData_Packetbeat
{
    public int port;        // Destination port
    public string ip;       // Destination IP
}
