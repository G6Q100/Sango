using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDisable : MonoBehaviour
{
    AudioSource audios;

    void Start()
    {
        audios = GetComponent<AudioSource>();
    }

    void Update()
    {
        Debug.Log(audios.time);
        if (audios.time >= audios.clip.length)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}
