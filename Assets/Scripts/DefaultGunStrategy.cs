using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class DefaultGunStrategy : FireStrategy
{
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 2f;

    public override void FireGun(Transform gunPos, GameObject source)
    {
        FireGun(gunPos, source, 0f);
    }

    public override void FireGun(Transform gunPos, GameObject source, float angleOffset)
    {
        GameObject newBullet = BulletProjectilePool.SharedInstance.GetPooledObject();
        if (newBullet == null) return;

        IDamageable shooter = source.GetComponent<IDamageable>();
        Projectile projectile = newBullet.GetComponent<Projectile>();
        //details can be injected from source
        projectile.damage = shooter.damage;
        projectile.gameObject.layer = LayerMask.NameToLayer(shooter.layer);
        projectile.source = source;

        Quaternion spread = Quaternion.Euler(0f, 0f, angleOffset);
        newBullet.transform.position = gunPos.position;
        newBullet.transform.rotation = spread * gunPos.rotation;
        newBullet.SetActive(true);

        Vector3 bulletDirection = spread * source.transform.up.normalized;
        projectile.StartCoroutine(MoveBullet(newBullet, bulletDirection));
    }

    IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
    {
        float startTime = Time.time;
        while (Time.time - startTime < projectileLifetime)
        {
            bullet.transform.position += direction * projectileSpeed * Time.deltaTime;
            yield return null;
        }
        // If the bullet hits something first it's deactivated, which also stops this coroutine.
        BulletProjectilePool.SharedInstance.ReturnToPool(bullet);
    }
}
