using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WeaponModel : MonoBehaviour
{
    public EnemyMelee_WeaponModel_Type weaponType;
    public AnimatorOverrideController overrideController;

    public Enemy_MeleeWeaponData weaponData;

    [SerializeField] private GameObject[] trails;

    void Awake()
    {
        EnableTrailEffect(false);
    }

    public void EnableTrailEffect(bool enable)
    {
        foreach (var trail in trails)
        {
            trail.SetActive(enable);
        }
    }
}
