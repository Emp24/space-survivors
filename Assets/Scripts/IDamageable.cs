using UnityEngine;

public interface IDamageable
{
    string layer { get; set; }
    float health { get; set; }
    float damage { get; set; }

    void TakeDamage(float damage);
    void Destroy();
}
