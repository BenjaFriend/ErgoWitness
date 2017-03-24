using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObjectPool : MonoBehaviour {



    public GameObject pooledObject;     // The object that we are pooing
    public int pooledAmount = 20;       // How much we want to initially pool
    public bool willGrow = true;        // If true then this pooler will grow until it reaches the max value

    private List<PooledObject> pooledObjects;

    /// <summary>
    /// Instantiate the pool list, and create all of the prefabs into that list
    /// </summary>
    void Awake()
    {
        // Instantiate the pool list
        pooledObjects = new List<PooledObject>();

        // Loop through the specified number of times and create that many prefab instances
        for (int i = 0; i < pooledAmount; i++)
        {
            // Instantiate a prefab instance
            PooledObject tempObj = Instantiate(pooledObject).GetComponent<PooledObject>();

            // Set this as inactive
            tempObj.SetPooledInActive();

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
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // If this object in inactive: 
            if (!pooledObjects[i]._IsActive)
            {
                pooledObjects[i].SetPooledActive();
                // Return that object
                return pooledObjects[i].gameObject;
            }
        }

        // Grow only if we are told that we should grow, and are not exceeding the max
        if (willGrow)
        {
            // Create a new instance of the prefab
            PooledObject obj = Instantiate(pooledObject).GetComponent<PooledObject>();
            // Add it to the pool
            pooledObjects.Add(obj);

            // Set this as active
            obj.SetPooledActive();

            // return that object
            return obj.gameObject;
        }

        // Otherwise return null
        return null;
    }



}
