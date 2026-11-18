using System.Collections.Generic;
using UnityEngine;

// Bullets can be deactivated by a collision at any time (which also kills their movement coroutine),
// so instead of trusting every bullet to be handed back, the pool scans for any inactive instance
// and grows when the whole pool is in flight.
public class BulletProjectilePool : MonoBehaviour, IObjectPool
{
    public static IObjectPool SharedInstance;
    public GameObject objectToPool;
    public int amountToPool;

    private readonly List<GameObject> pooledObjects = new List<GameObject>();
    private int searchStart;

    void Awake()
    {
        SharedInstance = this;
        InitializePool();
    }

    public void InitializePool()
    {
        pooledObjects.Clear();
        for (int i = 0; i < amountToPool; i++)
        {
            pooledObjects.Add(CreateInstance());
        }
    }

    // Returns an inactive bullet, growing the pool if every bullet is currently in use.
    public GameObject GetPooledObject()
    {
        int count = pooledObjects.Count;
        // Start where the last search ended: the next slot is very likely free, so this is O(1) in practice.
        for (int i = 0; i < count; i++)
        {
            int index = (searchStart + i) % count;
            GameObject candidate = pooledObjects[index];
            if (candidate != null && !candidate.activeInHierarchy)
            {
                searchStart = (index + 1) % count;
                return candidate;
            }
        }

        GameObject extra = CreateInstance();
        pooledObjects.Add(extra);
        return extra;
    }

    public void ReturnToPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private GameObject CreateInstance()
    {
        GameObject tmp = Instantiate(objectToPool);
        tmp.SetActive(false);
        return tmp;
    }
}
