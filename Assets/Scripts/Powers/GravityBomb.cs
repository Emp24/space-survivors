using UnityEngine;

public class GravityBomb : MonoBehaviour
{
    private float damageRate = 0.5f;

    private float nextDamageTime = 0;
    void Update()
    {
        ScanSurrounding();
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3f, LayerMask.GetMask("Enemy"));
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
        enemy.GetComponent<IDamageable>().TakeDotDamage(5f, 0.5f);
    }
}
