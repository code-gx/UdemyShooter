using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private WeaponVisualController controller;
    void Start()
    {
        controller = GetComponentInParent<WeaponVisualController>();
    }

    public void onReloadDone()
    {
        controller.ReturnRigWeightToOne();
    }

    public void onWeaponGrabDone()
    {
        controller.ReturnRigWeightToOne();
    }
}
