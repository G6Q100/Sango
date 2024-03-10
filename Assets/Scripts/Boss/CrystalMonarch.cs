using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CrystalMonarch : MonoBehaviour
{
    [SerializeField] Slider bossHP;
    HP hp;

    [SerializeField] GameObject startTrigger;
    [SerializeField] bool startFight;

    [SerializeField] GameObject lockDoor, head, crystal, crystalSpawner;

    Animator anim;
    float cutsceneCD;
    string mode = "Default";

    int rand;

    int phase = 1;

    private Player player;

    public bool destroyPorjectile, modeChanged;
    [SerializeField] Transform[] standPos;
    [SerializeField] GameObject fastCrystal, normalCrystal;
    int standNum = -1;

    bool alive;

    [SerializeField] GameObject nextLevel;

    Vector3 refVector3;
    AudioSource bgm;

    public static CrystalMonarch instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    void Start()
    {
        bgm = SoundManager.instance.boss1BGM;
        anim = GetComponent<Animator>();
        cutsceneCD = 3;

        hp = GetComponent<HP>();
        hp.active = false;
        alive = true;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        CamController.instance.threshold = 6;
    }
    void StoneStartMoving()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.shortStoneMoving);
    }

    void StoneMoving()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.stoneMoving);
    }
    void TeethShowing()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.meat);
    }

    public void StartBattle()
    {
        hp.active = true;
        bossHP.maxValue = hp.maxHp;
        bossHP.value = bossHP.maxValue;
        bossHP.GetComponent<Animator>().SetBool("FadeIn", true);
        CamController.instance.slowZoom = false;

        ChangeAttackMode();
    }

    private void Update()
    {
        if (!startTrigger.activeInHierarchy && !startFight)
        {
            startFight = true;

            lockDoor.SetActive(true);
            CamController.instance.target = gameObject;
            CamController.instance.slowZoom = true;
            bgm.volume = 0;
            bgm.Play();
        }

        if (destroyPorjectile)
        {
            bgm.volume -= Time.deltaTime / 2;
        }
        else if (bgm.volume < 1)
        {
            bgm.volume += Time.deltaTime / 2;
        }

        if (bossHP.gameObject.activeInHierarchy)
        {
            bossHP.value = hp.hp;
        }

        if (!startFight)
            return;

        if (phase == 1 && hp.hp <= hp.maxHp * 0.65f)
        {
            Phase2();
        }
        if (!alive)
            return;

        AttackLoop();
    }



    void Phase2()
    {
        phase = 2;
        StopAllCoroutines();
        CamController.instance.ResetCamShake();
        destroyPorjectile = true;

        hp.active = false;
        player.GetComponent<HP>().active = false;

        bossHP.GetComponent<Animator>().SetBool("FadeIn", false);
        CamController.instance.focus = head;
        player.lostControl = true;
        CamController.instance.focusDistant = 8;
        anim.StopPlayback();
        anim.SetTrigger("Transform");
        mode = "Idle";
    }

    public void Focusing(int distant)
    {
        CamController.instance.ResetCamShake();
        CamController.instance.focusDistant = distant;
        CamController.instance.focus = gameObject;
    }

    void CrystalAttack(int mode)
    {
        float speed = 200;
        float dampingSpeed = 1100;
        switch (mode)
        {
            case 1:
                for (int i = -15; i <= 15; i += 5)
                {
                    CrystalShooting(player.transform.position + Vector3.left * 10 + Vector3.up * i, speed, dampingSpeed, Vector3.right, 90, 1);
                }
                break;
            case 2:
                for (int i = -15; i <= 15; i += 5)
                {
                    CrystalShooting(player.transform.position + Vector3.right * 10 + Vector3.up * i, speed, dampingSpeed, Vector3.left, 270, 1);
                }
                break;
            case 3:
                for (int i = -20; i <= 20; i += 5)
                {
                    CrystalShooting(transform.position + Vector3.right * 5 + Vector3.up * i, speed, dampingSpeed, Vector3.right, 90, 1);
                }
                for (int i = -20; i <= 20; i += 5)
                {
                    CrystalShooting(transform.position + Vector3.left * 5 + Vector3.up * i, speed, dampingSpeed, Vector3.left, 270, 1);
                }
                break;
            case 4:
                switch (standNum)
                {
                    case 0:
                        if (phase == 2)
                            CrystalSpawn(crystalSpawner, transform.position, 200, 500, 0, 15, 9, 0.05f, normalCrystal, 1);
                        else if (phase == 3)
                            CrystalSpawn(crystalSpawner, transform.position, 200, 500, 0, 12, 11, 0.05f, normalCrystal, 1);
                        break;
                    case 1:
                        if (phase == 2)
                            CrystalSpawn(crystalSpawner, transform.position, 200, 500, 70, 15, 9, 0.05f, normalCrystal, 1);
                        else if (phase == 3)
                            CrystalSpawn(crystalSpawner, transform.position, 200, 500, 70, 12, 11, 0.05f, normalCrystal, 1);
                        break;
                    case 2:
                        if (phase == 2)
                            CrystalSpawn(crystalSpawner, transform.position, 200, 500, -70, -15, 9, 0.05f, normalCrystal, 1);
                        else if (phase == 3)
                            CrystalSpawn(crystalSpawner, transform.position, 200, 500, -70, -12, 11, 0.05f, normalCrystal, 1);
                        break;
                    case 3:
                        if (phase == 2)
                            CrystalSpawn(crystalSpawner, transform.position, 200, 500, 0, -15, 9, 0.05f, normalCrystal, 1);
                        else if (phase == 3)
                            CrystalSpawn(crystalSpawner, transform.position, 200, 500, 0, -12, 11, 0.05f, normalCrystal, 1);
                        break;
                }
                break;
            case 5:
                for (int i = -40; i <= 40; i += 5)
                {
                    CrystalSpawn(crystalSpawner, transform.position + Vector3.down * 43 + Vector3.right * i, 200, 0, 0, 0, 3, 1.8f, normalCrystal, 2);
                }
                for (int i = -40; i <= 35; i += 5)
                {
                    CrystalSpawn(crystalSpawner, transform.position + Vector3.down * 43 + Vector3.right * (i + 2.5f), 200, 0, 0, 0, 2, 1.8f, normalCrystal, 3);
                }
                break;
            case 6:
                FastCrystalSpawn(crystalSpawner, transform.position + Vector3.up * 10 + Vector3.left * 20, 13.5f, 0, 0, 13, 130, 0.03f, 0, fastCrystal, 0);
                FastCrystalSpawn(crystalSpawner, transform.position + Vector3.up * 10 + Vector3.right * 21, 13.5f, 0, 0, 13, 130, 0.03f, 0, fastCrystal, 0);
                break;
        }
    }
    void FastCrystalSpawn(GameObject crystalType, Vector3 spawnPos, float speed, float dampingSpeed,
        float rotZ, int rotateSpeed, int maxRotate, float attackCD, int mode, GameObject bullet, float range)
    {
        CrystalFixedSpawner oCrystal = ObjectPoolManager.SpawnObject(crystalType, spawnPos, Quaternion.identity).GetComponent<CrystalFixedSpawner>();
        oCrystal.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        oCrystal.speed = speed;
        oCrystal.dampingSpeed = dampingSpeed;
        oCrystal.rotateSpeed = rotateSpeed;
        oCrystal.maxRotateTime = maxRotate;
        oCrystal.attackCD = attackCD;
        oCrystal.mode = mode;
        oCrystal.crystal = bullet;
        oCrystal.range = range;
    }

    void CrystalSpawn(GameObject crystalType,Vector3 spawnPos, float speed, float dampingSpeed, 
        float rotZ, int rotateSpeed, int maxRotate, float attackCD, GameObject bullet, int mode)
    {
        CrystalFixedSpawner oCrystal = ObjectPoolManager.SpawnObject(crystalType, spawnPos, Quaternion.identity).GetComponent<CrystalFixedSpawner>();
        oCrystal.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        oCrystal.speed = speed;
        oCrystal.dampingSpeed = dampingSpeed;
        oCrystal.rotateSpeed = rotateSpeed;
        oCrystal.maxRotateTime = maxRotate;
        if (mode == 2)
            oCrystal.currentCD = attackCD;
        else if (mode == 3)
            oCrystal.currentCD += attackCD / 2;
        oCrystal.attackCD = attackCD;
        oCrystal.crystal = bullet;
        oCrystal.mode = mode;
    }

    void CrystalShooting(Vector3 spawnPos, float speed, float dampingSpeed, Vector3 dir, float rotZ, int mode)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.spawn);
        GameObject oCrystal = ObjectPoolManager.SpawnObject(crystal, spawnPos, Quaternion.Euler(0, 0, rotZ));
        oCrystal.GetComponent<Crystal>().speed = speed;
        oCrystal.GetComponent<Crystal>().dampingSpeed = dampingSpeed;
        oCrystal.GetComponent<Crystal>().lookPos = dir;
        oCrystal.GetComponent<Crystal>().mode = mode;
    }

    void ChangePhase()
    {
        hp.active = true;
        bossHP.GetComponent<Animator>().SetBool("FadeIn", true);
        CamController.instance.focus = null;
        player.lostControl = false;
        player.GetComponent<HP>().active = true;
        destroyPorjectile = false;
        modeChanged = true;
        ChangeAttackMode();
    }

    void ChangeAttackMode()
    {
        if (!modeChanged && phase == 2)
            return;

        if (CamController.instance.focus != null)
            CamController.instance.focus = null;

        if (hp.hp <= hp.maxHp * 0.45f && phase == 2)
        {
            phase = 3;
            standNum = 5;
        }

        rand++;

        if (rand > 3)
        {
            rand = 1;
        }
        if (phase == 2)
            rand = 4;
        else if (phase == 3)
            rand = 5;

        switch (rand)
        {
            case 1:
                mode = "Attack";
                anim.SetTrigger("RightAttack");
                break;
            case 2:
                mode = "Attack";
                anim.SetTrigger("LeftAttack");
                break;
            case 3:
                mode = "Attack";
                anim.SetTrigger("BothAttack");
                break;
            case 4:
                standNum++;
                if (standNum > 3)
                    standNum = 0;
                mode = "Phase2Attack";
                if (standNum <= 1)
                    anim.SetTrigger("BottomRightAttack");
                else
                    anim.SetTrigger("BottomLeftAttack");
                break;
            case 5:
                standNum--;
                if (standNum < 0)
                    standNum = 4;
                mode = "Phase2Attack";
                if (standNum <= 1)
                    anim.SetTrigger("BottomRightAttack");
                else if(standNum <= 3)
                    anim.SetTrigger("BottomLeftAttack");
                else
                    anim.SetTrigger("MiddleAttack");
                break;
        }
    }

    void AttackLoop()
    {
        cutsceneCD -= Time.deltaTime;
        switch (mode)
        {
            case "Default":
                if (cutsceneCD <= 0)
                {
                    anim.SetTrigger("StartFight");
                    mode = "Idle";
                }
                break;
            case "Idle":
                break;
            case "Attack":
                break;
            case "Phase2Attack":
                transform.position = Vector3.SmoothDamp(transform.position, standPos[standNum].position, ref refVector3, Mathf.Sqrt(0.1f));
                break;
        }
    }

    void EndFightStart()
    {
        Focusing(9);
        destroyPorjectile = true;
        alive = false;
        CamController.instance.target = head;
        player.lostControl = true;

    }
    void PlayMeatSound()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.meat);
    }

    void PlayStoneSound()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.stoneMoving);
    }

    void EndFightStop()
    {
        CamController.instance.focus = null;
        player.lostControl = false;
        nextLevel.SetActive(true);
        enabled = false;
    }
}
