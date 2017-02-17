using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic method of pooling objects
/// </summary>
public class ObjectPooler : MonoBehaviour {

    public GameObject pooledObject;
    public int pooledAmount = 20;
    public bool willGrow = true;
    public int maxGrowth = 200;

    private List<GameObject> pooledObjects;

    void Start ()
    {
        pooledObjects = new List<GameObject>();
        for(int i = 0; i < pooledAmount; i++)
        {
            GameObject tempObj = (GameObject)Instantiate(pooledObject);
            tempObj.SetActive(false);
            pooledObjects.Add(tempObj);
        }
	}

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        // Grow only if we are told that we should grow, and are not exceeding the max
        if (willGrow && pooledObjects.Count <= maxGrowth)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }
	
}
