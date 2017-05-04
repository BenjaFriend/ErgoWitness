using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Snort_Data
{
    public HitsParent_Snort hits;

}

[System.Serializable]
public class HitsParent_Snort
{
    public HitsData_Snort[] hits;  // Array of all the hits that we gathered since last time
}

[System.Serializable]
public class HitsData_Snort
{
    public Source_Snort _source;
}

public class Source_Snort
{
    // Packetbeat stuff
    public string runtime_timestamp;    // The timestamp of this object
    public string sourceIP;
    public string destIP;

    public string type;     // The type of alert that this is

    public int sourceIpInt; // A bit-conversion of the source ip string
    public int destIpInt;   // A bit conversion of the dest ip string

}