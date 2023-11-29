using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RocketStrategy : FireStrategy
{
    public GameObject prefab;
    public override void FireGun(Transform gunPos, PlayerController player)
    {
        GameObject newBullet = Instantiate(prefab, new Vector3(gunPos.position.x, gunPos.position.y, 0), Quaternion.identity);

        // GameObject newBullet = ObjectPool.SharedInstance.GetPooledObject();

        if (newBullet != null)
        {
            newBullet.transform.position = gunPos.transform.position;
            newBullet.transform.rotation = gunPos.transform.rotation;
            newBullet.SetActive(true);
        }
        Vector3 bulletDirection = player.transform.up;
        bulletDirection.Normalize();
        player.StartCoroutine(MoveBullet(newBullet, bulletDirection));
        // bullet.transform.position += bulletDirection * Time.deltaTime * 10f;
        // Debug.Log(bullet.transform.position);
        Destroy(newBullet, 3f); // Destroy the bullet after 2 seconds, adjust as needed.
    }
    IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
    {
        float startTime = Time.time;
        while (Time.time - startTime < 2f) // Adjust the duration as needed.
        {
            bullet.transform.position += direction * 10f * Time.deltaTime;
            yield return null;
        }
        bullet.SetActive(false);
    }
}
