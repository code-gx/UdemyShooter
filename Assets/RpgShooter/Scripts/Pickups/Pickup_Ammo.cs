using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoBox_Type
{
    BoxAmmoSmall,
    BoxAmmoBig,
}

[Serializable]
public struct AmmoData
{    public Weapon_Type weapon_Type;
    [Range(10, 100)] public int maxAmount;
    [Range(10, 100)] public int minAmount;
}


public class Pickup_Ammo : Interactable
{
    [SerializeField] private AmmoBox_Type ammoBoxType;
    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;
    protected override void Start()
    {
        base.Start();
        SetupBoxModel();
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;
        if(ammoBoxType == AmmoBox_Type.BoxAmmoBig)
            currentAmmoList = bigBoxAmmo;
        bool isAddAmmo = false;
        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weapon_Type);
            bool flag = AddBulletsToWeapon(weapon, ammo);
            if (flag == true)
                isAddAmmo = true;
        }
        if (isAddAmmo)
            ObjectPool.instance.ReturnToPool(gameObject);
    }

    private bool AddBulletsToWeapon(Weapon weapon, AmmoData ammo)
    {
        if (weapon != null)
        {
            weapon.totalReserveAmmo += GetBulletAmount(ammo);
            weapon.bulletsInMagazine = weapon.magazineCapacity;
            return true;
        }
        return false;
    }

    private int GetBulletAmount(AmmoData ammoData)
    {
        int minAmount = Math.Min(ammoData.minAmount, ammoData.maxAmount);
        int maxAmount = Math.Max(ammoData.minAmount, ammoData.maxAmount);
        return UnityEngine.Random.Range(minAmount, maxAmount);
    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            if (i == ((int)ammoBoxType))
            {
                boxModel[i].SetActive(true);
                mesh = boxModel[i].GetComponent<MeshRenderer>();
            }
            else
                boxModel[i].SetActive(false);
        }
    }
}
