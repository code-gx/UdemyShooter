using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls controls;
    public PlayerAim aim { get; private set;} // read only
    public PlayerMovement movement {get;private set;}
    public PlayerWeaponController weapon {get; private set;}
    public PlayerWeaponVisuals weaponVisuals {get; private set;}
    public PlayerInteraction interaction{get; private set;}

    private void Awake() {
        controls = new PlayerControls();
        aim = GetComponent<PlayerAim>();
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<PlayerWeaponController>();
        weaponVisuals = GetComponent<PlayerWeaponVisuals>();
        interaction = GetComponent<PlayerInteraction>();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }
}
