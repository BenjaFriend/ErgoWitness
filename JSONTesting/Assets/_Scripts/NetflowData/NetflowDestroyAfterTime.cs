using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set this object to inactive after the specified lifetime
/// This i for the object pooler so that it doesnt get destroied 
/// each time
/// </summary>
public class NetflowDestroyAfterTime : MonoBehaviour {

    public float lifetime = 5f; // How long this object will stay active

    void OnEnable()
    {
        // Destroy after 2 seconds
        Invoke("Destroy", lifetime);
    }

    public void Destroy()
    {
        // Set this object to in-active
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // Cancel any invokes we may have
        CancelInvoke();
    }

}
