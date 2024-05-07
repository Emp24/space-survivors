using UnityEngine;

public class ExperienceBlob : MonoBehaviour
{
    public float experience;
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            ExperienceBlobPool.SharedInstance.ConsumeObject(gameObject);
        }
    }
}
