using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is going to be like the computer script, but for netflow
/// data. This object will be pooled, like bullets in the Unity
/// example. So I need a script that will have it be activated/
/// deactivated on enable and disable
/// </summary>
[RequireComponent(typeof(TrailRenderer))]
public class NetflowObject : MonoBehaviour {

    // The different colors for the protocols
    public Material tcpMat;
    public Material udpColor;
    public Material httpColor;
    public Material httpsColor;
    public Material defaultColor;
    public float smoothing = 10f;       // How fast do we want to shoot this thing

    private Transform source;
    private Transform destinaton;
    private string protocol;
    private TrailRenderer trailRend;

    /// <summary>
    /// On set this changes the current position to source position
    /// </summary>
    public Transform Source
    {
        get { return source; }
        set
        {
            source = value;
            // Move to this positon
            transform.position = source.position;
            // Set as active
            gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// On set this will start to move this object with a coroutine so that nothing else needs to happen
    /// </summary>
    public Transform Destination
    {
        get { return destinaton; }
        set
        {
            destinaton = value;

            // Stop the coroutine and start it again with a new destination
            StopCoroutine("MoveToDestination");
            StartCoroutine("MoveToDestination");

        }
    }
    /// <summary>
    /// This sets the color, and if you set it then it changes the material of the line renderer
    /// </summary>
    public string Protocol
    {
        get { return protocol; }
        set
        {
            protocol = value; SetColor();
        }
    }

    /// <summary>
    /// Get the line renderer component
    /// </summary>
    void Awake ()
    {
        trailRend = GetComponent<TrailRenderer>();	
	}

    /// <summary>
    /// This is how I am gonna move between the source and destion
    /// </summary>
    /// <returns></returns>
	public IEnumerator MoveToDestination()
    {
        while (Vector3.Distance(transform.position, destinaton.position) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, destinaton.position, smoothing * Time.deltaTime);

            yield return null;
        }
    }

    /// <summary>
    /// Set the color of this based on our protocol
    /// </summary>
    private void SetColor()
    {
        Material trail = trailRend.material;

        switch (protocol)
        {
            case ("tcp"):
                trailRend.material = tcpMat;
                break;
            case ("udp"):
                trailRend.material = udpColor;

                break;
            case ("http"):
                trailRend.material = httpColor;

                break;
            case ("https"):
                trailRend.material = httpsColor;

                break;
            default:
                trailRend.material = defaultColor;

                break;
        }

    }
}
