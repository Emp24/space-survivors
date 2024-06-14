
using UnityEngine;
public class SeekerIdleState : State
{

    AnimationClip anim;
    public override void EnterState(Seeker enemy)
    {
        Debug.Log("hello from idle state");
        if (anim != null)
        {
            enemy.animator.Play(anim.name);
        }
    }
    public override void UpdateState(Seeker enemy)
    {
        if (enemy.isDestroying == false && enemy.health <= 0)
        {
            enemy.SwitchState(enemy.seekerDestroyedState);
        }
        else if (enemy.isTakingDamage == false && enemy.isDestroying == false)
        {
            enemy.SwitchState(enemy.seekerIdleState);
        }
        else if (enemy.isTakingDamage == true && enemy.isDestroying == false)
        {
            enemy.SwitchState(enemy.seekerDamagedState);
        }
    }

    public SeekerIdleState(AnimationClip _anim)
    {
        anim = _anim;
    }
}
