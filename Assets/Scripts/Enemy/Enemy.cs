using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IEnemy
{
    public EnemyData _enemyData;
    public EnemyData enemyData { get => _enemyData; set => _enemyData = value; }
    public FireStrategy gun;
    public Transform gunPos;
    private float _damage;
    [HideInInspector]
    public float damage { get => _damage; set => _damage = value; }
    private float fireRate;
    private string _layer = "Enemy";
    public string layer { get => _layer; set => _layer = value; }
    private float _health;
    public float health { get => _health; set => _health = value; }
    private float movementSpeed;
    private float nextMovementTime = 0f;
    private float nextFireTime = 0f;
    public GameObject player;

    public void Start()
    {
        health = _enemyData.health;
        damage = _enemyData.damage;
        fireRate = _enemyData.fireRate;
        movementSpeed = _enemyData.movementSpeed;

    }

    void Update()
    {
        Shoot();
        Rotation(player.transform.position);
        // Movement(player.transform.position);
        // Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        string collidingObjectTag = other.gameObject.tag;
        string sourceTag = other.gameObject.GetComponent<Projectile>().source.tag;

        if (collidingObjectTag == "Projectile" && sourceTag == "Player")
        {
            TakeDamage(other.gameObject.GetComponent<Projectile>().damage);
        }
    }

    // public void Destroy(GameObject gameObject)
    // {

    //     if (health <= 0)
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
    public void Movement(Vector2 playerPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPosition, 1 * Time.deltaTime);
    }
    public void Rotation(Vector2 playerPosition)
    {
        _enemyData.rotation.Rotation(player.transform, this.gameObject.transform);
        Debug.Log("current enemy rotation:" + transform.rotation);
    }
    public void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            gun.FireGun(gunPos, this.gameObject);
            nextFireTime = Time.time + fireRate;
        }
    }
}
