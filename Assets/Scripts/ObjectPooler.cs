using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ObjectPooler : Singleton<ObjectPooler>
{
    #region Field Declarations

    [Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
    }

    [Header("Pool")]
    [SerializeField] private List<Pool> objectToPool;
    [HideInInspector] public List<GameObject> pooledObjects = new List<GameObject>();

    #endregion

    #region Startup

    // Start is called before the first frame update
    private void Start()
    {
        CreatePool();
    }

    #endregion

    #region Spawn Object

    private void CreatePool()//Create pool spawn objects
    {
        foreach (var pool in objectToPool)
        {
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                pooledObjects.Add(obj);
                obj.transform.SetParent(this.transform); // set as children of Spawn Manager
            }
        }
    }
    public GameObject GetPooledObject(string tag)// Loop through list of pooled objects,deactivating them and adding them to the list 
    {
        // For as many objects as are in the pooledObjects list
        while (true)
        {
            if (CheckStatePoolObjects())//if the pool object active, break circle
                break;
            int randomIndex = UnityEngine.Random.Range(0, pooledObjects.Count);
            // if the pooled objects is NOT active, return that object 
            if (!pooledObjects[randomIndex].activeInHierarchy && pooledObjects[randomIndex].CompareTag(tag))
            {
                return pooledObjects[randomIndex];
            }
        }
        return null;
    }

    private bool CheckStatePoolObjects()//Check state pool object for spawn
    {
        foreach (GameObject poolObject in pooledObjects)
        {
            if (!poolObject.activeSelf)
                return false;
        }
        return true;
    }

    #endregion
}
