using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunTransforms;
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotGun;
    [SerializeField] private Transform sniperRifle;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(pistol);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(revolver);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(autoRifle);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(shotGun);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(sniperRifle);
        }
    }

    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransforms.Length; i ++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }
}
