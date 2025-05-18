using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    private bool weaponReady;
    private bool isShooting;

    [SerializeField] Weapon_Data defaultWeaponData;

    [Header("Bullet details")]
    [SerializeField] private float bulletImpact = 100;
    [SerializeField] private Weapon currentWeapon;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform weaponHolder;

    //通过速度修改质量的基准速度
    private const float REFRENCE_BULLET_SPEED = 20;

    [Header("Inventory")]
    [SerializeField] private int maxWeaponSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    [Header("后座力时间参数")]
    public float fireButtonTime = 0;
    public float maxButtonTime = 3.0f;  //最大后座力时间

    [SerializeField] private GameObject weaponPickupPrefab;
    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;
    public Weapon CurrentWeapon() => currentWeapon;
    public bool isHasOnlyOneWeapon() => weaponSlots.Count <= 1;
    public Weapon WeaponInSlots(Weapon_Type weaponType)
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
            {
                return weapon;
            }
        }
        return null;
    }
    public float getFireButtonDownTimeNormalized() => fireButtonTime / maxButtonTime;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
        Invoke("EquipStartWeapon", 0.1f);
        // EquipWeapon(0);
    }

    private void Update()
    {
        if (isShooting)
            Shoot();
        UpdateFireButtonTime();
    }

    private void UpdateFireButtonTime()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (currentWeapon.shootType == Shoot_Type.Auto && currentWeapon.HaveEnoughBullets() && GetWeaponReady())
                fireButtonTime += Time.deltaTime;
            fireButtonTime = Mathf.Min(fireButtonTime, maxButtonTime);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            fireButtonTime = 0;
        }
    }

    private void EquipStartWeapon()
    {
        weaponSlots[0] = new Weapon(defaultWeaponData);
        EquipWeapon(0);
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
        if (i >= weaponSlots.Count)
            return;
        if(currentWeapon == weaponSlots[i])
            return;
        SetWeaponReady(false);
        //数据先动 动画播放也是前端效果 不然会出错
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
        CameraManager.instance.SetCameraDistance(currentWeapon.cameraDistance);
    }

    private void DropWeapon()
    {
        if (isHasOnlyOneWeapon()) return;

        CreateWeaponOntheGround();

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOntheGround()
    {
        GameObject dropWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab);
        dropWeapon.GetComponent<Pickup_Weapon>().SetupPickupWeapon(currentWeapon, transform);
    }

    public void PickupWeapon(Weapon weapon)
    {
        Weapon newWeapon = weapon;
        Weapon oldWeapon = WeaponInSlots(newWeapon.weaponType);

        //有枪就补充子弹
        if (oldWeapon != null)
        {
            currentWeapon.totalReserveAmmo += oldWeapon.totalReserveAmmo;
            currentWeapon.bulletsInMagazine += oldWeapon.bulletsInMagazine;
            if (currentWeapon.bulletsInMagazine > currentWeapon.magazineCapacity)
            {
                currentWeapon.totalReserveAmmo += currentWeapon.bulletsInMagazine - currentWeapon.bulletsInMagazine;
                currentWeapon.bulletsInMagazine = currentWeapon.magazineCapacity;
            }
            return;
        }

        //没有对应的枪并且槽满了 换枪
        if(weaponSlots.Count >= maxWeaponSlots && oldWeapon == null)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;
            CreateWeaponOntheGround();
            EquipWeapon(weaponIndex);
            return;
        }

        //没枪但是没满
        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModels();
    }
    
    public void SetWeaponReady(bool ready) => weaponReady = ready;
    public bool GetWeaponReady() => weaponReady;
    #endregion

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);
        //取二者最小值
        int bullets = Math.Min(currentWeapon.bulletsPerShot, currentWeapon.bulletsInMagazine);
        for(int i = 1; i <= bullets; i++)
        {
            FireSingleBullet();
            yield return new WaitForSeconds(currentWeapon.burstFireDely);
            if (i >= bullets)
                SetWeaponReady(true);
        }
    }
    private void Shoot()
    {
        if (!GetWeaponReady())
            return;
        if (!currentWeapon.CanShoot())
        {
            return;
        }
        player.weaponVisuals.PlayFireAnimation();

        if (currentWeapon.shootType == Shoot_Type.Single)
            isShooting = false;
        if (currentWeapon.BurstActive())
        {
            StartCoroutine(BurstFire());
            return;
        }
        FireSingleBullet();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine --;
        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        // Instantiate(bulletPrefab, gunPoint.position,Quaternion.LookRotation(gunPoint.forward));
        newBullet.transform.position = GunPoint().position;
        //很关键 如果复用对象池 get对象时先active 再设置位置 会造成拖尾不正常的效果
        newBullet.GetComponent<TrailRenderer>().Clear();
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);
        Vector3 bulletDirection = currentWeapon.ApplySpread(newBullet.transform.forward, getFireButtonDownTimeNormalized());
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(currentWeapon.gunDistance, bulletImpact);
        
        newBullet.GetComponent<Rigidbody>().velocity = bulletDirection * bulletSpeed;
        newBullet.GetComponent<Rigidbody>().mass = REFRENCE_BULLET_SPEED / bulletSpeed;
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
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurst();
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
