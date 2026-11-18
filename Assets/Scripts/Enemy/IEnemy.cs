using UnityEngine;
public interface IEnemy
{
    EnemyData enemyData { get; set; }
    public void Movement(Vector2 playerPosition);
    public void Rotation(Vector2 playerPosition);
    public void ResetData();
    // Called by the wave controller right before the enemy is (re)spawned, to apply difficulty scaling.
    public void PrepareForSpawn(float healthMultiplier, bool isElite);
}
