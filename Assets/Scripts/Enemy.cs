using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float health;
    // public float damage;

    // public float speed;

    // public void Movement()
    // {

    // }

    void Update()
    {
        DestroyEnemy();
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("test");
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
}
