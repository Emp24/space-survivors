using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class DefaultGunStrategy : FireStrategy
{
    public override void FireGun(Transform gunPos, GameObject source)
    {
        // GameObject newBullet = Instantiate(bullet, new Vector3(gunPos.position.x, gunPos.position.y, 0), Quaternion.identity);
        GameObject newBullet = BulletProjectilePool.SharedInstance.GetPooledObject();
        if (newBullet != null)
        {
            Projectile projectile = newBullet.GetComponent<Projectile>();
            //details can be injected from source
            projectile.damage = source.GetComponent<IDamageable>().damage;
            projectile.gameObject.layer = LayerMask.NameToLayer(source.GetComponent<IDamageable>().layer);
            projectile.source = source;
            newBullet.transform.position = gunPos.position;
            newBullet.transform.rotation = gunPos.rotation;
            newBullet.SetActive(true);
        }
        double sourceRotationAngleRad = source.transform.rotation.z * Math.PI / 180;

        Vector3 bulletDirection = source.transform.up.normalized;
        bulletDirection.y = bulletDirection.y * (float)Math.Cos(sourceRotationAngleRad);
        newBullet.GetComponent<MonoBehaviour>().StartCoroutine(MoveBullet(newBullet, bulletDirection));
    }
    public void IntializeProjectilePosition(Transform newBulletTransform, Transform gunPos)
    {

    }

    public void IntializeProjectileData(float damage)
    {


    }
    IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
    {
        float startTime = Time.time;
        float duration = 2f;
        while (Time.time - startTime < duration) // Adjust the duration as needed.
        {
            bullet.transform.position += direction * 10f * Time.deltaTime;
            yield return null;
        }
        bullet.SetActive(false);
        BulletProjectilePool.SharedInstance.ReturnToPool(bullet);
    }
}