using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TankFire : MonoBehaviour
{
    private Transform _cannonTransform;
    private Transform _turretTransform;
    
    private Transform _firePosition;
    private GameObject _bulletFactory;

    [SerializeField] private AudioSource audioSource;
    private AudioClip _fireAudioClip;

    private Slider _powerSlider;
    private const float MaxPower = 25000f;
    private float _currentPower = 0f;
    private const float PowerUpSpeed = 7500f;

    private const int BulletDamage = 25;
    
    private TankInput _tankInput;
    
    private float _rotSpeed = 1500.0f;
    private float _minRotX = -25.0f;
    private float _maxRotX = 20.0f;
    
    private float _cannonAngleX;
    private float _turretAngleY;

    private GameObject _aim;

    private const float CoolTime = 2f;
    private bool _isReady = true;

    private void OnEnable()
    {
        if (_aim && _powerSlider)
        {
            _aim.SetActive(true);
            _powerSlider.value = 0f;
        }

        _isReady = true;
    }

    private void Start()
    {
        _tankInput = GetComponent<TankInput>();

        _turretTransform = transform.Find("Turret");
        _cannonTransform = _turretTransform.Find("Cannon");
        _firePosition = _cannonTransform.Find("FirePosition");

        _bulletFactory = Resources.Load<GameObject>("Bullet");
        _fireAudioClip = Resources.Load<AudioClip>("FireAudioClip");

        _aim = GameObject.FindWithTag("Aim");

        _powerSlider = GameObject.FindWithTag("PowerSlider").GetComponent<Slider>();

        _isReady = true;
    }
    
    private void Update()
    {
        if (!_tankInput.photonView.IsMine)
        {
            return;
        }
        
        CannonRotate();
        Fire();
    }

    private void OnDisable()
    {
        _aim.gameObject.SetActive(false);
    }

    private void CannonRotate()
    {
        float rotY = _tankInput.MouseX;
        float rotX = _tankInput.MouseY;
        
        _cannonAngleX += -rotX * _rotSpeed * Time.deltaTime;
        _turretAngleY += rotY * _rotSpeed * Time.deltaTime;
        
        _cannonAngleX = Mathf.Clamp(_cannonAngleX, _minRotX, _maxRotX);

        Quaternion cannonAngle = _cannonTransform.localRotation;
        _cannonTransform.localRotation = Quaternion.Euler(_cannonAngleX, cannonAngle.y, cannonAngle.z);

        Quaternion turretAngle = _turretTransform.localRotation;
        _turretTransform.localRotation = Quaternion.Euler(turretAngle.x, _turretAngleY, turretAngle.z);
    }

    private void Fire()
    {
        if (_tankInput.OnRightMouseDown)
        {
            _currentPower = 0;
        }
        else if (_tankInput.OnRightMouse)
        {
            if (_tankInput.OnLeftMouseDown && _isReady)
            {
                _tankInput.photonView.RPC("Shoot", RpcTarget.Others, _currentPower);
                Shoot(_currentPower);
            }
            else if (_currentPower < MaxPower)
            {
                _currentPower += PowerUpSpeed * Time.deltaTime;
                _powerSlider.value = (_currentPower / MaxPower);
            }
        }
        else if (_tankInput.OnRightMouseUp)
        {
            _currentPower = 0;
            _powerSlider.value = 0;
        }
    }

    private IEnumerator StartCoolTime()
    {
        yield return new WaitForSeconds(CoolTime);
        _isReady = true;
    }

    [PunRPC]
    private void Shoot(float currentPower)
    {
        audioSource.clip = _fireAudioClip;
        audioSource.Play();
                
        GameObject bullet = Instantiate(_bulletFactory, _firePosition.position, _firePosition.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        
        bulletScript.Shoot(currentPower, BulletDamage);
            
        _currentPower = 0;
        _powerSlider.value = 0;

        _isReady = false;
        StartCoroutine(StartCoolTime());
    }
}




