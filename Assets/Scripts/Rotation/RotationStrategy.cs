using UnityEngine;

public abstract class RotationStrategy : ScriptableObject
{
    public abstract void Rotation(Transform target, Transform objectTransform);
}
