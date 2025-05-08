using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float turnSpeed;
    public float aggresionRange;
    [Header("Idle data")]
    public float idleTime;
    [Header("Move data")]
    public float moveSpeed;
    [SerializeField] private Transform[] patrolPoints;

    public Transform player {get; private set;}
    public Animator anim {get; private set;}
    private int currentPatrolIndex;
    public NavMeshAgent agent {get; private set;}

    public EnemyStateMachine stateMachine {get; private set;}

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

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEularAngles = transform.rotation.eulerAngles;
        float yRotation = Mathf.LerpAngle(currentEularAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);
        return Quaternion.Euler(currentEularAngles.x, yRotation, currentEularAngles.z);
    }  

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();  
}
