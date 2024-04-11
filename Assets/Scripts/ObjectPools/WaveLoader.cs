using System.Collections.Generic;
using UnityEngine;

public class WaveLoader : MonoBehaviour
{
    private List<Wave> waves;
    // public static EnemyPool SharedInstance;
    private Queue<Queue<GameObject>> pooledObjects;
    // private GameObject objectToPool;
    // private int amountToPool;
    public WaveLoader(List<Wave> waves)
    {
        this.waves = waves;
        pooledObjects = new Queue<Queue<GameObject>>();
    }

    // Initializes the object pooling system by instantiating a specified number of GameObjects and adding them to a queue for future use.
    // public void InitializePool()
    // {
    //     pooledObjects = new Queue<GameObject>();
    //     GameObject tmp;
    //     for (int i = 0; i < amountToPool; i++)
    //     {
    //         tmp = Instantiate(objectToPool);
    //         tmp.SetActive(false);
    //         pooledObjects.Enqueue(tmp);
    //     }
    // }
    public Queue<Queue<GameObject>> IntializeWaves()
    {
        GameObject tmp;
        foreach (Wave wave in waves)
        {
            Queue<GameObject> tmpWave = new Queue<GameObject>();
            foreach (GameObject enemy in wave.enemies)
            {
                tmp = Instantiate(enemy);
                tmp.SetActive(false);
                tmpWave.Enqueue(tmp);
            }
            pooledObjects.Enqueue(tmpWave);
        }
        return pooledObjects;
    }
    // Retrieves an inactive GameObject from the object pool.
    //
    // Returns:
    //     The inactive GameObject retrieved from the object pool, or null if none are available.
    // public GameObject GetPooledObject()
    // {
    //     if (pooledObjects.Count > 0)
    //     {
    //         if (!pooledObjects.Peek().activeInHierarchy)
    //         {
    //             return pooledObjects.Dequeue();
    //         }
    //     }
    //     return null;
    // }
    public Wave GetPooledWave()
    {
        if (pooledObjects.Count > 0)
        {
            if (!pooledObjects.Peek().Peek().activeInHierarchy)
            {
                // return pooledObjects;
                return null;

            }
        }

        return null;
    }

    // Returns the specified GameObject into the pooledObjects queue.
    // public void ReturnToPool(GameObject gameObject)
    // {
    //     pooledObjects.Enqueue(gameObject);
    // }
}
