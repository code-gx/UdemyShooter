using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private float lastUpdateTime;
    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        this.enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.chaseSpeed;
    }

    public override void Update()
    {
        base.Update();
        
        enemy.FaceTarget(GetNextPathPoint());

        if (enemy.PlayerAttackRange())
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
        
        if (CanUpadateDestination())
        {
            enemy.agent.SetDestination(enemy.player.position);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.agent.isStopped = true;
    }

    private bool CanUpadateDestination()
    {
        if (Time.time > lastUpdateTime + .25f)
        {
            lastUpdateTime = Time.time;
            return true;
        }
        else
            return false;
    }
}
