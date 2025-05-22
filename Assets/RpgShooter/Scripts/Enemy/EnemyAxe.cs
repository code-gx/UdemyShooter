using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;
    private Transform player;
    private float flySpeed;
    private float rotationSpeed = 1600;

    private Vector3 direction;
    private float timer;

    public void AxeSetup(float flySpeed, Transform player, float timer)
    {
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        if (timer > 0)
            direction = player.transform.position - transform.position + Vector3.up;

        rb.velocity = direction.normalized * flySpeed;

        transform.forward = direction;
    }

    void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        Player player = other.GetComponent<Player>();

        if (bullet != null || player != null)
        {
            GameObject newFx = ObjectPool.instance.GetObject(impactFx);
            newFx.transform.position = transform.position;
            ObjectPool.instance.ReturnToPool(gameObject);
            ObjectPool.instance.DelayReturnObject(newFx, 0.9f);
        }
    }
}
