using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    
    [Header("Bullet details")]
    [SerializeField] private Weapon currentWeapon;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    //通过速度修改质量的基准速度
    private const float REFRENCE_BULLET_SPEED = 20;

    [Header("Inventory")]
    [SerializeField] private List<Weapon> weaponSlots;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
        currentWeapon.ammo = currentWeapon.maxAmmo;
    }

    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;
        controls.Character.Fire.performed += context => Shoot();
        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
    }

    private void EquipWeapon(int i)
    {
        currentWeapon = weaponSlots[i];
    }

    private void DropWeapon()
    {
        if(weaponSlots.Count <= 1)
        {
            return;
        }
        weaponSlots.Remove(currentWeapon);
    }
    private void Shoot()
    {
        if(currentWeapon.ammo <= 0)
        {
            return;
        }
        currentWeapon.ammo--;
        Transform aim = player.aim.Aim();
        gunPoint.LookAt(aim);
        weaponHolder.LookAt(aim);
        GameObject newBullet = 
                Instantiate(bulletPrefab, gunPoint.position,Quaternion.LookRotation(gunPoint.forward));
        newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed;
        newBullet.GetComponent<Rigidbody>().mass = REFRENCE_BULLET_SPEED /bulletSpeed;
        Destroy(newBullet, 10);
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + 25 * weaponHolder.transform.forward);
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(gunPoint.position, gunPoint.position + 25 * gunPoint.forward);
    }

    public Transform GunPoint() => gunPoint;
}
