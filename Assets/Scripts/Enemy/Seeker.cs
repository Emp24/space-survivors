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
        if (Time.time >= nextMovementTime)
        {
            Movement(player.transform.position);
            nextMovementTime = Time.time + _enemyData.movementSpeed;
        }

    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        string collidingObjectTag = other.gameObject.tag;
        string sourceTag = other.gameObject.GetComponent<Projectile>().source.tag;

        if (collidingObjectTag == "Player" && sourceTag == "Player")
        {
            TakeDamage(other.gameObject.GetComponent<Projectile>().damage);
        }
    }

    public void TakeDamage(float damage)
    {
        _enemyData.health -= damage;
    }
    public void Movement(Vector2 playerPosition)
    {

        transform.position = Vector2.MoveTowards(transform.position, playerPosition, 10 * Time.deltaTime);
    }
    public void Rotation(Vector2 playerPosition)
    {
        _enemyData.rotation.Rotation(player.transform, this.gameObject.transform);
    }
}
