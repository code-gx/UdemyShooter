using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals visualController;
    private PlayerWeaponController playerWeaponController;
    void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
        playerWeaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void onReloadDone()
    {
        visualController.MaximizeRigWeight();
        playerWeaponController.CurrentWeapon().Reload();
        playerWeaponController.SetWeaponReady(true);
    }

    public void onWeaponGrabSoonDone()
    {
        visualController.MaximizeRigWeight();
        visualController.MaximizeLeftHandIKWeight();   
    }

    public void onWeaponGrabDone()
    {
        playerWeaponController.SetWeaponReady(true);
    }

    public void SwitchWeaponModel() => visualController.SwitchCurrentWeaponModel();
}
