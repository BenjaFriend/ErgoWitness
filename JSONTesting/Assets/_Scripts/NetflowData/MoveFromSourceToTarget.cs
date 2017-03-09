using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFromSourceToTarget : MonoBehaviour {

    public float smoothing = 10f;       // How fast do we want to shoot this thing

    private Transform sourcePos;           // Our starting point
    private Transform destinatonPos;       // The spot that we want to be at

    private IEnumerator movingRoutine;
    private bool hasArrived;
    #region Mutators

    /// <summary>
    /// On set this changes the current position to source position
    /// </summary>
    public Transform SourcePos
    {
        get { return sourcePos; }
        set
        {
            // If we are changing this, then we have not arrived yet
            hasArrived = false;

            sourcePos = value;
            // Move to this positon
            transform.position = sourcePos.position;
            // Set as active
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// On set this will start to move this object with a coroutine so that nothing else needs to happen
    /// </summary>
    public Transform DestinationPos
    {
        get { return destinatonPos; }
        set
        {
            // If we are changing this, then we have not arrived yet
            hasArrived = false;

            destinatonPos = value;
            // Stop the coroutine and start it again with a new destination
            if(movingRoutine != null)
            {
                StopCoroutine(movingRoutine);
            }

            movingRoutine = MoveToDestination();

            StartCoroutine(movingRoutine);
        }
    }

    public IEnumerator MovingRoutine { get { return movingRoutine; } }
    public bool HasArrived { get { return hasArrived; } }
    #endregion

    /// <summary>
    /// Co ourtine that is started when the 'Destination' 
    /// property is set. This lerps between the current position
    /// and the destination position. 
    /// </summary>
    /// <returns>Movement of this object towards the destiantion</returns>
    public IEnumerator MoveToDestination()
    {
        // Make sure we know that we have not arrived yet
        hasArrived = false;

        // If the source position is null, then assume we want to start from our current location
        if(SourcePos == null)
        {
            SourcePos = transform;
        }

        // Break if our destination is null
        if (destinatonPos == null)
        {
            yield break;
        }

        while (Vector3.Distance(transform.position, destinatonPos.position) > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, destinatonPos.position, smoothing * Time.deltaTime);

            yield return null;
        }
        hasArrived = true;
    }
}
