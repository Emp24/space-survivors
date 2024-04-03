using UnityEngine;
public interface IEnemy
{
    EnemyData enemyData { get; set; }
    public void Movement(Vector2 playerPosition);
    public void Rotation(Vector2 playerPosition);

}
