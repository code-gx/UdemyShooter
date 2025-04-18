using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals controller;
    void Start()
    {
        controller = GetComponentInParent<PlayerWeaponVisuals>();
    }

    public void onReloadDone()
    {
        controller.MaximizeRigWeight();
    }

    public void onWeaponGrabSoonDone()
    {
        controller.MaximizeRigWeight();
        controller.MaximizeLeftHandIKWeight();   
    }

    public void onWeaponGrabDone()
    {
        controller.SetGrabBusy(false);
    }
}
