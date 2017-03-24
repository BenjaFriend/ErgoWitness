using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class will be put on any objects that will be pooled.
/// The reason for this is because 'gameObject.SetActive()' 
/// actually creates a pretty large amount of garbage collection,
/// and even frame drops when things happen. 
/// </summary>
public class PooledObject : MonoBehaviour {

    private bool _isActive;
    public bool _IsActive { get { return _isActive; } }

    /// <summary>
    /// Set this object as active
    /// </summary>
	public void SetPooledActive()
    {
        _isActive = true;
    }

    /// <summary>
    /// Set this object as inactive, and put it far far away
    /// </summary>
    public void SetPooledInActive()
    {
        // Set this object as false
        _isActive = false;

        // Move this object very far away
        transform.position = new Vector3(5000, 5000, 5000);
    }


}
