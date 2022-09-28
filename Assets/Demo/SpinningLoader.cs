using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinningLoader : MonoBehaviour
{
    private Image _image;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _image.rectTransform.Rotate(Vector3.back, 1f);
    }

    private void OnEnable()
    {
        _image.rectTransform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
