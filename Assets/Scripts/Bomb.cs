using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Sihanboo());
    }

    private IEnumerator Sihanboo()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
