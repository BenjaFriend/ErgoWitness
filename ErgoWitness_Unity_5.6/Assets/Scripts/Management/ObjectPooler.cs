using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic method of pooling objects
/// </summary>
public class ObjectPooler : MonoBehaviour {

    public GameObject pooledObject;     // The object that we are pooing
    [Tooltip("How many of the pooled object prefabs will be instantiated on load.")]
    public int pooledAmount = 20;       // How much we want to initially pool
    public bool willGrow = true;        // If true then this pooler will grow until it reaches the max value

    private List<GameObject> pooledObjects;

    /// <summary>
    /// Instantiate the pool list, and create all of the prefabs into that list
    /// </summary>
    void Start ()
    {
        // Instantiate the pool list
        pooledObjects = new List<GameObject>();

        // Loop through the specified number of times and create that many prefab instances
        for(int i = 0; i < pooledAmount; i++)
        {
            // Instantiate a prefab instance
            GameObject tempObj = (GameObject)Instantiate(pooledObject);
            // Set the object as false
            tempObj.SetActive(false);
            // Add that object to the list
            pooledObjects.Add(tempObj);
        }
	}

    /// <summary>
    /// Return the first in-active object in the pool,
    ///  or grow the pool and return that object
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledObject()
    {
        // Loop through and return the first in-active object
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            // If this object in inactive: 
            if (!pooledObjects[i].activeInHierarchy)
            {
                // Return that object
                return pooledObjects[i];
            }
        }

        // Grow only if we are told that we should grow, and are not exceeding the max
        if (willGrow)
        {
            // Create a new instance of the prefab
            GameObject obj = (GameObject)Instantiate(pooledObject);
            // Add it to the pool
            pooledObjects.Add(obj);
            // return that object
            return obj;
        }

        // Otherwise return null
        return null;
    }
	
}
