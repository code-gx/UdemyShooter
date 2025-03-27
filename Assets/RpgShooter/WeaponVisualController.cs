using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum Weapon_Grab_Type
{
    SideGrab,
    BackGrab,
}

public class WeaponVisualController : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Transform[] gunTransforms;
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotGun;
    [SerializeField] private Transform sniperRifle;

    private Transform currentGun;
    [Header("left hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;

    [Header("rig")]
    private Rig rig;
    private bool rigShouldBeIncreased;
    [SerializeField] private float rigIncreaseSpeed;

    private void Start()
    {
        SwitchOn(pistol);
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
    }

    private void Update()
    {
        CheckWeaponSwitch();
        if(Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Reload");
            PauseRig();
        }

        if (rigShouldBeIncreased)
        {
            rig.weight += rigIncreaseSpeed * Time.deltaTime;
            if(rig.weight >= 1)
            {
                rigShouldBeIncreased = false;
            }
        }
    }

    private void PauseRig()
    {
        rig.weight = 0.15f;
    }

    public void ReturnRigWeightToOne() => rigShouldBeIncreased = true;
    private void PlayWeaponGrabAnimation(Weapon_Grab_Type grabType)
    {
        PauseRig();
        leftHandIK.weight = 0;
        anim.SetBool("BusyGrabbingWeapon", true);
        anim.SetFloat("WeaponGrabType", (float)grabType);
        anim.SetTrigger("WeaponGrab");
    }
    
    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(pistol);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(Weapon_Grab_Type.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(revolver);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(Weapon_Grab_Type.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(autoRifle);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(Weapon_Grab_Type.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(shotGun);
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(Weapon_Grab_Type.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(sniperRifle);
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(Weapon_Grab_Type.BackGrab);
        }
    }

    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        currentGun = gunTransform;
        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransforms.Length; i ++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTransform>().transform;
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            if(i == layerIndex)
            {
                anim.SetLayerWeight(i, 1);
            }
            else
            {
                anim.SetLayerWeight(i, 0);
            }
        }
    }
}
