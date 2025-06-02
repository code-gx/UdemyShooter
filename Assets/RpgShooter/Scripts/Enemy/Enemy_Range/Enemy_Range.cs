using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Range : Enemy
{
    public Transform weaponHolder;

    public float fireRate = 1; //每s射的子弹数量
    public GameObject bulletPrefab;
    public Transform gunPoint;
    public float bulletSpeed;
    private const float REFRENCE_BULLET_SPEED = 20;
    public int bulletsToShot = 5;
    public float weaponCooldown = 1.5f;
    
    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");
        Vector3 bulletDirection = ((player.position + Vector3.up) - gunPoint.position).normalized;
        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.GetComponent<TrailRenderer>().Clear();
        newBullet.transform.rotation = Quaternion.LookRotation(bulletDirection);
        newBullet.GetComponent<Enemy_Bullet>().BulletSetup();

        newBullet.GetComponent<Rigidbody>().velocity = bulletDirection * bulletSpeed;
        newBullet.GetComponent<Rigidbody>().mass = REFRENCE_BULLET_SPEED / bulletSpeed;
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;
        base.EnterBattleMode();
        stateMachine.ChangeState(battleState);
    }
}
