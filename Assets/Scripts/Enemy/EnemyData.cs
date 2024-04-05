using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    public float _health;
    public Projectile projectile;
    public float fireRate;
    public string _layer = "Enemy";
    public string layer { get => _layer; set => _layer = value; }
    [SerializeField]
    private float _damage;
    public float health { get => _health; set => _health = value; }
    public float damage { get => _damage; set => _damage = value; }
    public float movementSpeed;
    public RotationStrategy rotation;

}


// [SerializeField] public FireStrategy gun;
// [SerializeField]
// public Transform gunPos;
// public float damage;