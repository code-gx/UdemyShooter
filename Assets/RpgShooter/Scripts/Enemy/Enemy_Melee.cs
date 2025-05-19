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
    public IdleState_Melee idleState{ get; private set; }
    public MoveState_Melee moveState{ get; private set; }
    public RecoveryState_Melee recoveryState{ get; private set; }
    public ChaseState_Melee chaseState{ get; private set; }
    public AttackState_Melee attackState{ get; private set; }

    public DeadState_Melee deadState{ get; private set; }

    public AbilityState_Melee abilityState;

    [Header("Enemy Settings")]
    public Enemy_Melee_Type meleeType;
    [SerializeField] private Transform shieldTransform;
    public float dodgeCooldown;
    private float lastTimeDodge;

    [Header("Attack Data")]
    public AttackData attackData;
    public List<AttackData> attackList;
    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pullWeapon;

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
    }

    private void InitializeSpeciality()
    {
        if (meleeType == Enemy_Melee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
        }
    }

    public void TriggerAbility()
    {
        abilityState.moveSpeed *= 0.6f;
        pullWeapon.gameObject.SetActive(false);
    }

    public override void GetHit()
    {
        base.GetHit();
        if (healthPoints <= 0)
            stateMachine.ChangeState(deadState);
    }

    public void PullWeapon()
    {
        hiddenWeapon.gameObject.SetActive(false);
        pullWeapon.gameObject.SetActive(true);
    }
    public bool PlayerAttackRange() =>Vector3.Distance(transform.position, player.position) < attackData.attackRange;

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
        // if (Vector3.Distance(transform.position, player.position) < 1.5f)
        //     return; 
        if (Time.time > lastTimeDodge + dodgeCooldown)
            {
                lastTimeDodge = Time.time;
                anim.SetTrigger("Dodge");
            }
    }
}
