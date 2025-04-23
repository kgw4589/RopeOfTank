using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public GameObject bullet;

    public LayerMask boomLayer;

    private int _damage = 0;
    private const float BombRadius = 10f;
    
    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(bullet, transform.position, transform.rotation);
        Collider[] colliders = new Collider[2];
        Physics.OverlapSphereNonAlloc(transform.position, BombRadius, colliders, boomLayer);
        for (int i = 0; i < 2; i++)
        {
            if (colliders[i] is null)
            {
                break;
            }
            
            colliders[i].GetComponent<IDamagable>().DamageAction(_damage);
            
            Rigidbody hitObjectRigid = colliders[i].GetComponent<Rigidbody>();
            if (hitObjectRigid is not null)
            {
                hitObjectRigid.velocity = Vector3.zero;
                hitObjectRigid.AddForce(Vector3.up * 50000f, ForceMode.Impulse);
            }
        }
        Destroy(this.gameObject);
    }

    public void Shoot(float power, int damage)
    {
        _damage = damage;
        _rigidbody.AddForce(transform.forward * power);
    }
}
