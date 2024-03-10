using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryAfterAnim : MonoBehaviour
{
    void ReturnPool()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
