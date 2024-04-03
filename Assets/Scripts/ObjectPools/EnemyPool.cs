using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour, IObjectPool
{
    public static IObjectPool SharedInstance;
    public Queue<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }
    void Start()
    {
        InitializePool();
    }

    // Initializes the object pooling system by instantiating a specified number of GameObjects and adding them to a queue for future use.
    public void InitializePool()
    {
        pooledObjects = new Queue<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Enqueue(tmp);
        }
    }

    // Retrieves an inactive GameObject from the object pool.
    //
    // Returns:
    //     The inactive GameObject retrieved from the object pool, or null if none are available.
    public GameObject GetPooledObject()
    {
        if (pooledObjects.Count > 0)
        {
            if (!pooledObjects.Peek().activeInHierarchy)
            {
                return pooledObjects.Dequeue();
            }
        }
        return null;
    }

    // Returns the specified GameObject into the pooledObjects queue.
    public void ReturnToPool(GameObject gameObject)
    {
        pooledObjects.Enqueue(gameObject);
    }
}
