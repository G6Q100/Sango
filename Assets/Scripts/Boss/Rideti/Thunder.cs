using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    [SerializeField] int mode;
    public float speed;

    void Update()
    {
        if (mode == 0)
        {
            transform.Rotate(0, 0, speed * Time.deltaTime);
            return;
        }
    }

    void ShakeCam(float time)
    {
        CamController.instance.ShakeCam(time, 16);
        SoundManager.instance.PlaySound(SoundManager.instance.groundHit);
    }

    void StopAttack()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
