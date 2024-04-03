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
    private float nextSpawnTime = 0f;
    public float spawnRate = 1f;
    public void Awake()
    {
        spawner = new EnemySpawner(player.transform, new List<float>() { -6f, 0f, 6f }, new Vector2(-6f, 6f));
    }

    public void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            spawner.Spawn();
            nextSpawnTime = Time.time + spawnRate;
        }
    }
}
