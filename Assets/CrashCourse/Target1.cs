using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target1 : MonoBehaviour
{
    [SerializeField] private Material gotHitMaterial;
    
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Bullet")
        {
            GetComponent<MeshRenderer>().material = gotHitMaterial;
        }
    }
}
