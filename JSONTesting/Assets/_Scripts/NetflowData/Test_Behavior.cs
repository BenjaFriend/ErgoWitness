using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will just send a new netflow object
/// to the given source and destination so that I can mess
/// with different trails and speed and what not
/// </summary>
public class Test_Behavior : MonoBehaviour {

    #region Fields

    public Transform destination;
    public Transform source;

    public GameObject netflowPrefab;
    public float frequency = 1f;

    #endregion

    /// <summary>
    /// Spawn a netflow object every time the frequency field
    /// is reached
    /// </summary>
    void Start ()
    {
        // Invoke the spawning whenever the amount of time is reached
        InvokeRepeating("SpawnNetflow", frequency, frequency);
	}
	
    /// <summary>
    /// instantiate a netflow object at it's source 
    /// </summary>
    void SpawnNetflow()
    {
        // Create a temp prefab of the netflow prefab
        GameObject temp = (GameObject)Instantiate(netflowPrefab);
        // Get the netflow component of that temp ojbect
        NetflowObject tempNet = temp.GetComponent<NetflowObject>();
        // Set the source of the netflow object
        tempNet.SourcePos = source;
        // Set the destination of the netflow object, which will also start it moving
        tempNet.DestinationPos = destination;
    }

}
