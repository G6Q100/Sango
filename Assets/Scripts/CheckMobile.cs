using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMobile : MonoBehaviour
{
    [SerializeField] bool isMobile;
    void Start()
    {
        if (isMobile && Application.isMobilePlatform)
            return;
        if (!isMobile && !Application.isMobilePlatform)
            return;
        gameObject.SetActive(false);
    }
}
