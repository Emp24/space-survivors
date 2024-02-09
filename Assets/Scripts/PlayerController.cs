using UnityEditor.Callbacks;
using UnityEngine;



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
    public string _layer = "Player";
    public string layer { get => _layer; set => _layer = value; }

    [SerializeField] FireStrategy defaultGun;
    [SerializeField] FireStrategy rocketGun;
    public GameObject player;
    public GameObject bullet;
    private float attackRate = 0.2f;
    public Vector2 speed = new(5, 5);
    public Transform rightGun;
    public Transform leftGun;

    void Update()
    {
        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(5, 5, 0), Color.green);
        PlayerMovement();
        PlayerRotation();
        if (Input.GetKey(KeyCode.Mouse0))
        {

            defaultGun.FireGun(rightGun, this.gameObject);
            defaultGun.FireGun(leftGun, this.gameObject);
        }
    }
    private float startTime;
    private float journeyLength;



    void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(new Vector3(-0.01f, -0.68f, 0), new Vector3(50, 50, 0));
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        string collidingObjectTag = other.gameObject.tag;
        string sourceTag = other.gameObject.GetComponent<Projectile>().source.tag;

        if (collidingObjectTag == "Projectile" && sourceTag == "Enemy")
        {
            TakeDamage(other.gameObject.GetComponent<Projectile>().damage);
        }
    }
    public void PlayerMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(speed.x * inputX, speed.y * inputY);

        // movement *= Time.deltaTime;
        rb.velocity = movement;

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
    }



}
