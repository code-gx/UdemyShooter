using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Range : Enemy
{
    [Header("Weapon details")]
    public EnemyRange_WeaponModel_Type weaponType;
    public Enemy_RangeWeaponData weaponData;
    [SerializeField] List<Enemy_RangeWeaponData> avaliableWeaponData;

    [Space]
    public Transform weaponHolder;
    public Transform gunPoint;
    public GameObject bulletPrefab;
    private const float REFRENCE_BULLET_SPEED = 20;

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
        SetupWeapon();
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
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);
        newBullet.GetComponent<Enemy_Bullet>().BulletSetup();

        newBullet.GetComponent<Rigidbody>().velocity = weaponData.ApplySpread(bulletDirection) * weaponData.bulletSpeed;
        newBullet.GetComponent<Rigidbody>().mass = REFRENCE_BULLET_SPEED / weaponData.bulletSpeed;
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;
        base.EnterBattleMode();
        stateMachine.ChangeState(battleState);
    }

    public void SetupWeapon()
    {
        List<Enemy_RangeWeaponData> filterList = new List<Enemy_RangeWeaponData>();
        foreach (var weapondata in avaliableWeaponData)
        {
            if (weapondata.weaponType == weaponType)
            {
                filterList.Add(weapondata);
            }
        }

        if (filterList.Count > 0)
        {
            int index = Random.Range(0, filterList.Count);
            weaponData = filterList[index];
        }
        else
            Debug.LogWarning("没有武器数据");
    }
}
