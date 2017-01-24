using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct JSONDataObject
{
    public bool timed_out;

    [System.Serializable]
    public struct Shards
    {
        public int total;
        public int successful;
    }

    public struct Hits
    {

    }
}
