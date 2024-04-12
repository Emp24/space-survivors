using System.Collections.Generic;
using UnityEngine;

public class WaveLoader : MonoBehaviour
{
    private List<Wave> waves;
    private Queue<Queue<GameObject>> pooledObjects;
    public WaveLoader(List<Wave> waves)
    {
        this.waves = waves;
        pooledObjects = new Queue<Queue<GameObject>>();
    }

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
    // public Wave GetPooledWave()
    // {
    //     if (pooledObjects.Count > 0)
    //     {
    //         if (!pooledObjects.Peek().Peek().activeInHierarchy)
    //         {
    //             // return pooledObjects;
    //             return null;

    //         }
    //     }

    //     return null;
    // }
}
