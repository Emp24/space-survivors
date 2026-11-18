using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable
{
    public const float MinFireInterval = 0.1f;

    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    private float _health;
    [SerializeField]
    private float _damage;
    public float health { get => _health; set => _health = value; }
    public float damage { get => _damage; set => _damage = value; }
    public string _layer = "PlayerProjectile";
    public string layer { get => _layer; set => _layer = value; }
    [SerializeField] FireStrategy defaultGun;
    [SerializeField] FireStrategy rocketGun;
    [SerializeField] FireStrategy gravityBomb;
    public GameObject player;
    public GameObject bullet;
    public Vector2 speed = new(20, 20);
    public Transform rightGun;
    public Transform leftGun;
    public Camera cam;
    public Slider healthBar;
    public Animator playerAnimator;
    public int playerLevel = 1;
    public float xp = 0;
    public float nextLevelXp = 10;
    public Action onPlayerDataUpdated;

    [Header("Combat")]
    [Tooltip("Seconds between volleys. Default 0.5, floored at MinFireInterval.")]
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 12f;
    [Tooltip("Brief invulnerability after taking a hit, so a swarm can't delete you in one frame.")]
    [SerializeField] private float hitInvulnerability = 0.3f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 55f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1.5f;
    [SerializeField] private float dashInvulnerabilityGrace = 0.1f;

    [Header("Pickup & sustain")]
    [SerializeField] private float magnetRadius = 5f;
    [SerializeField] private float magnetPullSpeed = 10f;
    [SerializeField] private float healthRegen = 0f;

    private float nextFireTime;
    private float maxHealth;
    private float invulnerableUntil;
    private float dashEndTime;
    private float nextDashTime;
    private Vector2 dashDirection;
    private float regenBuffer;
    private bool isDead;
    private SpriteRenderer[] spriteRenderers;
    private readonly List<Collider2D> magnetHits = new List<Collider2D>(64);
    private ContactFilter2D consumablesFilter;
    private readonly Dictionary<string, int> upgradeStacks = new Dictionary<string, int>();

    public float MaxHealth { get => maxHealth; set { maxHealth = Mathf.Max(1f, value); RefreshHealth(); } }
    public float FireInterval { get => fireInterval; set => fireInterval = Mathf.Max(MinFireInterval, value); }
    public int ProjectileCount { get => projectileCount; set => projectileCount = Mathf.Max(1, value); }
    public float DashCooldown { get => dashCooldown; set => dashCooldown = Mathf.Max(0.3f, value); }
    public float MagnetRadius { get => magnetRadius; set { magnetRadius = value; magnetPullSpeed = 10f * value / 5f; } }
    public float HealthRegen { get => healthRegen; set => healthRegen = value; }
    public bool IsDashing => Time.time < dashEndTime;
    public bool IsDead => isDead;
    // 0..1 recharge progress, for the HUD.
    public float DashCharge => dashCooldown <= 0f ? 1f : Mathf.Clamp01(1f - (nextDashTime - Time.time) / dashCooldown);

    void Awake()
    {
        maxHealth = health;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        consumablesFilter = new ContactFilter2D();
        consumablesFilter.SetLayerMask(LayerMask.GetMask("Consumables"));
        consumablesFilter.useTriggers = true;
    }

    void Start()
    {
        RefreshHealth();
    }

    void Update()
    {
        bool inPlay = GameManager.instance.gameState == GameState.Playing || GameManager.instance.gameState == GameState.StartGame;

        if (inPlay && !isDead)
        {
            HandleDash();
            PlayerRotation();
            HandleFiring();
            Regenerate();
            LevelUp();
        }

        PlayerMovement();
        ScanSurrounding();
        UpdateInvulnerabilityFlash();
    }

    private void HandleFiring()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextFireTime)
        {
            playerAnimator.SetTrigger("Fire");
            // Fan the volley out symmetrically around the aim direction.
            for (int i = 0; i < projectileCount; i++)
            {
                float offset = (i - (projectileCount - 1) * 0.5f) * spreadAngle;
                defaultGun.FireGun(rightGun, gameObject, offset);
            }
            nextFireTime = Time.time + fireInterval;
        }
        else
        {
            playerAnimator.SetTrigger("StopFire");
        }
    }

    private void HandleDash()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || Time.time < nextDashTime) return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Dash where you're steering; if standing still, dash where you're aiming.
        dashDirection = input.sqrMagnitude > 0.01f ? input.normalized : (Vector2)transform.up;
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;
        invulnerableUntil = Mathf.Max(invulnerableUntil, dashEndTime + dashInvulnerabilityGrace);
    }

    private void Regenerate()
    {
        if (healthRegen <= 0f || health >= maxHealth) return;
        // Apply in chunks so the HUD isn't rebuilt every frame.
        regenBuffer += healthRegen * Time.deltaTime;
        if (regenBuffer >= 1f)
        {
            Heal(regenBuffer);
            regenBuffer = 0f;
        }
    }

    public void LevelUp()
    {
        if (xp < nextLevelXp) return;

        // Carry overflow XP into the next level instead of discarding it.
        xp -= nextLevelXp;
        nextLevelXp *= 1.2f;
        playerLevel++;
        onPlayerDataUpdated?.Invoke();
        GameManager.instance.ChangeGameState(GameState.PowerMenu);
    }

    public void GainXP(float amount)
    {
        xp += amount;
        onPlayerDataUpdated?.Invoke();
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        string collidingObjectTag = other.gameObject.tag;
        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        string sourceTag = projectile != null && projectile.source != null ? projectile.source.tag : null;

        if (collidingObjectTag == "Projectile" && sourceTag == "Enemy")
        {
            TakeDamage(projectile.damage);
        }
        if (collidingObjectTag == "Enemy")
        {
            TakeDamage(other.gameObject.GetComponent<IEnemy>().enemyData.damage);
        }
        if (collidingObjectTag == "ExperienceBlob")
        {
            GainXP(other.gameObject.GetComponent<ExperienceBlob>().experience);
        }
    }

    public void PlayerMovement()
    {
        if (IsDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");
            rb.linearVelocity = new Vector2(speed.x * inputX, speed.y * inputY);
        }

        //moving the camera with the player (keeping the camera on the player)
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
    }

    public void PlayerRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 objectPos = cam.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(mousePos.y - objectPos.y, mousePos.x - objectPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    public void TakeDamage(float damage)
    {
        if (isDead || Time.time < invulnerableUntil) return;

        health -= damage;
        invulnerableUntil = Time.time + hitInvulnerability;
        RefreshHealth();
        GameEvents.PlayerDamaged(damage);
        Destroy();
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        health = Mathf.Min(maxHealth, health + amount);
        RefreshHealth();
    }

    private void RefreshHealth()
    {
        if (healthBar != null) healthBar.value = health / maxHealth;
        onPlayerDataUpdated?.Invoke();
    }

    // Part of IDamageable: handles death. The ship stays in the scene so the camera keeps its framing.
    public void Destroy()
    {
        if (isDead || health > 0) return;

        health = 0;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        GameEvents.PlayerDied();
        GameManager.instance.ChangeGameState(GameState.GameOver);
    }

    private void UpdateInvulnerabilityFlash()
    {
        bool flashing = !isDead && Time.time < invulnerableUntil;
        float alpha = flashing ? (Mathf.Repeat(Time.time * 20f, 1f) < 0.5f ? 0.35f : 1f) : 1f;
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Color c = sr.color;
            if (!Mathf.Approximately(c.a, alpha))
            {
                c.a = alpha;
                sr.color = c;
            }
        }
    }

    public void ScanSurrounding()
    {
        int count = Physics2D.OverlapCircle(transform.position, magnetRadius, consumablesFilter, magnetHits);
        for (int i = 0; i < count; i++)
        {
            SuckExpBlobs(magnetHits[i].gameObject);
        }
    }

    public void SuckExpBlobs(GameObject blob)
    {
        blob.transform.position = Vector2.MoveTowards(blob.transform.position, transform.position, magnetPullSpeed * Time.deltaTime);
    }

    public void TakeDotDamage(float damage, float time)
    {
    }

    public int GetUpgradeStacks(string upgradeId) => upgradeStacks.TryGetValue(upgradeId, out int stacks) ? stacks : 0;

    public void ApplyUpgrade(Upgrade upgrade)
    {
        upgrade.Apply(this);
        upgradeStacks[upgrade.Id] = GetUpgradeStacks(upgrade.Id) + 1;
        onPlayerDataUpdated?.Invoke();
    }
}
