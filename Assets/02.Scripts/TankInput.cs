using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TankInput : MonoBehaviour
{
    public PhotonView photonView;
    
    public string moveAxisName = "Vertical";
    public string rotateAxisName = "Horizontal";

    public string mouseXAxisName = "Mouse X";
    public string mouseYAxisName = "Mouse Y";
    
    public float Move { get; private set; }
    public float Rotate { get; private set; }
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public bool OnLeftMouseDown { get; private set; }
    public bool OnLeftMouseUp { get; private set; }
    public bool OnRightMouseDown { get; private set; }
    public bool OnRightMouse { get; private set; }
    public bool OnRightMouseUp { get; private set; }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    
    private void OnEnable()
    {
        Move = 0;
        Rotate = 0;
        MouseX = 0;
        MouseY = 0;
        OnLeftMouseDown = false;
        OnLeftMouseUp = false;
        OnRightMouseDown = false;
        OnRightMouse = false;
        OnRightMouseUp = false;
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        
        Move = Input.GetAxis(moveAxisName);
        Rotate = Input.GetAxis(rotateAxisName);
        MouseX = Input.GetAxis(mouseXAxisName);
        MouseY = Input.GetAxis(mouseYAxisName);
        
        OnLeftMouseDown = Input.GetMouseButtonDown(0);
        OnLeftMouseUp = Input.GetMouseButtonUp(0);
        
        OnRightMouseDown = Input.GetMouseButtonDown(1);
        OnRightMouseUp = Input.GetMouseButtonUp(1);
        OnRightMouse = Input.GetMouseButton(1);
    }
}