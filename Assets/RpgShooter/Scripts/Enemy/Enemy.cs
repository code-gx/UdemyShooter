using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int healthPoints = 10;
    [Header("Idle data")]
    public float idleTime;
    public float aggresionRange;
    [Header("Move data")]
    public float turnSpeed;
    public float moveSpeed = 1.5f;
    public float chaseSpeed = 4.0f;
    private bool manualMovement;
    private bool manualRotation;
    [SerializeField] private Transform[] patrolPoints;

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    public bool inBattleMode { get; private set; }
    public NavMeshAgent agent { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }
    public Enemy_Visuals enemyVisual { get; private set; }

    public bool getMaualMovement() => manualMovement;
    public bool getManualRotation() => manualRotation;

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        enemyVisual = GetComponent<Enemy_Visuals>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
        inBattleMode = false;
    }

    protected virtual void Update()
    {
        if (CanEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    public void FaceTarget(Vector3 target, float speed = 1)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEularAngles = transform.rotation.eulerAngles;
        float yRotation = Mathf.LerpAngle(currentEularAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime * speed);
        transform.rotation = Quaternion.Euler(currentEularAngles.x, yRotation, currentEularAngles.z);
    }

    #region Patrol logic
    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPointsPosition.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex++];
        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;
        return destination;
    }
    #endregion

    public bool CanEnterBattleMode()
    {
        if (IsPlayerInAggresionRange() && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }
        return false;
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public virtual void GetHit()
    {
        EnterBattleMode();
        healthPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCorutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCorutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    #region animation Events
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    public virtual void TriggerAbility()
    {
        stateMachine.currentState.TriggerAbility();
    }
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    #endregion
    
    public bool IsPlayerInAggresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }
    
}
