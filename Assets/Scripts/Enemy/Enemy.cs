using System.Collections;
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
    private string _layer = "EnemyProjectile";
    public string layer { get => _layer; set => _layer = value; }
    private float _health;
    public float health { get => _health; set => _health = value; }
    private float nextFireTime = 0f;
    public GameObject player;
    private bool isDestroyed = false;
    public Animator animator;

    public void Start()
    {
        health = _enemyData.health;
        damage = _enemyData.damage;
        fireRate = _enemyData.fireRate;
        player = _enemyData.spawnPoint;
    }

    void Update()
    {
        if (!isDestroyed)
        {

            Shoot();
        }
        Rotation(player.transform.position);
        Movement(player.transform.position);
        Destroy();
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

    public bool PlayDestroyAnimation()
    {

        animator.SetBool("isDestroyed", true);
        return true;
        // Destroy(gameObject);
    }
    public IEnumerator DestoryCoroutine()
    {

        if (health <= 0)
        {
            isDestroyed = true;
            PlayDestroyAnimation();
            yield return new WaitForSeconds(0.4f);
            Destroy(gameObject);
            OnDestruction();
        }
    }
    public void Destroy()
    {
        StartCoroutine(DestoryCoroutine());
    }

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
        // Debug.Log("current enemy rotation:" + transform.rotation);
    }

    public void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            gun.FireGun(gunPos, this.gameObject);
            nextFireTime = Time.time + fireRate;
        }
    }
    public void OnDestruction()
    {
        ExperienceBlobPool.SharedInstance.SpawnObject(transform);
    }
}
