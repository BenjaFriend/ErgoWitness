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
    public string _index;
    //public string _type;
    //public string _id;
    public Source _source; 
}

// This is really the information that I care about right now
[System.Serializable]
public class Source
{
    // Packetbeat stuff
    public string transport;
    public DestinationData_Packetbeat dest;
    public SourceData_Packetbeat packet_source;

    // Snort stuff will show here
    //public int offset;
    //public string packet_data;
    //public string input_type;
    //public string source;
    
    // Bro stuff is here
    //public int resp_pkts;
    public int id_orig_p;
    //public float duration;
    //public string uid;
    public string id_orig_h;
    public string conn_state;
    public string id_resp_h;
    public int id_resp_p;
    //public int resp_ip_bytes;
    //public int orig_bytes;
    //public int orig_ip_bytes;
    //public int orig_pkts;
    //public int missed_bytes;

    // Both snort and bro have a message
    //public string message;

    // More snort stuff
    //public float packet_event_second;
    //public int packet_sensor_id;
    //public int packet_length;
    //public float packet_second;
    public bool alert;
    //public int packet_event_id;

    // More bro stuff
    //public int resp_bytes;
    public string service;
    public string proto;
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
