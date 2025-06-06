using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Range : Enemy
{
    [Header("Enemy perks")]
    public Cover_Perk coverPerkType;
    [Header("Advance perk")]
    public float advanceSpeed;
    public float advanceStopDistance;
    [Header("Cover system")]
    public float safeDistance;
    public CoverPoint lastCover { get; private set; }
    public CoverPoint currentCover { get; private set; }
    [Header("Weapon details")]
    public EnemyRange_WeaponModel_Type weaponType;
    public Enemy_RangeWeaponData weaponData;
    [SerializeField] List<Enemy_RangeWeaponData> avaliableWeaponData;

    [Header("Aim details")]
    public float slowAim = 4;
    public float fastAim = 20;
    public Transform aim;
    public Transform playersBody;
    public LayerMask whatToIgnore;

    [Space]
    public Transform weaponHolder;
    public Transform gunPoint;
    public GameObject bulletPrefab;
    private const float REFRENCE_BULLET_SPEED = 20;

    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    public RunToCoverState_Range runToCoverState { get; private set; }
    public AdavancePlayer_Range advancePlayerState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdavancePlayer_Range(this, stateMachine, "Advance");
    }

    protected override void Start()
    {
        base.Start();

        playersBody = player.GetComponent<Player>().playerBody;

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
        Vector3 bulletDirection = (player.position + Vector3.up - gunPoint.position).normalized;
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
        if (CanGetCover())
            stateMachine.ChangeState(runToCoverState);
        else
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

    #region Cover system
    public bool CanGetCover()
    {
        if (coverPerkType == Cover_Perk.Unavailiable)
            return false;
        currentCover = AttemptFindCover()?.GetComponent<CoverPoint>();
        if (lastCover != currentCover && currentCover != null)
            return true;
        Debug.LogWarning("没有找到掩体");
        return false;
    }
    private Transform AttemptFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();
        foreach (var cover in CollectNearByCovers())
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoint(transform));
        }
        CoverPoint closetCoverPoint = null;
        float shortestDistance = float.MaxValue;
        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float distance = Vector3.Distance(transform.position, coverPoint.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closetCoverPoint = coverPoint;
            }
        }
        if (closetCoverPoint != null)
        {
            lastCover?.SetOccupied(false);
            lastCover = currentCover;

            currentCover = closetCoverPoint;
            currentCover.SetOccupied(true);
            return currentCover.transform;
        }
        return null;
    }

    private List<Cover> CollectNearByCovers()
    {
        float coverCheckRadius = 30f;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverCheckRadius);
        List<Cover> coverList = new List<Cover>();
        foreach (var collider in hitColliders)
        {
            Cover cover = collider.GetComponent<Cover>();
            //如果物体有两个碰撞体会检测两次
            if (cover != null && !coverList.Contains(cover))
                coverList.Add(cover);
        }
        return coverList;
    }
    #endregion

    public bool AimOnPlayer()
    {
        float distanceAimToPlayer = Vector3.Distance(aim.position, player.position);
        return distanceAimToPlayer < 2;
    }
}
