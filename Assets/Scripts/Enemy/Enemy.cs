using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    // [SerializeField]
    // private float health;
    [SerializeField]
    public FireStrategy gun;
    [SerializeField]
    public Transform gunPos;
    // public float damage;
    public Projectile projectile;
    public string _layer = "Enemy";
    public string layer { get => _layer; set => _layer = value; }

    [SerializeField]
    private float _health;
    [SerializeField]
    private float _damage;
    public float health { get => _health; set => _health = value; }
    public float damage { get => _damage; set => _damage = value; }
    public float fireRate = 1f;
    private float movementSpeed = 0.09f;
    public float nextMovementTime = 0f;
    private float nextFireTime = 0f;
    public GameObject player;


    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            gun.FireGun(gunPos, this.gameObject);
            nextFireTime = Time.time + fireRate;

        }
        if (Time.time >= nextMovementTime)
        {
            Movement(player.transform.position);
            Rotation(player.transform.position);
            nextMovementTime = Time.time + movementSpeed;
        }
        // StartCoroutine(Movement());

        // if (Time.time >= nextFireTime)
        // {
        //     Movement(new Vector2(0, 0));
        // }
        DestroyEnemy();
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

    public void DestroyEnemy()
    {

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
    public void Movement(Vector2 playerPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPosition, 10 * Time.deltaTime);
    }
    public void Rotation(Vector2 playerPosition)
    {
        float angle = Mathf.Atan2(playerPosition.y, playerPosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }
}
