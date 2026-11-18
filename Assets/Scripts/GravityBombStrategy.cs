using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class GravityBombStrategy : FireStrategy
{
    public GameObject prefab;
    [Tooltip("Bomb damage (impact and each pull tick) as a fraction of the shooter's damage.")]
    [SerializeField] private float damageMultiplier = 0.5f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float travelTime = 2f;
    [SerializeField] private float fieldLifetime = 5f;

    public override void FireGun(Transform gunPos, GameObject source)
    {
        FireGun(gunPos, source, 0f);
    }

    public override void FireGun(Transform gunPos, GameObject source, float angleOffset)
    {
        Quaternion spread = Quaternion.Euler(0f, 0f, angleOffset);
        GameObject newBullet = Instantiate(prefab, new Vector3(gunPos.position.x, gunPos.position.y, 0), spread * gunPos.rotation);

        GravityBomb bomb = newBullet.GetComponent<GravityBomb>();
        bomb.source = source;
        //details can be injected from source
        bomb.damage = source.GetComponent<IDamageable>().damage * damageMultiplier;

        Vector3 bulletDirection = spread * source.transform.up.normalized;
        bomb.StartCoroutine(MoveBullet(bomb, bulletDirection));
    }

    IEnumerator MoveBullet(GravityBomb bomb, Vector3 direction)
    {
        float startTime = Time.time;
        while (Time.time - startTime < travelTime)
        {
            if (!bomb.isExpanded)
            {
                bomb.transform.position += direction * projectileSpeed * Time.deltaTime;
            }
            yield return null;
        }
        yield return new WaitForSeconds(fieldLifetime);
        // Bombs are instantiated per shot, so destroy them rather than leaving inactive copies behind.
        Destroy(bomb.gameObject);
    }
}
