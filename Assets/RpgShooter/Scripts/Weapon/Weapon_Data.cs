using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;
    
    [Header("Regular shot")]
    public string weaponName;
    public Weapon_Type weaponType;
    public float fireRate; 

    [Header("Burst")]
    public bool burstModeAvailable;
    public bool burstActive;
    public int bulletsPerShot = 1;
    public float burstModeFireRate;

    public float burstFireDely = 0.1f;

    [Header("Spread")]
    public float baseSpread;
    public float maxSpread;

    [Header("Weapon generics")]
    public Shoot_Type shootType;
    [Range(1,3)]
    public float reloadSpeed;
    [Range(1,3)]
    public float equipSpeed;
    [Range(4,8)]
    public float gunDistance;
    [Range(4,8)]
    public float cameraDistance;
}
