using System.Collections;
using UnityEngine;

public class Seeker : MonoBehaviour, IEnemy, IDamageable
{
    private const float BaseMoveSpeed = 3f;

    public GameObject player;
    public EnemyData _enemyData;
    private float nextMovementTime = 0f;
    public EnemyData enemyData { get { return _enemyData; } set { _enemyData = value; } }
    private float _health;
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

    [Header("Elite")]
    [SerializeField] private float eliteHealthMultiplier = 8f;
    [SerializeField] private float eliteScale = 1.7f;
    [SerializeField] private float eliteSpeedMultiplier = 0.75f;
    [SerializeField] private float eliteExperience = 8f;
    [SerializeField] private float eliteKnockback = 2.5f;
    [SerializeField] private Color eliteTint = new Color(1f, 0.45f, 0.45f);

    public bool IsElite { get; private set; }
    private bool isDying;
    private float speedMultiplier = 1f;
    private float experienceDrop = 1f;
    private Vector3 baseScale;
    private SpriteRenderer spriteRenderer;
    private Color baseColor = Color.white;

    private bool initialized;

    public void Awake()
    {
        // The prefab is saved inactive, so for pooled Seekers Awake only runs on first activation,
        // after the wave controller has already called PrepareForSpawn. Don't reset stats here.
        EnsureInitialized();
    }

    void OnEnable()
    {
        // The spawner assigns the target right before activation.
        player = _enemyData.spawnPoint;
    }

    // Caches the prefab's original look. Must run before anything modifies scale/colour,
    // whichever of Awake / PrepareForSpawn / ResetData happens first.
    private void EnsureInitialized()
    {
        if (initialized) return;
        initialized = true;
        baseScale = transform.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) baseColor = spriteRenderer.color;
        health = _enemyData.health;
        damage = _enemyData.damage;
        fireRate = _enemyData.fireRate;
        movementSpeed = _enemyData.movementSpeed;
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
        GameEvents.EnemyKilled(transform.position, IsElite);
        yield return new WaitForSeconds(0.1f);
        OnDestruction();
        WaveControllerTime.instance.DestoryEnemy(gameObject);
    }

    public void Destroy()
    {
        // Guard against dying twice (e.g. killing blow + player collision in the same frame),
        // which used to return the same enemy to the pool twice.
        if (isDying) return;
        isDying = true;
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
            if (IsElite)
            {
                // Elites don't kamikaze: they bounce off and come back for more.
                Vector3 away = (transform.position - other.transform.position).normalized;
                transform.position += away * eliteKnockback;
            }
            else
            {
                Destroy();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDying) return;
        animator.Play("seeker-take-damage");
        health -= damage;
        GameEvents.EnemyDamaged(transform.position, damage);
        if (health <= 0)
        {
            Destroy();
        }
    }

    public void Movement(Vector2 playerPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPosition, BaseMoveSpeed * speedMultiplier * Time.deltaTime);
    }

    public void Rotation(Vector2 playerPosition)
    {
        _enemyData.rotation.Rotation(player.transform, this.gameObject.transform);
    }

    public void OnDestruction()
    {
        ExperienceBlobPool.SharedInstance.SpawnObject(transform, experienceDrop);
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
        EnsureInitialized();
        health = _enemyData.health;
        damage = _enemyData.damage;
        fireRate = _enemyData.fireRate;
        movementSpeed = _enemyData.movementSpeed;
        player = _enemyData.spawnPoint;
        isTakingDamage = false;
        isDying = false;
        IsElite = false;
        speedMultiplier = 1f;
        experienceDrop = 1f;
        transform.localScale = baseScale;
        if (spriteRenderer != null) spriteRenderer.color = baseColor;
    }

    public void PrepareForSpawn(float healthMultiplier, bool isElite)
    {
        ResetData();
        health *= healthMultiplier;
        if (!isElite) return;

        IsElite = true;
        health *= eliteHealthMultiplier;
        speedMultiplier = eliteSpeedMultiplier;
        experienceDrop = eliteExperience;
        transform.localScale = baseScale * eliteScale;
        if (spriteRenderer != null) spriteRenderer.color = eliteTint;
    }
}
