using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (enemy.PlayerAttackRange())
        {
            enemy.anim.SetFloat("RecoveryIndex", 1);
        }
        else
        {
            enemy.anim.SetFloat("RecoveryIndex", 0);
            Debug.Log("11111111");
        }
    }

    public override void Update()
    {
        base.Update();
        enemy.transform.rotation = enemy.FaceTarget(enemy.player.position);
        if (triggerCalled)
        {
            if (enemy.PlayerAttackRange())
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
