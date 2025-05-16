using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 attackDirection;
    private float attackMoveSpeed;
    private const float MAX_ATTACK_DISTANCE = 50f;
    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        attackMoveSpeed = enemy.attackData.moveSpeed;
        enemy.anim.SetFloat("AttackAnimationSpeed", enemy.attackData.animationSpeed);
        enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex);

        enemy.PullWeapon();
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Update()
    {
        base.Update();

        //只有每帧的旋转 没有动画很奇怪
        // if (enemy.getManualRotation())
        // {
        //     enemy.transform.rotation = enemy.FaceTarget(enemy.player.position);
        // }

        if (enemy.getMaualMovement())
            {
                enemy.transform.position =
                    Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);
            }
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.recoveryState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
