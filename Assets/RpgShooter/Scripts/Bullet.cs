using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private void OnCollisionEnter(Collision other)
    {
        // rb.constraints = RigidbodyConstraints.FreezeAll;
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
}
