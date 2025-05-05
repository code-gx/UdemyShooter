using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Weapon : Interactable
{
    [SerializeField] private Weapon_Data weaponData;

    //如果序列化了 weapon变量就不为空 没有序列化
    [SerializeField] private Weapon weapon;
    [SerializeField] private BackupWeaponModel[] models;
    private bool isFirstTime = true;

    protected override void Start()
    {
        base.Start();
        // weapon = weapon == null? new Weapon(weaponData): weapon;
        if(isFirstTime)
            weapon = new Weapon(weaponData);
        SetupGameObject();
    }

    /// <summary>
    /// 和start的时序问题 SetupPickupWeapon先执行 start后执行 所以需要仍武器传参时绕过start函数
    /// 两种方法 1.weapon = weapon == null? new Weapon(weaponData): weapon 和 private Weapon weapon
    /// 配套使用 因为游戏启动时weapon为null 
    /// 2.加一个bool变量绕过start
    /// </summary>
    public void SetupPickupWeapon(Weapon weapon, Transform transform)
    {
        isFirstTime = false;
        this.weaponData = weapon.weaponData;
        this.weapon = weapon;
        this.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        SetupGameObject();
    }

    [ContextMenu("Update Item Model")]
    public void SetupGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();
        SetupWeaponModel();
    }
    private void SetupWeaponModel()
    {
        foreach (var model in models)
        {
            if (model.weaponType == weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                mesh = model.gameObject.GetComponent<MeshRenderer>();
            }
            else
                model.gameObject.SetActive(false);
        }
    } 
    public override void Interaction()
    {
        weaponController?.PickupWeapon(weapon);
        ObjectPool.instance.ReturnToPool(gameObject);
    }
}
