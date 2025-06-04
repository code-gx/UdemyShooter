using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Range Weapon Data")]
public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon Details")]
    public EnemyRange_WeaponModel_Type weaponType;
    public float fireRate = 1;
    public int minBulletsPerAttack;
    public int maxBulletsPerAttack;
    public float weaponCooldown;
    [Header("Bullet Detail")]
    public float bulletSpeed = 20f;
    public float bulletSpread = 0.1f;
    public int GetBulletsPerAttack() => Random.Range(minBulletsPerAttack, maxBulletsPerAttack + 1);
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        float randomizedValue = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue,randomizedValue,randomizedValue);
        return spreadRotation * originalDirection; 
    }
}
