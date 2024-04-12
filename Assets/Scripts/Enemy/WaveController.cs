using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    //manages waves
    //uses enemy spawner to spawn enemies in waves
    //Takes a wave (list of enemies and their stats and details)
    //Passes these stats and details to enemy spawner
    //Enemy spawner intializes and spawns the enemy 
    //Enemy should be represented by a sprite and a bunch of stats 
    public GameObject player;
    public EnemySpawner spawner;
    // private WaveLoader waveLoader;
    public List<Wave> waves;
    //Contains the waves of enemeis to be spawned
    private Queue<Queue<GameObject>> pooledObjects;
    private float spawnRate = 0.5f;
    public void Awake()
    {
        // waveLoader = new WaveLoader(waves);
        pooledObjects = IntializeWaves();
        spawner = new EnemySpawner(player.transform, new List<float>() { -10f, 10f }, new Vector2(-10f, 10f));
    }

    public void Start()
    {
        StartCoroutine(StartWave());
    }
    public IEnumerator StartWave()
    {
        while (pooledObjects.Count > 0)
        {
            Queue<GameObject> currentWave = pooledObjects.Dequeue();
            yield return StartCoroutine(SpawnWave(currentWave));
        }
    }
    public IEnumerator SpawnWave(Queue<GameObject> wave)
    {
        while (wave.Count > 0)
        {
            yield return new WaitForSeconds(spawnRate);
            GameObject enemy = wave.Dequeue();
            Debug.Log("Enemy spawned: " + enemy.GetComponent<IEnemy>().enemyData.damage + Time.time.ToString());
            spawner.Spawn(enemy);
        }
    }

    public Queue<Queue<GameObject>> IntializeWaves()
    {
        pooledObjects = new Queue<Queue<GameObject>>();
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

}
