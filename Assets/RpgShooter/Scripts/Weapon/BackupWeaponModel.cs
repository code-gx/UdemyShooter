using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Hang_Type {
    LowBackHang,
    BackHang,
    SdieHang,
}
public class BackupWeaponModel : MonoBehaviour
{
    public Weapon_Type weaponType;
    public Hang_Type hangType;

    public void Activate(bool flag) => this.gameObject.SetActive(flag);
}
