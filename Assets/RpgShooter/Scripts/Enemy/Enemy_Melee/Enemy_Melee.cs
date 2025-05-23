using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attack_Type
{
    Close,
    Charge,
}

public enum Enemy_Melee_Type
{
    Ruglar,
    Shield,
    Dodge,
    AxeThrow,
}

[Serializable]
public struct AttackData
{
    public string AttackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1,2)]
    public float animationSpeed;
    public Attack_Type attackType;
}

public class Enemy_Melee : Enemy
{
    #region States
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState  { get; private set; }
    #endregion

    [Header("Enemy Visual")]
    private Enemy_Visuals enemyVisual;

    [Header("Enemy Settings")]
    public Enemy_Melee_Type meleeType;
    [SerializeField] private Transform shieldTransform;
    public float dodgeCooldown;

    //保证开局可以闪避
    private float lastTimeDodge = -10;
    [Header("Axe throw ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float aimTimer;
    public float axeThrowCoolDown;
    private float lastThrowTime;
    public Transform axeStartPoint;

    [Header("Attack Data")]
    public AttackData attackData;
    public List<AttackData> attackList;

    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); //idle是占位符 用ragdoll
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");
        enemyVisual = GetComponent<Enemy_Visuals>();
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        InitializeSpeciality();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        if (CanEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;
        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);
    }

    //保证在Enemy_Visuals的初始化之前
    private void InitializeSpeciality()
    {
        if (meleeType == Enemy_Melee_Type.AxeThrow)
        {
            enemyVisual.setWeaponModelType(EnemyMelee_WeaponModel_Type.Throw);
        }

        if (meleeType == Enemy_Melee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            enemyVisual.setWeaponModelType(EnemyMelee_WeaponModel_Type.OneHand);
        }
    }

    public override void TriggerAbility()
    {
        base.TriggerAbility();
        abilityState.moveSpeed *= 0.6f;
        EnableWeaponModel(false);
    }

    public override void GetHit()
    {
        base.GetHit();
        if (healthPoints <= 0)
            stateMachine.ChangeState(deadState);
    }

    public void EnableWeaponModel(bool active)
    {
        enemyVisual.currentWeaponModel.SetActive(active);
    }
    public bool PlayerAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
    public void ActivateDodgeRoll()
    {
        if (meleeType != Enemy_Melee_Type.Dodge)
            return;
        if (stateMachine.currentState != chaseState)
            return;
        if (Vector3.Distance(transform.position, player.position) < 1.5f)
            return;
        float dodgeDuration = GetAnimationClipDuration("DodgeRoll");
        if (Time.time > lastTimeDodge + dodgeCooldown + dodgeDuration)
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("Dodge");
        }
    }

    public bool CanThrowAxe()
    {
        if (meleeType != Enemy_Melee_Type.AxeThrow)
            return false;
        if (Time.time < lastThrowTime + axeThrowCoolDown)
            return false;
        lastThrowTime = Time.time;
        return true;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        Debug.Log("No such" + clipName + " animation");
        return 0;
    }
}
