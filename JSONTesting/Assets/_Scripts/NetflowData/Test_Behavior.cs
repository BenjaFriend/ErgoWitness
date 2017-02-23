using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will just send a new netflow object
/// to the given source and destination so that I can mess
/// with different trails and speed and what not
/// </summary>
public class Test_Behavior : MonoBehaviour {

    public Transform destination;
    public Transform source;

    public GameObject netflowPrefab;
    public float frequency = 1f;

	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("SpawnNetflow", frequency, frequency);
	}
	
    void SpawnNetflow()
    {
        GameObject temp = (GameObject)Instantiate(netflowPrefab);
        NetflowObject tempNet = temp.GetComponent<NetflowObject>();
        tempNet.Source = source;
        tempNet.Destination = destination;
        Destroy(temp, 5f);
    }

}
