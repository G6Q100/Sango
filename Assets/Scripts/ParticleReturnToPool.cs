using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleReturnToPool : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
