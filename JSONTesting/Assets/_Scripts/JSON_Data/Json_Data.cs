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
    public int took;            // How many samples is this?
    public bool timed_out;      // Did we time out?
    public Hits hits;       // The data that actually matters
}

[System.Serializable]
public class Hits
{
    public int total;           // How many total hits do we have on the server?
    public HitsData[] hits;  // Array of all the hits that we gathered since last time
}

[System.Serializable]
public class HitsData
{
    public string _index;
    public string _type;
    public string _id;
    public Source _source;      // This is the source of that hit
}

// This is really the information that I care about right now
[System.Serializable]
public class Source
{
    public SourceData source;
    public string transport;    // TCP, UDP, etc
    public string type;         // What type of traffic is it? (DNS, icmp, etc)
    public DestinationData dest;
    public string @timestamp;
}

[System.Serializable]
public class SourceData
{
    public int port;
    public string ip;           // The IP of the hit
}

[System.Serializable]
public class DestinationData
{
    public int port;
    public string ip;           // The IP of the hit
}

