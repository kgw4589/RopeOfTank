using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class RopeAction : MonoBehaviour
{
    public Transform player;
    private Rigidbody _playerRigid;
    
    public Transform tip;
    public LayerMask grapplingObject;
    private Camera _camera;
    private RaycastHit _hit;
    private Vector3 _spot;

    private bool _isRopeOk = true;
    
    private LineRenderer _lr;
    private SpringJoint _sj;

    private bool _onGrappling = false;
    private bool _isDash = false;

    private TankInput _tankInput;

    private void Start()
    {
        _tankInput = transform.GetComponent<TankInput>();
        
        _playerRigid = player.gameObject.GetComponent<Rigidbody>();
        _camera = Camera.main;
        _lr = GetComponentInChildren<LineRenderer>();
    }

    private void Update()
    {
        DrawRope();
        if (!_tankInput.photonView.IsMine)
        {
            return;
        }

        if (_tankInput.OnRightMouseDown)
        {
            _isRopeOk = false;
        }
        if (_tankInput.OnLeftMouseDown && _isRopeOk)
        {
            _isRopeOk = false;
            _tankInput.photonView.RPC("RopeShoot", RpcTarget.Others, null);
            RopeShoot();
        }
        else if (_tankInput.OnLeftMouseUp)
        {
            _isRopeOk = true;
            _tankInput.photonView.RPC("EndShoot", RpcTarget.Others, null);
            EndShoot();
        }
        if (_isDash)
        {
            RopeDash();
        }
    }

    [PunRPC]
    private void RopeShoot()
    {
        if (Physics.Raycast(tip.transform.position,tip.transform.forward,
                                out _hit, 200f, grapplingObject))
        {
            _onGrappling = true;

            _spot = _hit.point;
            _lr.positionCount = 2;  
            _lr.SetPosition(0, tip.position);
            _lr.SetPosition(1, _spot);
            
            _sj = player.gameObject.AddComponent<SpringJoint>();
            _sj.autoConfigureConnectedAnchor = false;
            _sj.connectedAnchor = _spot;

            float dis = Vector3.Distance(transform.position, _spot);

            _sj.maxDistance = dis * 0.7f;
            _sj.minDistance = dis * 0.01f;
            _sj.spring = 20000.0f;
            _sj.damper = 150f;
            _sj.breakForce = 1000000000000000f;
        }
    }

    [PunRPC]
    private void EndShoot()
    {
        _onGrappling = false;

        var springJoints = GetComponents<SpringJoint>();
        for (int i = 0; i < springJoints.Length; i++)
        {
            Destroy(springJoints[i]);
        }
        _lr.positionCount = 0;
    }
    
    private void DrawRope()
    {
        if (_onGrappling)
        {
            _lr.SetPosition(0, tip.position);

            if (_tankInput.OnRightMouseDown && !_isDash)
            {
                _playerRigid.velocity = Vector3.zero;
                _isDash = true;
            }
        }
    }

    private void RopeDash()
    {
        var dis = Vector3.Distance(_spot, player.position);
        if (dis > 5f)
        {
            player.position = Vector3.Lerp(player.position, _spot, 0.015f);
        }
        else
        {
            _playerRigid.velocity = Vector3.zero;
            _isDash = false;
        }
    }
}
