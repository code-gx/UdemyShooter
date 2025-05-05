using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 destination;
    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        destination = enemy.GetPatrolDestination();
    }

    public override void Update()
    {
        base.Update();
        enemy.agent.SetDestination(destination);
        //remainingDistance初始化为0 计算路径需要几帧 pathPending是否正在计算路径
        if (enemy.agent.pathPending == false && enemy.agent.remainingDistance <= 1)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("exit" + " move state");
    }
}
