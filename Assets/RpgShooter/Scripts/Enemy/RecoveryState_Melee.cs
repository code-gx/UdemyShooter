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
        //决定下一个攻击动画
        enemy.attackData = UpdatedAttackData();
    }

    public override void Update()
    {
        base.Update();
        float rotationSpeed = enemy.anim.GetFloat("RecoveryIndex") == 1 ? 3 : 1; 
        enemy.transform.rotation = enemy.FaceTarget(enemy.player.position,rotationSpeed);
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.abilityState);
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

    private bool PlayerClose() => Vector3.Distance(enemy.transform.position, enemy.player.position) <= 0.7;

    private AttackData UpdatedAttackData()
    {
        List<AttackData> validAttacks = new List<AttackData>(enemy.attackList);
        
        if (PlayerClose())
            validAttacks.RemoveAll(parameter => parameter.attackType == Attack_Type.Charge);
        int index = Random.Range(0, validAttacks.Count);
        return validAttacks[index];
    }
}
