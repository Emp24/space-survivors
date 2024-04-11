using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner
{
    public Transform player;
    private List<float> xPositions;
    private Vector2 yPositions;

    public EnemySpawner(Transform player, List<float> xPositions, Vector2 yPositions)
    {
        this.player = player;
        this.xPositions = xPositions;
        this.yPositions = yPositions;
    }

    public void Spawn(GameObject enemy)
    {
        // GameObject enemy = EnemyPool.SharedInstance.GetPooledObject();

        if (enemy != null)
        {

            int randomIndex = Random.Range(0, xPositions.Count);
            enemy.transform.position = new Vector2(player.position.x + xPositions[randomIndex], player.position.y + Random.Range(yPositions.x, yPositions.y));
            //Could intialize the enemy stats here 

            enemy.GetComponent<Seeker>().player = player.gameObject;
            enemy.SetActive(true);
            // Debug.Log("Player position: " + player.position);
            // Debug.Log("Enemy x position:" + xPositions[randomIndex] + player.position.x);
            // Debug.Log("Random index: " + randomIndex);
        }
    }


}
