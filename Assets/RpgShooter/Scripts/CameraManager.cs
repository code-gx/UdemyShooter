using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer  transposer;
    private float targetCameraDistance;
    [Header("Camera Distance")]
    [SerializeField] private bool canChangeDistance;
    [SerializeField] private float cameraDistanceLerpRate;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void SetCameraDistance(float distance) => targetCameraDistance = distance;

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (!canChangeDistance) return;
        if(MathF.Abs(transposer.m_CameraDistance - targetCameraDistance) < 0.01f)
        {
            transposer.m_CameraDistance = targetCameraDistance;
            return;
        }
        transposer.m_CameraDistance = 
            Mathf.Lerp(transposer.m_CameraDistance, targetCameraDistance, cameraDistanceLerpRate * Time.deltaTime);
    }
}
