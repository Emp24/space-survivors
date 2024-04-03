using UnityEngine;

public abstract class FireStrategy : ScriptableObject
{
    public abstract void FireGun(Transform gunPos, GameObject source);


}

