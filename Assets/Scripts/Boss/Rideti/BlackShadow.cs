using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackShadow : MonoBehaviour
{
    void Update()
    {
        transform.localScale += Vector3.one * 100 * Time.deltaTime;
    }
}
