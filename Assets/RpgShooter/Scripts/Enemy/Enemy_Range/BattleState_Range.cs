using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState_Range : EnemyState
{
    public Enemy_Range enemy;

    private float lastTimeShot = -10;
    private int bulletsShot = 0;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(enemy.player.position);

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
    }

    private void AttemptToResetWeapon() => bulletsShot = 0;
    private bool WeaponOnCooldown() => Time.time > lastTimeShot + enemy.weaponCooldown;
    private bool WeaponOutOfBullets() => bulletsShot >= enemy.bulletsToShot;
    private bool CanShoot() => Time.time > lastTimeShot + 1 / enemy.fireRate;

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShot++;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
