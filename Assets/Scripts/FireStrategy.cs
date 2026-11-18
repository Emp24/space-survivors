using UnityEngine;

public abstract class FireStrategy : ScriptableObject
{
    public abstract void FireGun(Transform gunPos, GameObject source);

    // Fires rotated by angleOffset degrees from the source's facing (used for spread / multishot).
    // Weapons that don't support spread simply ignore the offset.
    public virtual void FireGun(Transform gunPos, GameObject source, float angleOffset)
    {
        FireGun(gunPos, source);
    }
}
