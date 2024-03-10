using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource gunShot, gunHit, shortStoneMoving, stoneMoving, speed, spawn, shatter, meat,
        dash, addAbilityUse, heal, getHurt, beast, fingerSlap, groundHit;

    public AudioSource boss1BGM;
    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlaySound(AudioSource audio)
    {
        audio.Play();
    }
}
