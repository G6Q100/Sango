using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    Vector3 movePoint, refVector3;
    [SerializeField] Vector3 offset;
    [SerializeField] GameObject target;

    [SerializeField] int mode;

    private void Update()
    {
        if (mode == 1)
        {
            transform.Rotate(0, 0, Time.deltaTime * 75);
        }
    }

    void FixedUpdate()
    {
        movePoint = target.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, movePoint, ref refVector3, Mathf.Sqrt(0.01f));
    }
}
