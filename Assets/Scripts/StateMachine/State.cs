using UnityEngine;

public abstract class State
{
    public abstract void EnterState(Seeker enemy);
    public abstract void UpdateState(Seeker enemy);

}
