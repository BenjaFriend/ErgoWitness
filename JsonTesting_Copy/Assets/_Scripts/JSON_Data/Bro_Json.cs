using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bro_Json
{
    public string ts;
    public string uid;
    public string id_orig_h;
    public int id_orig_p;
    public string id_resp_h;
    public int id_resp_p;
    public string proto;
    public string service;
    public float duration;
    public string conn_state;
    public int missed_bytes;
    public int orig_pkts;
    public int orig_ip_bytes;
    public int resp_pkts;
    public int resp_ip_bytes;
}
