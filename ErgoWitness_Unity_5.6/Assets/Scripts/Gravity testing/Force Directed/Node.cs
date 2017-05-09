using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is for me testing how to make a force directed
/// graph in unity... 
/// 
/// Really just need a joint connecting the source to the destinatoin, and a way to grab it with the mouse
/// </summary>
[RequireComponent (typeof(Rigidbody))]
public class Node : MonoBehaviour {

    private Rigidbody rb;

    private List<HingeJoint> joints;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();

    }

    /// <summary>
    /// Add a joint connection to this object
    /// 
    /// When I get a new connection, then I want to 
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="connectingBody"></param>
    public void AddConnection(Rigidbody connectingBody)
    {
        // Add a line renderer componeent to this new connection

    }

    /// <summary>
    /// When the user clicks and drags on this object, then move it around 
    /// </summary>
    private void OnMouseDrag()
    {
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        //transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
        rb.MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen)));
    }

    // I need to draw a connection to every joint that we have

}
