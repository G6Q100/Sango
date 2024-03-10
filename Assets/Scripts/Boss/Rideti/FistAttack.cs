using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistAttack : MonoBehaviour
{
    bool activate;
    [SerializeField]bool fixedPos, notActivate;

    private void OnEnable()
    {
        activate = false;
    }

    private void Update()
    {
        if (fixedPos)
        {
            transform.localPosition = Vector3.zero;
            return;
        }
        if (Rideti.instance == null)
        {
            activate = false;
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (!Rideti.instance.gameObject.activeInHierarchy)
        {
            activate = false;
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (Rideti.instance.destroyPorjectile)
        {
            activate = false;
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }
    }

    void FistAttacking()
    {
        CamController.instance.ShakeCam(0.3f, 20);
    }

    void FistSound()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.groundHit);
    }

    void EndAttack()
    {
        activate = false;
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    void ShakeCam(float time)
    {
        CamController.instance.ShakeCam(time, 15);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (activate)
            return;

        if (collision.tag == "Player")
        {
            if (!notActivate)
                activate = true;
            collision.GetComponent<HP>().GetHit(1);
        }
    }
}
