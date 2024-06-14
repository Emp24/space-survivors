
using UnityEngine;

public class SeekerDestroyedState : State
{

    public AnimationClip anim;
    public override void EnterState(Seeker enemy)
    {
        Debug.Log("Hello from seeker destroyed state");
        if (anim != null)
        {
            enemy.animator.Play(anim.name);
        }
    }
    public override void UpdateState(Seeker enemy)
    {
        enemy.Destroy();
    }
    public SeekerDestroyedState(AnimationClip _anim)
    {
        anim = _anim;
    }
}
