using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    private TrailRenderer trailRenderer;
    [SerializeField] private GameObject bulletImpactFX;
    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;
    public void BulletSetup(float flyDistance, float impactForce)
    {
        this.impactForce = impactForce;
        bulletDisabled = false;
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        UpdateTrailTime();
        CheckDisabled();
        CheckReturnPool();
    }

    private void CheckReturnPool()
    {
        if (trailRenderer.time < 0)
        {
            ObjectPool.instance.ReturnToPool(gameObject);
        }
    }

    private void CheckDisabled()
    {
        if (Vector3.Distance(transform.position, startPosition) >= flyDistance && !bulletDisabled)
        {
            // ObjectPool.instance.ReturnBullet(gameObject);
            boxCollider.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    private void UpdateTrailTime()
    {
        if (Vector3.Distance(transform.position, startPosition) >= flyDistance - 1.5)
            trailRenderer.time -= 3.5f * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        CreateImpactFx(other);
        ObjectPool.instance.ReturnToPool(gameObject);

        // rb.constraints = RigidbodyConstraints.FreezeAll;
        Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
        EnemyShield shield = other.gameObject.GetComponent<EnemyShield>();

        if (shield != null)
        {
            shield.ReduceDurablity();
            return;
        }


        if (enemy != null)
            {
                Vector3 force = rb.velocity.normalized * impactForce;
                var contact = other.contacts[0];
                Rigidbody hitRigidBody = other.collider.attachedRigidbody;
                enemy.GetHit();
                enemy.HitImpact(force, contact.point, hitRigidBody);
            }
    }

    private void CreateImpactFx(Collision other)
    {
        if (other.contacts.Length > 0)
        {
            var contact = other.contacts[0];
            GameObject newImpactFx = ObjectPool.instance.GetObject(bulletImpactFX);
            newImpactFx.transform.position = contact.point;
            ObjectPool.instance.DelayReturnObject(newImpactFx, 0.8f);
        }
    }
}
