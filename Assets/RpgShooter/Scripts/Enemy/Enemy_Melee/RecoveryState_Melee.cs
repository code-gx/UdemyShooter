using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }
    public override void Enter()
    {
        base.Enter();
        SetNextRecoveryAndAttack();
    }

    private void SetNextRecoveryAndAttack()
    {
        //决定下一个recovery动画
        int recoveryIndex = enemy.PlayerAttackRange() ? 1 : 0;
        enemy.anim.SetFloat("RecoveryIndex", recoveryIndex);
    }

    public override void Update()
    {
        base.Update();
        float rotationSpeed = enemy.anim.GetFloat("RecoveryIndex") == 1 ? 3 : 1; 
        enemy.FaceTarget(enemy.player.position,rotationSpeed);
        if (triggerCalled)
        {
            if (enemy.CanThrowAxe())
                stateMachine.ChangeState(enemy.abilityState);
            else if (enemy.PlayerAttackRange())
                stateMachine.ChangeState(enemy.attackState);
            else
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
