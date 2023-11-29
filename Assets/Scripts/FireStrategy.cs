using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FireStrategy : ScriptableObject
{
    public abstract void FireGun(Transform gunPos, PlayerController player);


}

