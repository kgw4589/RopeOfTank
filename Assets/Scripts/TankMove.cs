using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof (TankInput))]
public class TankMove : MonoBehaviour, IDamagable
{
    private float _moveSpeed = 30f;
    private float _rotateSpeed = 75f;

    private Slider _hpSlider;
    private const float MaxHp = 100;
    private float _currentHp = 100; 

    private Rigidbody _rigidbody;
    private TankInput _tankInput;

    [SerializeField] private AudioSource audioSource;
    private AudioClip _moveAudioClip;

    [SerializeField] private GameObject normalCamTarget;
    [SerializeField] private GameObject fireCamTarget;

    private GameObject _bomb;

    private Vector3 _moveRayBoxSize = new Vector3(5, 1);
    private float _moveRayDistance = 12f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _tankInput = GetComponent<TankInput>();
        audioSource = GetComponent<AudioSource>();
        
        _hpSlider = GameObject.FindWithTag("HpSlider").GetComponent<Slider>();
        _moveAudioClip = Resources.Load<AudioClip>("MoveAudioClip");
        _bomb = Resources.Load<GameObject>("Bomb");
    }

    private void OnEnable()
    {
        GameManager.Instance.SetRandomPosition(gameObject);
        
        _currentHp = MaxHp;
        if (_tankInput.photonView.IsMine)
        {
            _hpSlider.value = 1;
        }

        audioSource.clip = _moveAudioClip;
        audioSource.Play();
        
        if (_tankInput.photonView.IsMine)
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.normalTarget = normalCamTarget;
            cameraController.fireTarget = fireCamTarget;
            
            cameraController.Init();
        }
    }

    private void FixedUpdate()
    {
        MoveTank();
        RotateTank();
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    private void RotateTank()
    {
        float turn = _tankInput.Rotate * _rotateSpeed * Time.deltaTime;
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0, turn, 0));
    }

    private void MoveTank()
    {
        Vector3 moveDistance = transform.forward * (_tankInput.Move * _moveSpeed * Time.deltaTime);
        Vector3 rayPos = transform.position + new Vector3(0, 5, 0);
        
        if (moveDistance.z > 0
            && Physics.BoxCast(rayPos, _moveRayBoxSize,moveDistance,
                out RaycastHit forwardHit, Quaternion.identity, _moveRayDistance))
        {
            return;
        }
        if (moveDistance.z < 0
            && Physics.BoxCast(rayPos, _moveRayBoxSize, moveDistance,
                out RaycastHit backHit, Quaternion.identity, _moveRayDistance))
        {
            return;
        }

        _rigidbody.MovePosition(_rigidbody.position + moveDistance);
    }

    public void DamageAction(int damage)
    {
        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            _tankInput.photonView.RPC("Die", RpcTarget.Others);
            Die();
        }

        if (_tankInput.photonView.IsMine)
        {
            _hpSlider.value = _currentHp / MaxHp;
        }
    }
    
    [PunRPC]
    private void Die()
    {
        Instantiate(_bomb, transform.position, transform.rotation);
        gameObject.SetActive(false);
        GameManager.Instance.ReSpawn(gameObject, _tankInput.photonView.IsMine);
        if (_tankInput.photonView.IsMine)
        {
            _hpSlider.value = 0;
            ++GameManager.Instance.EnemyScore;
        }
        else
        {
            ++GameManager.Instance.MyScore;
        }
    }
}