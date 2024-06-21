using System.Collections;
using UnityEngine;

public class GravityBomb : Projectile
{
    private float radius = 3f;
    private new float damage = 5f;
    //Damage every damageInterval 
    private float damageInterval = 0.5f;
    public bool isExpanded = false;
    void Update()
    {
        if (isExpanded)
        {

            ScanSurrounding();
        }
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {

            //Change sprite from projectile to gravity bomb 
            //Scan surrounding (begin the suck) 
            StartCoroutine(Expand());
            isExpanded = true;
            gameObject.layer = LayerMask.NameToLayer("GravityBomb");
        }

    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
    void OnDrawGizmosSelected()
    {
        // Optional: Draw additional visualizations when the object is selected
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
    public void ScanSurrounding()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                Debug.Log(collider.gameObject.name + " in range");
                SuckEnemies(collider.gameObject);
            }
        }
    }

    public void SuckEnemies(GameObject enemy)
    {
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, transform.position, 2.3f * Time.deltaTime);
        Damage(enemy);
    }

    public void Damage(GameObject enemy)
    {
        enemy.GetComponent<IDamageable>().TakeDotDamage(damage, damageInterval);
    }

    public IEnumerator Expand()
    {

        gameObject.GetComponent<Animator>().Play("gravity-bomb-expansion");
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Animator>().Play("gravity-bomb-expanded");

    }
}
