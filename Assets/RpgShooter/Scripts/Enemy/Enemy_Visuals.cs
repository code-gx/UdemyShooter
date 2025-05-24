using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMelee_WeaponModel_Type
{
    OneHand,
    // TwoHand,
    Throw,
}

public class Enemy_Visuals : MonoBehaviour
{
    [Header("Corruption Visual")]
    [SerializeField] private Enemy_CorruptionCrystal[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Weapon Visual")]
    [SerializeField] private Enemy_WeaponModel[] weaponModels;
    [SerializeField] private EnemyMelee_WeaponModel_Type weaponType;
    public GameObject currentWeaponModel { get; private set; }

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Awake()
    {
        weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        corruptionCrystals = GetComponentsInChildren<Enemy_CorruptionCrystal>(true);
    }

    private void Start()
    {
        SetupRandomColor();
        SetupRandomWeaponModel();
        SetupRandomCorruption();
    }

    public void setWeaponModelType(EnemyMelee_WeaponModel_Type type = EnemyMelee_WeaponModel_Type.OneHand)
    {
        weaponType = type;
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
        foreach (var weaponModel in weaponModels)
        {
            weaponModel.gameObject.SetActive(false);
        }

        List<Enemy_WeaponModel> filterModels = new List<Enemy_WeaponModel>();

        foreach (var weaponModel in weaponModels)
        {
            if (weaponType == weaponModel.weaponType)
            {
                filterModels.Add(weaponModel);
            }
        }

        int index = Random.Range(0, filterModels.Count);
        currentWeaponModel = filterModels[index].gameObject;
        currentWeaponModel.SetActive(true);
    }

    private void SetupRandomCorruption()
    {
        List<int> avalibleIndexs = new List<int>();
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
}
