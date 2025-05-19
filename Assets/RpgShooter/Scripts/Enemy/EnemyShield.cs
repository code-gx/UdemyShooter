using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] private Enemy_Melee enemy;
    [SerializeField] private int durability;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();
    }
    public void ReduceDurablity()
    {
        durability--;
        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0);
            Destroy(gameObject);
        }
    } 
}
