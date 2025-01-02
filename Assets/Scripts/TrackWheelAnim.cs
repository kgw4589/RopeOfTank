using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class TrackWheelAnim : MonoBehaviour
{
    private MeshRenderer _renderer;
    
    private TankInput _tankInput;

    private float _scrollSpeed = 2.0f;
    private float _offset;

    private void Start()
    {
        _tankInput = GetComponent<TankInput>();

        _renderer = transform.Find("Track").GetComponent<MeshRenderer>();
    }
    
    private void Update()
    {
        if (!_tankInput.photonView.IsMine)
        {
            return;
        }
        
        _offset += (_scrollSpeed * _tankInput.Move * Time.deltaTime);

        _renderer.material.SetTextureOffset("_MainTex", new Vector2(0, _offset));
        _renderer.material.SetTextureOffset("_BumpMap", new Vector2(0, _offset));
    }
}
