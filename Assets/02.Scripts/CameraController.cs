using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject normalTarget;
    public GameObject fireTarget;

    private TankInput _tankInput;

    private bool _isInitOk = false; 

    public void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _tankInput = normalTarget.transform.root.GetComponentInChildren<TankInput>();

        _isInitOk = true;
    }

    private void Update()
    {
        if (!_isInitOk)
        {
            return;
        }
        
        if (_tankInput.OnRightMouse)
        {
            CamFollowFire();
        }
        else
        {
            CamFollowNormal();
        }
    }

    private void CamFollowFire()
    {
        transform.position = Vector3.Lerp(fireTarget.transform.position, transform.position, 0.2f);
        transform.rotation = Quaternion.Lerp(fireTarget.transform.rotation, transform.rotation, 0.2f);
    }

    private void CamFollowNormal()
    {
        transform.position = Vector3.Lerp(normalTarget.transform.position, transform.position, 0.2f);
        transform.rotation = Quaternion.Lerp(normalTarget.transform.rotation, transform.rotation, 0.2f);
    }
}
