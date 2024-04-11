using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private WaveLoader waveLoader;
    public List<Wave> waves;
    //Contains the waves of enemeis to be spawned
    private Queue<Queue<GameObject>> pooledObjects;
    public void Awake()
    {
        waveLoader = new WaveLoader(waves);
        pooledObjects = waveLoader.IntializeWaves();
        spawner = new EnemySpawner(player.transform, new List<float>() { -6f, 0f, 6f }, new Vector2(-6f, 6f));
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
            yield return new WaitForSeconds(2f);
            GameObject enemy = wave.Dequeue();
            Debug.Log("Enemy spawned: " + enemy.GetComponent<IEnemy>().enemyData.damage + Time.time.ToString());
            // spawner.Spawn(enemy);
        }
    }


}
