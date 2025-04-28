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
    public float defaultFireRate = 10;
    private float fireRate; //每秒射出的子弹数
    private float lastShootTime;
    [Header("Burst details")]
    public bool burstModeAvailable;
    public bool burstActive;
    public float burstModeFireRate;
    public int bulletsPerShot;
    public float burstFireDely = 0.1f;
    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;
    [Range(1,3)]
    public float reloadSpeed = 1;
    [Range(1,3)]
    public float equipSpeed = 1;

    [Header("Spread")]
    public float baseSpread = 0;
    public float maxSpread = 4;
    public float currentSpread;
    public Vector3 ApplySpread(Vector3 originalDirection, float buttonTime)
    {
        currentSpread = getCurrentSpread(baseSpread, maxSpread, buttonTime);
        float randomizedValue = UnityEngine.Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue,randomizedValue,randomizedValue);
        if(weaponType == Weapon_Type.Shotgun)
            spreadRotation = Quaternion.Euler(0, randomizedValue, 0);
        return spreadRotation * originalDirection; 
    }
    
    //先快后慢的幂函数 保证后坐力越来越大
    private float getCurrentSpread(float a, float b, float t)
    {
        return a + (b - a) * MathF.Pow(t, 2);
    }

    #region Burst Region
    public bool BurstActive() => burstActive;
    public void ToggleBurst()
    {
        if(burstModeAvailable)
            burstActive = !burstActive;
        if(burstActive)
        {
            fireRate = burstModeFireRate;
        }
        else
        {
            fireRate = defaultFireRate;
        }
    }
    #endregion

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

    public bool HaveEnoughBullets() => bulletsInMagazine > 0 ;

    public void Reload()
    {
        int bulletsToReload = magazineCapacity - bulletsInMagazine;
        bulletsToReload = Math.Min(bulletsToReload, totalReserveAmmo);
        bulletsInMagazine += bulletsToReload;
        totalReserveAmmo -= bulletsToReload;
    }
    #endregion
}
