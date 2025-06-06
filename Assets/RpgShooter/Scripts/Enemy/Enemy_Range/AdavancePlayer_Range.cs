using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdavancePlayer_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 playerPos;
    public AdavancePlayer_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.enemyVisual.EnableIK(true, true);

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.advanceSpeed;
    }

    public override void Update()
    {
        base.Update();
        playerPos = enemy.player.transform.position;

        enemy.agent.SetDestination(enemy.player.transform.position);
        enemy.FaceTarget(GetNextPathPoint());

        if (Vector3.Distance(enemy.transform.position, playerPos) < enemy.advanceStopDistance)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
