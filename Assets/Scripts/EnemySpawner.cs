using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int waveNumber = 0;
    public List<int> enemiesInWave = new List<int>() { 5, 10, 15, 20 };
    public float spawnRate = 4f;
    public float nextSpawnTime = 0f;
    public Transform player;
    private List<float> xPositions = new List<float>() { -5.0f, 0.0f, 5.0f };
    // spawn enemies around the player in a circle 
    // spawn enemies in waves of 5? 
    public void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            Spawn();
            nextSpawnTime = Time.time + spawnRate;
        }
    }
    public void Spawn()
    {
        GameObject enemy = EnemyPool.SharedInstance.GetPooledObject();
        if (enemy != null)
        {
            int randomIndex = Random.Range(0, xPositions.Count - 1);
            enemy.transform.position = new Vector2(xPositions[randomIndex], player.position.y + Random.Range(-6, 6));
            enemy.GetComponent<Enemy>().player = player.gameObject;
            enemy.SetActive(true);
        }
    }
}
