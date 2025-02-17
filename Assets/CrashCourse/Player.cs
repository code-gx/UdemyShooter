using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    [Header("Gun data")]
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bulletPrefab;
    
    [Header("Movemenet data")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    private Rigidbody rb;
    private float verticalInput;
    private float horizontalInput;
    [Header("Tower data")]
    [SerializeField] private Transform tankTowerTransform;
    [SerializeField] private float towerRotationSpeed;

    [Header("Aim data")]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private LayerMask whatIsAimMask;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        UpdataAim();
        CheckInputs();
    }

    void FixedUpdate() {
        // move
        ApplyMovement();
        //rotate
        ApplyBodyRotation();
        //aim
        ApplyTowerRotation();
    }

    private void CheckInputs()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }

        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if(verticalInput < 0)
        {
            horizontalInput = -Input.GetAxis("Horizontal");
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = gunPoint.forward * bulletSpeed;

        Destroy(bullet, 7);
    }
    private void ApplyMovement()
    {
        Vector3 movement = transform.forward * moveSpeed * verticalInput;
        rb.velocity = movement;
    }

    private void ApplyBodyRotation()
    {
        transform.Rotate(new Vector3(0, rotationSpeed * horizontalInput, 0));
    }

    private void ApplyTowerRotation()
    {
        Vector3 direction = aimTransform.position - tankTowerTransform.position;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        tankTowerTransform.rotation = Quaternion.RotateTowards(tankTowerTransform.rotation, targetRotation, towerRotationSpeed);
    }

    private void UpdataAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, whatIsAimMask))
        {
            Vector3 pos = new Vector3(hit.point.x, aimTransform.position.y, hit.point.z);
            aimTransform.position = pos;
        }
    }
}
