using System;
using Cinemachine;
using UnityEngine;


public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;
    [Header("Aim Laser")]
    [SerializeField] private LineRenderer aimLaser;

    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private bool isLockingTarget;
    [Range(0.5f, 1f)]
    #region unUsed
    [SerializeField] private float minCameraDistance = 1f;
    [Range(1f, 3f)]
    [SerializeField] private float maxCameraDistance = 3;
    [Range(2f, 5f)]
    [SerializeField] private float aimSensetivity = 5;
    #endregion
    [Header("Aim info")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;

    private Vector2 mouseInput;
    private RaycastHit lastMouseHit;
    [Header("Camera info")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float cameraMoveSpeed;

    //cinemachine的body组件
    private CinemachineFramingTransposer cinemachineCompoent;
    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
        cinemachineCompoent = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            isAimingPrecisly = !isAimingPrecisly;
        if(Input.GetKeyDown(KeyCode.L))
            isLockingTarget = !isLockingTarget;
        UpdateAimLaser();
        UpdateAimPosition();
        UpadteCamera();
    }

    private void UpdateAimLaser()
    {
        Transform gunPoint = player.weapon.GunPoint();
        aimLaser.SetPosition(0, gunPoint.position);
        float distance = 5f;
        Vector3 endPoint = gunPoint.forward * distance + gunPoint.position;
        if (Physics.Raycast(gunPoint.position, gunPoint.forward, out var hitInfo, distance ))
        {
            endPoint = hitInfo.point;
        }
        aimLaser.SetPosition(1, endPoint);
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();
        if(target != null && isLockingTarget)
        {
            
            if(target.GetComponent<Renderer>())
            {
                aim.position = target.GetComponent<Renderer>().bounds.center;
            }
            else
                aim.position = target.position;
            return;
        }
        aim.position = GetMouseHitInfo().point;
        if (!GetIsAimPrecisly())
            aim.position = new Vector3(GetMouseHitInfo().point.x, transform.position.y + 1f, GetMouseHitInfo().point.z);
        // aim.position = Vector3.Lerp(aim.position, SetAimPosition(), aimSensetivity * Time.deltaTime);
    }

    private void UpadteCamera()
    {
        if (mouseInput.x > 1520 && mouseInput.y > 780)
        {
            cinemachineCompoent.m_ScreenX = Mathf.Lerp(cinemachineCompoent.m_ScreenX, 0.25f, Time.deltaTime * cameraMoveSpeed);
            cinemachineCompoent.m_ScreenY = Mathf.Lerp(cinemachineCompoent.m_ScreenY, 0.75f, Time.deltaTime * cameraMoveSpeed);
        }
        else if (mouseInput.x > 1520 && mouseInput.y < 300)
        {
            cinemachineCompoent.m_ScreenX = Mathf.Lerp(cinemachineCompoent.m_ScreenX, 0.25f, Time.deltaTime * cameraMoveSpeed);
            cinemachineCompoent.m_ScreenY = Mathf.Lerp(cinemachineCompoent.m_ScreenY, 0.35f, Time.deltaTime * cameraMoveSpeed);
        }
        else if (mouseInput.x < 400 && mouseInput.y < 300)
        {
            cinemachineCompoent.m_ScreenX = Mathf.Lerp(cinemachineCompoent.m_ScreenX, 0.75f, Time.deltaTime * cameraMoveSpeed);
            cinemachineCompoent.m_ScreenY = Mathf.Lerp(cinemachineCompoent.m_ScreenY, 0.35f, Time.deltaTime * cameraMoveSpeed);
        }
        else if (mouseInput.x < 400 && mouseInput.y > 780)
        {
            cinemachineCompoent.m_ScreenX = Mathf.Lerp(cinemachineCompoent.m_ScreenX, 0.75f, Time.deltaTime * cameraMoveSpeed);
            cinemachineCompoent.m_ScreenY = Mathf.Lerp(cinemachineCompoent.m_ScreenY, 0.7f, Time.deltaTime * cameraMoveSpeed);
        }
        else
        {
            cinemachineCompoent.m_ScreenX = Mathf.Lerp(cinemachineCompoent.m_ScreenX, 0.5f, Time.deltaTime * cameraMoveSpeed);
            cinemachineCompoent.m_ScreenY = Mathf.Lerp(cinemachineCompoent.m_ScreenY, 0.5f, Time.deltaTime * cameraMoveSpeed);
        }
    }

    public Transform Target()
    {
        Transform target = null;
        if(GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }
        return target;
    }

    public Transform Aim() => aim;

    public bool GetIsAimPrecisly() => isAimingPrecisly;

    //废弃
    private Vector3 SetAimPosition()
    {
        //摄像机角度 如果向下运动按最大摄像机距离会跑出屏幕
        bool moveingDownwards = player.movement.moveInput.y < -0.5f;
        float actualMaxCameraDistance = moveingDownwards ? minCameraDistance: maxCameraDistance;
        // print(actualMaxCameraDistance);
        Vector3 desiredAimPosition = GetMouseHitInfo().point;
        print(desiredAimPosition);
        Vector3 aimDirection = desiredAimPosition - transform.position;
        
        aimDirection.y = 0;
        aimDirection.Normalize();
        float distanceToDesiredAimPosition = MathF.Sqrt(Mathf.Pow(desiredAimPosition.x - transform.position.x,2)
                                            + Mathf.Pow(desiredAimPosition.y - transform.position.y, 2));
        float clampDistance = Mathf.Clamp(minCameraDistance, actualMaxCameraDistance, distanceToDesiredAimPosition);
        desiredAimPosition = transform.position + aimDirection * clampDistance;
        desiredAimPosition.y = transform.position.y + 1;
        return desiredAimPosition;
    }
    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastMouseHit = hitInfo;
            return hitInfo;
        }
        return lastMouseHit;
    }
    private void AssignInputEvents()
    {
        controls = player.controls;
        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }
}
