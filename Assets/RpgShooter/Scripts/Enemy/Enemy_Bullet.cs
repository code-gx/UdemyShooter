using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Enemy_Bullet : Bullet
{
    protected override void OnCollisionEnter(Collision other)
    {
        CreateImpactFx(other);
        ObjectPool.instance.ReturnToPool(gameObject);

        Player player = other.gameObject.GetComponentInParent<Player>();
        // if (player != null)
        // {
        //     Debug.Log("Hit player");
        // }
    }
}
