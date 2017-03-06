using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will move whatever object that is is on
/// in a sphereical pattern, zooming in on some of the 
/// new PC's and then slowyl zooming out
/// </summary>
public class Automated_Camera : MonoBehaviour {

    public static Automated_Camera currentAutoCam;

    public float speed = 1f;       // How fast do we want to shoot this thing
    private Vector3 targetpos;
    private Vector3 newPos;

	/// <summary>
    /// Set up the target position and look at it
    /// </summary>
	void Start ()
    {
        // Set the reference
        currentAutoCam = this;
        // Set the target position
        targetpos = Vector3.zero;
        transform.LookAt(targetpos);
    }

    /// <summary>
    /// Move the camera aroudn the target
    /// </summary>
    void Update ()
    {
        //transform.LookAt(targetpos);
        //transform.Translate(Vector3.right * Time.deltaTime * speed);
        transform.RotateAround(targetpos, new Vector3(0.0f, 1.0f, 0.0f), 20 * Time.deltaTime * speed);
    }

    /// <summary>
    /// Set the new target position and look at it
    /// </summary>
    /// <param name="newTarget">The new target to set</param>
    public void ChangeTarget(Vector3 newTarget)
    {
        // Set the new target
        targetpos = newTarget;

        // Look at the new target
        transform.LookAt(targetpos);
    }

    public void ChangeRadius(float radius)
    {
        newPos = transform.position;
        newPos.z += radius - Mathf.Abs(newPos.z);

        // Move the object back farther
        MoveToDestination(newPos);
    }


    public IEnumerator MoveToDestination(Vector3 destination)
    {
        // Break if our destination is null
        if (destination == null)
        {
            yield break;
        }

        while (Vector3.Distance(transform.position, destination) > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);

            yield return null;
        }
    }


}
