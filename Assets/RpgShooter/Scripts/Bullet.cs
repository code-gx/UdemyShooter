using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject bulletImpactFX;
    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;
    public void BulletSetup(float flyDistance)
    {
        bulletDisabled = false;
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, startPosition) >= flyDistance - 1.5)
            trailRenderer.time -= 3 * Time.deltaTime;
        if(Vector3.Distance(transform.position, startPosition) >= flyDistance && !bulletDisabled)
        {
            // ObjectPool.instance.ReturnBullet(gameObject);
            boxCollider.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
        if(trailRenderer.time < 0)
        {
            ObjectPool.instance.ReturnBullet(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        // rb.constraints = RigidbodyConstraints.FreezeAll;
        print("彭值11111");
        CreateImpactFx(other);
        ObjectPool.instance.ReturnBullet(gameObject);
    }

    private void CreateImpactFx(Collision other)
    {
        if (other.contacts.Length > 0)
        {
            var contact = other.contacts[0];
            GameObject newImpactFx = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(newImpactFx, 1.0f);
        }
    }

    void OnDestroy()
    {
        UnityEngine.Debug.Log("销毁");
    }
}
