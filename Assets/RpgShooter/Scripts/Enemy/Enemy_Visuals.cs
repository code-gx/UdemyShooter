using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum EnemyMelee_WeaponModel_Type
{
    OneHand,
    // TwoHand,
    Throw,
    Unarmed,
}

public enum EnemyRange_WeaponModel_Type
{
    Pistol,
    Revolver,
    Shotgun,
    AutoRifle,
    Rifle,
}

public class Enemy_Visuals : MonoBehaviour
{
    [Header("Corruption Visual")]
    [SerializeField] private Enemy_CorruptionCrystal[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;
    public GameObject currentWeaponModel;

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Rig reference")]
    [SerializeField] private Transform leftHandIK;
    [SerializeField] private Transform leftElbowIK;
    [SerializeField] private Rig rig;

    private void Start()
    {
        SetupRandomColor();
        SetupRandomCorruption();
        SetupRandomWeaponModel();
    }

    public void EnabelWeaponTrail(bool enable)
    {
        currentWeaponModel.GetComponent<Enemy_WeaponModel>().EnableTrailEffect(enable);
    }

    private void SetupRandomColor()
    {
        int index = Random.Range(0, colorTextures.Length);
        Material newMat = new Material(skinnedMeshRenderer.material);
        newMat.mainTexture = colorTextures[index];
        skinnedMeshRenderer.material = newMat;
    }

    private void SetupRandomWeaponModel()
    {
        bool thisEnemyIsMelee = GetComponent<Enemy_Melee>() != null;
        bool thisEnemyIsRange = GetComponent<Enemy_Range>() != null;

        if (thisEnemyIsRange)
        { 
            currentWeaponModel = FindeRangeWeaponModel();
            GetComponent<Enemy_Range>().gunPoint =
                currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().gunPoint;
        }

        if (thisEnemyIsMelee)
            currentWeaponModel = FindMeleeWeaponModel();
        currentWeaponModel.SetActive(true);
        OverrideAnimationController();
    }

    private void SetupRandomCorruption()
    {
        List<int> avalibleIndexs = new List<int>();
        corruptionCrystals = GetComponentsInChildren<Enemy_CorruptionCrystal>(true);
        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            avalibleIndexs.Add(i);
            corruptionCrystals[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < corruptionAmount; i++)
        {
            if (avalibleIndexs.Count == 0)
                break;
            int randomIndex = Random.Range(0, avalibleIndexs.Count);
            int objectIndex = avalibleIndexs[randomIndex];
            corruptionCrystals[objectIndex].gameObject.SetActive(true);
            avalibleIndexs.RemoveAt(randomIndex);
        }
    }

    private GameObject FindMeleeWeaponModel()
    {
        Enemy_WeaponModel[] weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        List<Enemy_WeaponModel> filterModels = new List<Enemy_WeaponModel>();

        EnemyMelee_WeaponModel_Type weaponType = GetComponent<Enemy_Melee>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponType == weaponModel.weaponType)
            {
                filterModels.Add(weaponModel);
            }
        }

        int index = Random.Range(0, filterModels.Count);
        return filterModels[index].gameObject;
    }

    private GameObject FindeRangeWeaponModel()
    {
        Enemy_RangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_RangeWeaponModel>(true);
        EnemyRange_WeaponModel_Type weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponType == weaponModel.weaponType)
            {
                SwitchAnimationLayer((int)weaponModel.weaponHoldType);
                SetupLeftHandIK(weaponModel.leftHandIK, weaponModel.leftElbowIK);
                return weaponModel.gameObject;
            }
        }
        Debug.Log("未找到远程武器");
        return null;
    }

    private void OverrideAnimationController()
    {
        AnimatorOverrideController overrideController =
                    currentWeaponModel.GetComponent<Enemy_WeaponModel>()?.overrideController;
        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }
    private void SwitchAnimationLayer(int layerIndex)
    {
        Animator anim = GetComponentInChildren<Animator>();
        for (int i = 1; i < anim.layerCount; i++)
        {
            if (i == layerIndex)
            {
                anim.SetLayerWeight(i, 1);
            }
            else
            {
                anim.SetLayerWeight(i, 0);
            }
        }
    }

    public void EnableIK(bool enable)
    {
        rig.weight = enable ? 1 : 0;
    }

    private void SetupLeftHandIK(Transform leftHand, Transform leftElbow)
    {
        leftHandIK.localPosition = leftHand.localPosition;
        leftHandIK.localRotation = leftHand.localRotation;

        leftElbowIK.localPosition = leftElbow.localPosition;
        leftElbowIK.localRotation = leftElbow.localRotation;
    }
}
