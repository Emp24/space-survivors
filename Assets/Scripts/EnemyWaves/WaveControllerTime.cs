using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Endless, pooled enemy spawning that escalates over time:
// more enemies alive at once, faster spawns, tougher enemies, and periodic elites.
public class WaveControllerTime : MonoBehaviour
{
    public static WaveControllerTime instance;

    [Header("Concurrency")]
    [SerializeField] private int startMaxEnemies = 10;
    [SerializeField] private int maxEnemiesCap = 150;
    [SerializeField] private float extraEnemiesPerMinute = 15f;

    [Header("Spawn rate")]
    [SerializeField] private float startSpawnInterval = 0.5f;
    [SerializeField] private float minSpawnInterval = 0.08f;
    [Tooltip("Seconds for the spawn interval to halve.")]
    [SerializeField] private float spawnIntervalHalfLife = 150f;

    [Header("Toughness")]
    [Tooltip("Enemy health grows by this fraction every minute (0.2 = +20%/min).")]
    [SerializeField] private float healthGrowthPerMinute = 0.2f;

    [Header("Elites")]
    [SerializeField] private float firstEliteTime = 45f;
    [SerializeField] private float eliteInterval = 35f;

    [Header("Pooling")]
    [SerializeField] private int initialPoolPerEnemyType = 10;

    private int currentNumberOfEnemies = 0;
    // 4 spawn points for the enemies to spawn from North/South/East/West (to give the player the sense of being surrounded)
    public List<Transform> spawnPoints;
    public List<GameObject> enemies;
    private Queue<GameObject> pooledEnemies;
    public GameObject player;
    public EnemySpawner spawner;

    private float elapsed;
    private float nextEliteTime;

    public float Elapsed => elapsed;
    public int MaxEnemies => Mathf.Min(maxEnemiesCap, startMaxEnemies + Mathf.FloorToInt(extraEnemiesPerMinute * elapsed / 60f));
    public float SpawnInterval => Mathf.Max(minSpawnInterval, startSpawnInterval * Mathf.Pow(0.5f, elapsed / spawnIntervalHalfLife));
    public float HealthMultiplier => 1f + healthGrowthPerMinute * elapsed / 60f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        pooledEnemies = new Queue<GameObject>();
        IntializeEnemies();
        spawner = new EnemySpawner(player.transform, new List<float>() { -10f, 10f }, new Vector2(-10f, 10f));
        nextEliteTime = firstEliteTime;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {
        // Scaled time, so pausing doesn't advance the difficulty.
        elapsed += Time.deltaTime;
    }

    public IEnumerator SpawnEnemies()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(SpawnInterval);
        }
    }

    public void Spawn()
    {
        if (currentNumberOfEnemies >= MaxEnemies) return;

        bool spawnElite = elapsed >= nextEliteTime;
        if (spawnElite) nextEliteTime += eliteInterval;

        GameObject enemy = pooledEnemies.Count > 0 ? pooledEnemies.Dequeue() : CreateEnemy(enemies[Random.Range(0, enemies.Count)]);
        currentNumberOfEnemies++;
        enemy.GetComponent<IEnemy>().PrepareForSpawn(HealthMultiplier, spawnElite);
        spawner.Spawn(enemy);
    }

    public void IntializeEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            for (int i = 0; i < initialPoolPerEnemyType; i++)
            {
                pooledEnemies.Enqueue(CreateEnemy(enemy));
            }
        }
    }

    private GameObject CreateEnemy(GameObject prefab)
    {
        GameObject tmp = Instantiate(prefab);
        tmp.SetActive(false);
        return tmp;
    }

    public void DestoryEnemy(GameObject enemy)
    {
        currentNumberOfEnemies--;
        enemy.GetComponent<IEnemy>().ResetData();
        enemy.SetActive(false);
        pooledEnemies.Enqueue(enemy);
    }
}
