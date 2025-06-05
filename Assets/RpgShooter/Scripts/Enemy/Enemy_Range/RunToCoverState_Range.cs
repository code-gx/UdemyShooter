using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunToCoverState_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 destination;

    public RunToCoverState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = false;
        destination = enemy.AttemptFindCover().position;
        enemy.agent.speed = enemy.chaseSpeed;
        enemy.agent.SetDestination(destination);
        enemy.enemyVisual.EnableIK(true, false);
    }

    public override void Update()
    {
        base.Update();
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance < enemy.agent.stoppingDistance)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.FaceTarget(GetNextPathPoint());
    }
}
