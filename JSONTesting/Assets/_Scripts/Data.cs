using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public int took;
    public bool timed_out;
    public ShardsData _shards;
    public HitsData hits;
}

[System.Serializable]
public class ShardsData
{
    public int total;
    public int successful;
    public int failed;
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
