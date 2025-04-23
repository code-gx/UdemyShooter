using System;
using UnityEngine;

public enum Weapon_Type
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rfile
}

public enum Shoot_Type
{
    Single,
    Auto
}

[System.Serializable]
public class Weapon 
{
    public Weapon_Type weaponType;
    
    [Header("Shooting details")]
    [Space]
    public Shoot_Type shootType;
    public float fireRate = 1; //每秒射出的子弹数
    private float lastShootTime;
    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;
    [Range(1,3)]
    public float reloadSpeed = 1;
    [Range(1,3)]
    public float equipSpeed = 1;
    

    public bool CanShoot()
    {
        if (HaveEnoughBullets() && ReadyToFire())
        {
            bulletsInMagazine--;
            return true;
        }
        return false;
    }

    private bool ReadyToFire()
    {
        if(Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }
        else
            return false;
    }

    #region reload methods
    public bool CanReload()
    {
        if(bulletsInMagazine == magazineCapacity)
            return false;
        if (totalReserveAmmo > 0)
        {
            return true;
        }
        return false;
    }

    private bool HaveEnoughBullets() => bulletsInMagazine > 0 ;

    public void Reload()
    {
        int bulletsToReload = magazineCapacity - bulletsInMagazine;
        bulletsToReload = Math.Min(bulletsToReload, totalReserveAmmo);
        bulletsInMagazine += bulletsToReload;
        totalReserveAmmo -= bulletsToReload;
    }
    #endregion
}
