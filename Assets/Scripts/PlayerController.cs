using System;
using UnityEngine;
using UnityEngine.UI;



public class PlayerController : MonoBehaviour, IDamageable
{
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
    private float nextFireTime = 0.0f;
    // Default firerate is 0.5f (attack each half a secod) maximum fireRate will be 0.1f
    private float fireRate = 0.5f;
    [SerializeField] FireStrategy defaultGun;
    [SerializeField] FireStrategy rocketGun;
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
    private float startTime;
    private float journeyLength;
    public Action onPlayerDataUpdated;

    void Start()
    {
        healthBar.value = health / 100;
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(new Vector3(-0.01f, -0.68f, 0), new Vector3(50, 50, 0));
    }

    void Update()
    {
        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(5, 5, 0), Color.green);
        PlayerMovement();

        if (GameManager.instance.gameState == GameState.Playing || GameManager.instance.gameState == GameState.StartGame)
        {
            PlayerRotation();

            if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextFireTime)
            {
                playerAnimator.SetTrigger("Fire");
                defaultGun.FireGun(rightGun, this.gameObject);
                defaultGun.FireGun(leftGun, this.gameObject);
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                playerAnimator.SetTrigger("StopFire");
            }
        }

        LevelUp();
        ScanSurrounding();
    }
    public void LevelUp()
    {
        if (xp >= nextLevelXp)
        {
            playerLevel++;
            nextLevelXp += nextLevelXp * 0.2f;
            xp = 0;
            Debug.Log("current player level:" + playerLevel);
            GameManager.instance.ChangeGameState(GameState.PowerMenu);
        }
        onPlayerDataUpdated?.Invoke();

    }

    public void GainXP(float amount)
    {
        xp += amount;
        Debug.Log("xp gained:" + xp);
        onPlayerDataUpdated?.Invoke();
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        string collidingObjectTag = other.gameObject.tag;
        string sourceTag = other.gameObject.GetComponent<Projectile>()?.source?.tag;

        if (collidingObjectTag == "Projectile" && sourceTag == "Enemy")
        {
            TakeDamage(other.gameObject.GetComponent<Projectile>().damage);
        }
        if (collidingObjectTag == "Enemy")
        {
            TakeDamage(other.gameObject.GetComponent<IEnemy>().enemyData.damage);
        }
        if (collidingObjectTag == "ExperienceBlob")
        {
            float xpGained = other.gameObject.GetComponent<ExperienceBlob>().experience;
            GainXP(xpGained);
        }
    }

    public void PlayerMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(speed.x * inputX, speed.y * inputY);

        // movement *= Time.deltaTime;
        rb.velocity = movement;
        //moving the camera with the player (keeping the camera on the player)
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);

        // transform.Translate(movement);
    }

    public void PlayerRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.value = health / 100;
        onPlayerDataUpdated?.Invoke();
    }
    public void Destroy()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

    }
    public void ScanSurrounding()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f, LayerMask.GetMask("Consumables"));
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                Debug.Log(collider.gameObject.name + " in range");
                SuckExpBlobs(collider.gameObject);
            }
        }

    }

    public void SuckExpBlobs(GameObject blob)
    {

        blob.transform.position = Vector2.MoveTowards(blob.transform.position, transform.position, 10 * Time.deltaTime);
    }

    public void ApplyPower(int powerId)
    {
        Debug.Log("Apply Power: " + powerId);
        switch (powerId)
        {
            case 0:
                damage *= 2;
                Debug.Log("player damage:" + damage);
                break;
            case 1:
                speed = new Vector2(speed.x + 0.2f, speed.y + 0.2f);
                Debug.Log("player speed:" + speed);
                break;
            case 2:
                health += 10;
                Debug.Log("player health:" + health);
                break;
        }
    }

}
