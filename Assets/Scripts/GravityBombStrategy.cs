using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class GravityBombStrategy : FireStrategy
{
    public GameObject prefab;
    public override void FireGun(Transform gunPos, GameObject source)
    {
        GameObject newBullet = Instantiate(prefab, new Vector3(gunPos.position.x, gunPos.position.y, 0), Quaternion.identity);
        // GameObject newBullet = BulletProjectilePool.SharedInstance.GetPooledObject();
        if (newBullet != null)
        {
            // Projectile projectile = newBullet.GetComponent<Projectile>();
            //details can be injected from source
            // newBullet.transform.position = gunPos.position;
            // newBullet.transform.rotation = gunPos.rotation;
            // newBullet.SetActive(true);
        }
        double sourceRotationAngleRad = source.transform.rotation.z * Math.PI / 180;

        Vector3 bulletDirection = source.transform.up.normalized;
        bulletDirection.y = bulletDirection.y * (float)Math.Cos(sourceRotationAngleRad);
        newBullet.GetComponent<MonoBehaviour>().StartCoroutine(MoveBullet(newBullet, bulletDirection));
    }

    IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
    {
        float startTime = Time.time;
        float duration = 2f;
        while (Time.time - startTime < duration) // Adjust the duration as needed.
        {
            if (bullet.GetComponent<GravityBomb>() != null && !bullet.GetComponent<GravityBomb>().isExpanded)
            {
                bullet.transform.position += direction * 10f * Time.deltaTime;
            }
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        bullet.SetActive(false);
        // BulletProjectilePool.SharedInstance.ReturnToPool(bullet);
    }
}

