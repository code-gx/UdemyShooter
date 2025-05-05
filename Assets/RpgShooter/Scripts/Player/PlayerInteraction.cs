using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public List<Interactable> interactables = new List<Interactable>();
    public Interactable closestInteractable;

    private void Start()
    {
        Player player= GetComponent<Player>();
        player.controls.Character.Interaction.performed += context => InteractWithClosest();
    }
    private void InteractWithClosest()
    {
        closestInteractable?.Interaction();
        interactables.Remove(closestInteractable);
        UpadateClosestInteractable();
    }

    public void UpadateClosestInteractable()
    {
        closestInteractable?.HighlightActive(false);
        closestInteractable = null;
        float closestDistance = float.MaxValue;
        foreach (Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(interactable.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
        closestInteractable?.HighlightActive(true);
    }

    public List<Interactable> GetInteractables() => interactables;

}
