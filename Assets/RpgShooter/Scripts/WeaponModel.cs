using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon_Equip_Type {
    SideEquip,
    BackEquip
}

//动画层索引
public enum Weapon_Hold_Type {
    CommonHold = 1,
    LowHold,
    HighHold
}

public class WeaponModel : MonoBehaviour
{
    public Weapon_Type weaponType;
    public Weapon_Equip_Type equipType;
    public Weapon_Hold_Type holdTpye;

    public Transform gunPoint;
    public Transform holdPoint; // 左手ik位置 
}
