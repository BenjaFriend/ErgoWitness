using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Ben Hoffman
/// Purpose of classes: This is the outline of all of the JSON data that I will need
/// in order to get the things that I want.
/// </summary>
[System.Serializable]
public class Data
{
    public bool timed_out;
    public HitsData hits;
}

[System.Serializable]
public class HitsData
{
    public int total;
    public float max_score;
    public HitsAuxData[] hits;
}

[System.Serializable]
public class HitsAuxData
{
    public string _index;
    public string _type;
    public Source _source; 
}

// This is really the information that I care about right now
[System.Serializable]
public class Source
{
    public string method;
    public int bytes_in;
    public string ip;
    public string id;
    public string transport;
    public string type;
    public string @timestamp;
    public int port;
    public string client_ip;
}
