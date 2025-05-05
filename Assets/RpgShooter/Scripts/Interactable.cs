using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public MeshRenderer mesh;
    protected Material defaultMaterial;
    [SerializeField] protected Material highLightMaterial;
    protected PlayerWeaponController weaponController;


    protected virtual void Start()
    {
        if (mesh == null)
        {
            mesh = GetComponentInChildren<MeshRenderer>();
        }
        defaultMaterial = mesh.material;
    }

    public virtual void Interaction()
    {
        Debug.Log("Interacted with" + gameObject.name);
    }

    public void HighlightActive(bool active)
    {
        if (active)
        {
            mesh.material = highLightMaterial ? highLightMaterial :mesh.material;
        }
        else
            mesh.material = defaultMaterial;  
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (weaponController == null)
            weaponController = other.GetComponent<PlayerWeaponController>();
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if(playerInteraction == null)
        {
            return;
        }
        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpadateClosestInteractable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        if(playerInteraction == null)
        {
            return;
        }
        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpadateClosestInteractable();
    }
}
