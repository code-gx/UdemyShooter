using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Visuals : MonoBehaviour
{
    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void SetupRandomColor()
    {
        int index = Random.Range(0, colorTextures.Length);
        Material newMat = new Material(skinnedMeshRenderer.material);
        newMat.mainTexture = colorTextures[index];
        skinnedMeshRenderer.material = newMat;
    }
    private void Start()
    {
        SetupRandomColor();
    }
}
