using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform aim;

    private void Start()
    {
        player = GetComponent<Player>();
        player.controls.Character.Fire.performed += context => Shoot();
    }
    private void Shoot()
    {
        gunPoint.LookAt(aim);
        weaponHolder.LookAt(aim);
        GameObject newBullet = 
                Instantiate(bulletPrefab, gunPoint.position,Quaternion.LookRotation(gunPoint.forward));
        newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletSpeed;
        Destroy(newBullet, 10);
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + 25 * weaponHolder.transform.forward);
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(gunPoint.position, gunPoint.position + 25 * gunPoint.forward);
    }
}
