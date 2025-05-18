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
    public float moveSpeed;
    public float chaseSpeed;
    private bool manualMovement;
    private bool manualRotation;
    [SerializeField] private Transform[] patrolPoints;

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    private int currentPatrolIndex;
    public NavMeshAgent agent { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {

    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex++].position;
        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;
        return destination;
    }

    public Quaternion FaceTarget(Vector3 target, float speed = 1)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEularAngles = transform.rotation.eulerAngles;
        float yRotation = Mathf.LerpAngle(currentEularAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime * speed);
        return Quaternion.Euler(currentEularAngles.x, yRotation, currentEularAngles.z);
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    public virtual void GetHit()
    {
        healthPoints--;
    }

    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(HitImpactCorutine(force, hitPoint, rb));
    }

    private IEnumerator HitImpactCorutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool getMaualMovement() => manualMovement;
    public bool getManualRotation() => manualRotation;
    
}
