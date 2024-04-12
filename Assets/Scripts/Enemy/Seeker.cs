using UnityEngine;

public class Seeker : MonoBehaviour, IEnemy, IDamageable
{
    public GameObject player;
    public EnemyData _enemyData;
    private float nextMovementTime = 0f;
    public EnemyData enemyData { get { return _enemyData; } set { _enemyData = value; } }
    private float _health;
    private Projectile projectile;
    private float fireRate;
    private string _layer = "Enemy";
    public string layer { get => _layer; set => _layer = value; }
    [SerializeField]
    private float _damage;
    public float health { get => _health; set => _health = value; }
    [HideInInspector]
    public float damage { get => _damage; set => _damage = value; }
    private float movementSpeed;

    public void Awake()
    {

        health = _enemyData.health;
        damage = _enemyData.damage;
        fireRate = _enemyData.fireRate;
        movementSpeed = _enemyData.movementSpeed;
        player = _enemyData.spawnPoint;

    }

    public void Update()
    {
        Movement(player.transform.position);
        if (Time.time >= nextMovementTime)
        {

            Rotation(player.transform.position);
            nextMovementTime = Time.time + _enemyData.movementSpeed;
        }
        Destroy();

    }

    public void Destroy()
    {

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        string collidingObjectTag = other.gameObject.tag;
        string sourceTag = other.gameObject?.GetComponent<Projectile>()?.source?.tag;

        if (collidingObjectTag == "Projectile" && sourceTag == "Player")
        {
            TakeDamage(other.gameObject.GetComponent<Projectile>().damage);
        }
        if (collidingObjectTag == "Player")
        {
            health = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Seeker took damage: " + damage);
        health -= damage;
    }
    public void Movement(Vector2 playerPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPosition, 3 * Time.deltaTime);
    }
    public void Rotation(Vector2 playerPosition)
    {
        _enemyData.rotation.Rotation(player.transform, this.gameObject.transform);
    }
}
