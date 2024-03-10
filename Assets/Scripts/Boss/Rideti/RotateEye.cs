using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class RotateEye : MonoBehaviour
{
    [SerializeField] float speed = 1;

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime * 180);
    }
}
