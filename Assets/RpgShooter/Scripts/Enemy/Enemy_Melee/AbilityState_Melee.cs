using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 moveDirection;
    public float moveSpeed;
    private const float MAX_MOVE_DISTANCE = 20f;
    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.EnableWeaponModel(true);
        moveSpeed = enemy.moveSpeed;
        moveDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVE_DISTANCE);
    }

    public override void Update()
    {
        base.Update();
        if (enemy.getManualRotation())
        {
            enemy.FaceTarget(enemy.player.position);
            moveDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVE_DISTANCE);
        }
        if (enemy.getMaualMovement())
        {
            if (!enemy.PlayerAttackRange())
            {
                enemy.transform.position =
                Vector3.MoveTowards(enemy.transform.position, moveDirection, moveSpeed * Time.deltaTime);
            }
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void TriggerAbility()
    {
        base.TriggerAbility();
        GameObject newAxe = ObjectPool.instance.GetObject(enemy.axePrefab);
        newAxe.transform.position = enemy.axeStartPoint.position;
        newAxe.GetComponent<EnemyAxe>().AxeSetup(enemy.axeFlySpeed, enemy.player, enemy.aimTimer);
    }
}
