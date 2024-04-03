using Unity.VisualScripting;
using UnityEngine;

public class Seeker : MonoBehaviour, IEnemy
{
    public GameObject player;
    public EnemyData _enemyData;
    public float nextMovementTime = 0f;
    public EnemyData enemyData { get { return _enemyData; } set { _enemyData = value; } }
    public void Update()
    {

        Rotation(player.transform.position);
        // if (Time.time >= nextMovementTime)
        // {
        //     Movement(player.transform.position);
        //     nextMovementTime = Time.time + _enemyData.movementSpeed;
        // }
    }
    public void Movement(Vector2 playerPosition)
    {

    }
    public void Rotation(Vector2 playerPosition)
    {
        _enemyData.rotation.Rotation(player.transform, this.gameObject.transform);
    }
}
