using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float health;
    [SerializeField]
    public FireStrategy gun;
    [SerializeField]
    public Transform gunPos;
    public float damage;
    public Projectile projectile;

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
        if (other.gameObject.tag == "Projectile")
        {
            TakeDamage(10);
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
