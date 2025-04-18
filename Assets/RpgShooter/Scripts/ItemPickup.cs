using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerWeaponController>()? .PickupWeapon(weapon);
    }
}
