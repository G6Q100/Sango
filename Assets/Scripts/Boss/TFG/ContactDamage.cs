using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    private void Update()
    {
        if (TFG.instance == null)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (!TFG.instance.gameObject.activeInHierarchy)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (TFG.instance.destroyPorjectile)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }
    }

    void DisableObject()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<HP>().GetHit(1);
        }
    }
}
