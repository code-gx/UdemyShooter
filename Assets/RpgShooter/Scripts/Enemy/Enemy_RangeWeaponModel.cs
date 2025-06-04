using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyRange_WeaponHoldType
{
    Comman,
    LowHold,
    HighHold,
}
public class Enemy_RangeWeaponModel : MonoBehaviour
{
    public Transform gunPoint;
    [Space]
    public EnemyRange_WeaponModel_Type weaponType;
    public EnemyRange_WeaponHoldType weaponHoldType;

    public Transform leftHandIK;
    public Transform leftElbowIK;
}
