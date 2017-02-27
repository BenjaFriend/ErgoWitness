using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFromSourceToTarget : MonoBehaviour {

    public float smoothing = 10f;       // How fast do we want to shoot this thing

    private Transform sourcePos;           // Our starting point
    private Transform destinatonPos;       // The spot that we want to be at

    #region Mutators
    /// <summary>
    /// On set this changes the current position to source position
    /// </summary>
    public Transform SourcePos
    {
        get { return sourcePos; }
        set
        {
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
            destinatonPos = value;
            // Stop the coroutine and start it again with a new destination
            StopCoroutine("MoveToDestination");
            StartCoroutine("MoveToDestination");
        }
    }

    #endregion

    /// <summary>
    /// Co ourtine that is started when the 'Destination' 
    /// property is set. This lerps between the current position
    /// and the destination position. 
    /// </summary>
    /// <returns>Movement of this object towards the destiantion</returns>
    public IEnumerator MoveToDestination()
    {
        // Break if our destination is null
        if (destinatonPos == null || sourcePos == null)
        {
            yield break;
        }

        while (Vector3.Distance(transform.position, destinatonPos.position) > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, destinatonPos.position, smoothing * Time.deltaTime);

            yield return null;
        }
    }
}
