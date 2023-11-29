using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DefaultGunStrategy : FireStrategy
{
    public override void FireGun(Transform gunPos, PlayerController player)
    {
        // GameObject newBullet = Instantiate(bullet, new Vector3(gunPos.position.x, gunPos.position.y, 0), Quaternion.identity);
        GameObject newBullet = ObjectPool.SharedInstance.GetPooledObject();

        if (newBullet != null)
        {

            newBullet.transform.position = gunPos.transform.position;
            newBullet.transform.rotation = gunPos.transform.rotation;
            newBullet.SetActive(true);
        }
        Vector3 bulletDirection = player.transform.up.normalized;
        player.StartCoroutine(MoveBullet(newBullet, bulletDirection));
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
        ObjectPool.SharedInstance.ReturnToPool(bullet);
    }
}