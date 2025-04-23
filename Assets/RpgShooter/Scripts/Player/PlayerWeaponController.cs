using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    private bool weaponReady;
    private bool isShooting;
    
    [Header("Bullet details")]
    [SerializeField] private Weapon currentWeapon;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform weaponHolder;

    //通过速度修改质量的基准速度
    private const float REFRENCE_BULLET_SPEED = 20;

    [Header("Inventory")]
    [SerializeField] private int maxWeaponSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;
    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;
    public Weapon CurrentWeapon() => currentWeapon;
    public bool isHasOnlyOneWeapon() => weaponSlots.Count <= 1;
    public Weapon BackupWeapon()
    {
        foreach(var weapon in weaponSlots)
        {
            if(weapon != currentWeapon)
            {
                return weapon;
            }
        }
        return null;
    }
    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
        Invoke("EquipStartWeapon", 0.1f);
    }

    private void Update()
    {
        if(isShooting)
            Shoot();
    }

    private void EquipStartWeapon()
    {
        currentWeapon = weaponSlots[0];
        SetWeaponReady(true);
        player.weaponVisuals.SwitchCurrentWeaponModel();
        player.weaponVisuals.SwitchOnBackupWeaponModels();
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + 25 * weaponHolder.transform.forward);
    //     Gizmos.color = Color.yellow;

    //     Gizmos.DrawLine(GunPoint().position, GunPoint().position + 25 * GunPoint().forward);
    // }
    #region Slots management - Pickup\Equip\Drop\Ready Weapon
    private void EquipWeapon(int i)
    {
        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
    }

    private void DropWeapon()
    {
        if(isHasOnlyOneWeapon()) return;
        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }
    
    public void PickupWeapon(Weapon newWeapon)
    {
        if(weaponSlots.Count >= maxWeaponSlots)
        {
            return;
        }
        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModels();
    }
    
    public void SetWeaponReady(bool ready) => weaponReady = ready;
    public bool GetWeaponReady() => weaponReady;
    #endregion
    private void Shoot()
    {
        if (!GetWeaponReady())
            return;
        if(!currentWeapon.CanShoot())
        {
            return;
        }
        if(currentWeapon.shootType == Shoot_Type.Single)
            isShooting = false;
        GameObject newBullet = ObjectPool.instance.GetBullet();
                // Instantiate(bulletPrefab, gunPoint.position,Quaternion.LookRotation(gunPoint.forward));
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);
        newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed;
        newBullet.GetComponent<Rigidbody>().mass = REFRENCE_BULLET_SPEED /bulletSpeed;
        player.weaponVisuals.PlayFireAnimation();
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;
        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;
        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.Reload.performed += context => 
        {
            if (currentWeapon.CanReload()&&GetWeaponReady())
            {
                Reload();
            }
        };
    }
    #endregion
}
