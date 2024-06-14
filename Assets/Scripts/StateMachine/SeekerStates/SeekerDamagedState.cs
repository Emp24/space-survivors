
using UnityEngine;

public class SeekerDamagedState : State
{
    AnimationClip anim;
    public override void EnterState(Seeker enemy)
    {
        Debug.Log("hello form seeker damage state");
        if (anim != null)
        {
            enemy.animator.Play(anim.name);
        }
    }
    public override void UpdateState(Seeker enemy)
    {

        enemy.isTakingDamage = false;
        if (enemy.isDestroying == false && enemy.health <= 0)
        {
            enemy.SwitchState(enemy.seekerDestroyedState);
        }
        else if (enemy.isTakingDamage == false && enemy.isDestroying == false)
        {
            enemy.SwitchState(enemy.seekerIdleState);
        }
    }
    public SeekerDamagedState(AnimationClip _anim)
    {
        anim = _anim;
    }
}
