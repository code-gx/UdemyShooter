using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Player player;
    private Animator anim;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;
    
    [Header("left hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private float leftHandIKIncreaseRate;
    private bool shouldIncreaseLeftHandIKWeight;

    [Header("rig")]
    private Rig rig;
    [SerializeField] private float rigWeightIncreaseRate;
    private bool shouldIncreaseRigWeight;


    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
        player = GetComponent<Player>();
        //先保证currentweapon初始化完
        // Invoke("InitWeaponReloadAndEquipSpeed", 0.11f);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    #region  rig Animation Method
    public void MaximizeRigWeight() => shouldIncreaseRigWeight = true;
    public void MaximizeLeftHandIKWeight() => shouldIncreaseLeftHandIKWeight = true;
    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncreaseLeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKIncreaseRate * Time.deltaTime;
            if (leftHandIK.weight >= 1)
            {
                shouldIncreaseLeftHandIKWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncreaseRigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;
            if (rig.weight >= 1)
            {
                shouldIncreaseRigWeight = false;
            }
        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }


    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;
    }
    #endregion
    
    public void SwitchCurrentWeaponModel()
    {
        SwitchAnimationLayer((int)CurrentWeaponModel().holdTpye);
        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();
        SwitchOnBackupWeaponModels();
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i ++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    public void SwitchOffBackupWeaponModels()
    {
        foreach (var backupWeapon in backupWeaponModels)
        {
            backupWeapon.Activate(false);
        }
    }

    public void SwitchOnBackupWeaponModels()
    {
        if (player.weapon.isHasOnlyOneWeapon())
            return;
        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;
        foreach (var backupWeaponModel in backupWeaponModels)
        {
            if (player.weapon.CurrentWeapon().weaponType == backupWeaponModel.weaponType)
                continue;
            if (player.weapon.WeaponInSlots(backupWeaponModel.weaponType) != null)
            {
                if (backupWeaponModel.hangType == Hang_Type.LowBackHang)
                {
                    lowHangWeapon = backupWeaponModel;
                }
                else if(backupWeaponModel.hangType == Hang_Type.BackHang)
                {
                    backHangWeapon = backupWeaponModel;
                }
                else if(backupWeaponModel.hangType == Hang_Type.SdieHang)
                {
                    sideHangWeapon = backupWeaponModel;
                }
            }

            lowHangWeapon ?.Activate(true);
            backHangWeapon ?.Activate(true);
            sideHangWeapon ?.Activate(true);
        }
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

    public WeaponModel CurrentWeaponModel()
    {
        Weapon currentWeapn = player.weapon.CurrentWeapon(); 
        return GetWeaponModel(currentWeapn.weaponType);
    }

    public WeaponModel GetWeaponModel(Weapon_Type type)
    {
        for(int i = 0; i < weaponModels.Length; i++)
        {
            if(weaponModels[i].weaponType == type)
            {
                return weaponModels[i];
            }
        }
        return null;
    }

    public BackupWeaponModel GetBackupWeaponModel(Weapon_Type type)
    {
        for(int i = 0; i < backupWeaponModels.Length; i++)
        {
            if(backupWeaponModels[i].weaponType == type)
            {
                return backupWeaponModels[i];
            }
        }
        return null;
    }

    public void InitWeaponReloadAndEquipSpeed()
    {
        anim.SetFloat("EquipSpeed", player.weapon.CurrentWeapon().equipSpeed);
        anim.SetFloat("ReloadSpeed", player.weapon.CurrentWeapon().reloadSpeed);
    }

    public void PlayFireAnimation() => anim.SetTrigger("Fire");

    public void PlayReloadAnimation()
    {
        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }
    public void PlayWeaponEquipAnimation()
    {
        Weapon_Equip_Type equipType = CurrentWeaponModel().equipType;
        Weapon currentWeapon = player.weapon.CurrentWeapon();
        //主要是手腕ik 影响了切枪的动作
        ReduceRigWeight();
        //记得重置ik增加的开关
        shouldIncreaseRigWeight = false;
        shouldIncreaseLeftHandIKWeight = false;
        leftHandIK.weight = 0;
        anim.SetFloat("EquipSpeed", currentWeapon.equipSpeed);
        anim.SetFloat("ReloadSpeed", currentWeapon.reloadSpeed);
        anim.SetFloat("EquipType", (float)equipType);
        anim.SetTrigger("EquipWeapon");
    }
}
