using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    public IdleState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName):base(enemyBase, stateMachine, animBoolName)
    {
        //可以as是因为enemyBase本身是个子类对象 
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemyBase.idleTime;
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
