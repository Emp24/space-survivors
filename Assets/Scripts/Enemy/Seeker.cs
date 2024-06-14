using System.Collections;
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
    public Animator animator;
    public bool isTakingDamage = false;
    private bool isDestroying = false;
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
    }

    public bool PlayDestroyAnimation()
    {
        animator.Play("seeker-destruction-animation");
        return true;
    }
    public IEnumerator DestoryCoroutine()
    {
        PlayDestroyAnimation();
        yield return new WaitForSeconds(0.1f);
        WaveControllerTime.instance.DestoryEnemy(gameObject);
        OnDestruction();
    }
    public void Destroy()
    {
        StartCoroutine(DestoryCoroutine());
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
            Destroy();
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Seeker took damage: " + damage);
        animator.Play("seeker-take-damage");
        health -= damage;
        if (health <= 0)
        {
            Destroy();
        }
    }
    public void Movement(Vector2 playerPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPosition, 3 * Time.deltaTime);
    }
    public void Rotation(Vector2 playerPosition)
    {
        _enemyData.rotation.Rotation(player.transform, this.gameObject.transform);
    }
    public void OnDestruction()
    {
        ExperienceBlobPool.SharedInstance.SpawnObject(transform, 1f);
    }

    public IEnumerator TakeDotDamageCoroutine(float damage, float time)
    {
        isTakingDamage = true;
        yield return new WaitForSeconds(time);
        TakeDamage(damage);
        isTakingDamage = false;
        animator.SetBool("isTakingDamage", false);
    }

    public void TakeDotDamage(float damage, float time)
    {
        if (!isTakingDamage)
        {
            StartCoroutine(TakeDotDamageCoroutine(damage, time));
        }
    }
    public void ResetData()
    {
        health = _enemyData.health;
        damage = _enemyData.damage;
        fireRate = _enemyData.fireRate;
        movementSpeed = _enemyData.movementSpeed;
        player = _enemyData.spawnPoint;
        isTakingDamage = false;
    }
}
