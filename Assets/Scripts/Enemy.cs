using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    // public float speed;

    // public void Movement()
    // {

    // }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            gun.FireGun(gunPos, this.gameObject);
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
}
