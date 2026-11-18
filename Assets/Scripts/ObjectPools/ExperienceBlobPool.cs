using System.Collections.Generic;
using UnityEngine;

public class ExperienceBlobPool : MonoBehaviour
{

    public static ExperienceBlobPool SharedInstance;
    public Queue<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
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
    //     An inactive GameObject from the pool, or a newly created one if the pool is exhausted.
    public GameObject GetPooledObject()
    {
        // Blobs left uncollected stay out of the pool, so grow it instead of failing when it runs dry.
        if (pooledObjects.Count == 0 || pooledObjects.Peek().activeInHierarchy)
        {
            GameObject extra = Instantiate(objectToPool);
            extra.SetActive(false);
            return extra;
        }
        return pooledObjects.Dequeue();
    }

    // Returns the specified GameObject into the pooledObjects queue.
    public void ReturnToPool(GameObject gameObject)
    {
        pooledObjects.Enqueue(gameObject);
    }
    public void ConsumeObject(GameObject blob)
    {
        blob.SetActive(false);
        ReturnToPool(blob);
    }
    public void SpawnObject(Transform transform, float experienceMultiplier)
    {
        GameObject blob = GetPooledObject();
        blob.transform.position = transform.position;
        blob.GetComponent<ExperienceBlob>().experience = experienceMultiplier;
        blob.SetActive(true);
    }
}
