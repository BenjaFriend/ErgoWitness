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
    public string _id;
    public Source _source; 
}

// This is really the information that I care about right now
[System.Serializable]
public class Source
{
    public string runtime_timestamp;
    public int id_orig_p;
    public string id_orig_h;
    public string conn_state;
    public string id_resp_h;
    public int id_resp_p;
    public string service;
    public string proto;
    public int sourceIpInt;
    public int destIpInt;
}

