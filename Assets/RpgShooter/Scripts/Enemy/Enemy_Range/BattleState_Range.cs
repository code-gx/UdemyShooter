using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState_Range : EnemyState
{
    public Enemy_Range enemy;

    private float lastTimeShot = -10;
    private int bulletsShot = 0;
    private int bulletsPerAttack;
    private float weaponCooldown;
    private float coverCheckTimer;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.weaponCooldown;
        enemy.enemyVisual.EnableIK(true, true);

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerInAggresionRange() == false)
            stateMachine.ChangeState(enemy.advancePlayerState);

        ChangeCoverIfShould();

        if (WeaponOutOfBullets())
        {
            if (WeaponOnCooldown())
                AttemptToResetWeapon();
            return;
        }

        if (CanShoot())
        {
            Shoot();
        }
        enemy.FaceTarget(enemy.player.position);
    }

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerkType != Cover_Perk.CanChangeCover)
            return;
        coverCheckTimer -= Time.deltaTime;
        if (coverCheckTimer < 0)
        {
            if (IsPlayerInClearSight() || IsPlayerClose())
            {
                if (enemy.CanGetCover())
                    stateMachine.ChangeState(enemy.runToCoverState);
            }
            coverCheckTimer = 0.5f;
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.enemyVisual.EnableIK(false, false);
    }

    #region Weapon region
    private void AttemptToResetWeapon()
    {
        bulletsShot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.weaponCooldown;
    }
    private bool WeaponOnCooldown() => Time.time > lastTimeShot + weaponCooldown;
    private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerAttack;
    private bool CanShoot() => Time.time > lastTimeShot + 1 / enemy.weaponData.fireRate;

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShot++;
    }
    #endregion

    #region Cover System

    private bool IsPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.safeDistance;
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position;
        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hitPoint))
        {
            return hitPoint.collider.gameObject.GetComponentInParent<Player>();
        }
        return false;
    }
    #endregion
}
