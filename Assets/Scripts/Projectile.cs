using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;

    public GameObject source;

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" && source.tag == "Player")
        {
            gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Player" && source.tag == "Enemy")
        {
            gameObject.SetActive(false);
        }
    }
}
