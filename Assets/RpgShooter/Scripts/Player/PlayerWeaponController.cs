using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering.LookDev;
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
    [SerializeField] private int maxWeaponSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;
    public Transform GunPoint() => gunPoint;
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

    private void EquipStartWeapon()
    {
        currentWeapon = weaponSlots[0];
        player.weaponVisuals.SwitchCurrentWeaponModel();
        player.weaponVisuals.SwitchOnBackupWeaponModels();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + 25 * weaponHolder.transform.forward);
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(gunPoint.position, gunPoint.position + 25 * gunPoint.forward);
    }
    #region Slots management - Pickup\Equip\Drop Weapon
    private void EquipWeapon(int i)
    {
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
            Debug.Log("no slots");
            return;
        }
        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModels();
    }
    #endregion
    private void Shoot()
    {
        if(!currentWeapon.CanShoot())
        {
            return;
        }
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

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;
        controls.Character.Fire.performed += context => Shoot();
        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.Reload.performed += context => 
        {
            if (!currentWeapon.CanReload())
                return;
            player.weaponVisuals.PlayReloadAnimation();
        };
    }
    #endregion
}
