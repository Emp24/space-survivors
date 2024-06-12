using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveControllerTime : MonoBehaviour
{
    public static WaveControllerTime instance;
    private int maxNumberOfEnemies = 10;
    private int currentNumberOfEnemies = 0;
    private float spawnRate = 0.5f;
    // 4 spawn points for the enemies to spawn from North/South/East/West (to give the player the sense of being surrounded) 
    public List<Transform> spawnPoints;
    public List<GameObject> enemies;
    private Queue<GameObject> pooledEnemies;
    public GameObject player;
    public EnemySpawner spawner;
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        pooledEnemies = new Queue<GameObject>();
        IntializeEnemies();
        spawner = new EnemySpawner(player.transform, new List<float>() { -10f, 10f }, new Vector2(-10f, 10f));
    }
    public IEnumerator SpawnEnemies()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnRate);
        }
    }
    public void Spawn()
    {
        if (currentNumberOfEnemies < maxNumberOfEnemies)
        {
            currentNumberOfEnemies++;
            GameObject tmp;
            tmp = pooledEnemies.Dequeue();
            // tmp.SetActive(true);
            // tmp.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            spawner.Spawn(tmp);
        }
    }
    public void IntializeEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            IntializeEnemy(enemy, 10);
        }

    }
    public void IntializeEnemy(GameObject enemy, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject tmp;
            tmp = Instantiate(enemy);
            tmp.SetActive(false);
            pooledEnemies.Enqueue(tmp);
        }
    }
    public void DestoryEnemy(GameObject enemy)
    {
        currentNumberOfEnemies--;
        enemy.GetComponent<IEnemy>().ResetData();
        enemy.SetActive(false);
        pooledEnemies.Enqueue(enemy);
        Debug.Log("current number of enemies:" + currentNumberOfEnemies);

    }
}
