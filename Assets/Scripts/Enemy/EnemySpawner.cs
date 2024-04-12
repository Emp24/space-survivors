using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner
{
    public Transform spawnPoint;
    private List<float> xPositions;
    private Vector2 yPositions;

    public EnemySpawner(Transform spawnPoint, List<float> xPositions, Vector2 yPositions)
    {
        this.spawnPoint = spawnPoint;
        this.xPositions = xPositions;
        this.yPositions = yPositions;
    }

    public void Spawn(GameObject enemy)
    {
        if (enemy != null)
        {

            EnemyData newEnemyData = enemy.GetComponent<IEnemy>().enemyData;
            int randomIndex = Random.Range(0, xPositions.Count);
            enemy.transform.position = new Vector2(spawnPoint.position.x + xPositions[randomIndex], spawnPoint.position.y + Random.Range(yPositions.x, yPositions.y));
            Debug.Log("<color=green> enemy coordinates: " + enemy.transform.position + "</color>");
            //Could intialize the enemy stats here 
            //Inject new enemy data for stats

            newEnemyData._health = 20f;
            newEnemyData.spawnPoint = spawnPoint.gameObject;

            enemy.GetComponent<IEnemy>().enemyData = newEnemyData;
            enemy.SetActive(true);
        }
    }

}
