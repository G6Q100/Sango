using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AttackParts
{
    [SerializeField]public GameObject atackParts;
    public HP hp;

    public float attackCD;
    public float currentCD;
}

public class ChimeraBeast : MonoBehaviour
{
    [SerializeField] GameObject triggerRange, aimPos;
    [SerializeField] GameObject[] focusPos;
    [SerializeField] Slider bossHP;

    [SerializeField] Transform leftHand, rightHand, spikeMouth, middleMouth;
    [SerializeField] GameObject wave, fireball, giantFireBall, groundCrack, lockDoor;

    [SerializeField] AttackParts[] normalMouths;
    [SerializeField] AttackParts spikeMouthPart;
    int attackCounter = 0, crackAttackUsed = 0;

    Animator anim;
    bool startFight, battleLoop;

    HP hp;
    Player player;

    int rand, phase = 1;
    float waveCD, currentWaveCD;
    bool battling;

    public bool destroyPorjectile;

    [SerializeField] GameObject nextLevel;

    public static ChimeraBeast instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        anim = GetComponent<Animator>();

        hp = GetComponent<HP>();
        hp.active = false;

        for (int i = 0; i < normalMouths.Length; i++)
        {
            normalMouths[i].attackCD = Random.Range(0f + (0.5f * i), 0.5f + (0.5f * i));
        }
        spikeMouthPart.attackCD = 1f;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        CamController.instance.threshold = 15;
    }

    void Update()
    {
        if (!triggerRange.activeInHierarchy && !startFight)
        {
            lockDoor.SetActive(true);
            startFight = true;
            anim.SetTrigger("StartFight");
            CamController.instance.target = player.gameObject;
            CamController.instance.slowZoom = true;
            player.lostControl = true;
            SoundManager.instance.boss1BGM.volume = 0;
            SoundManager.instance.boss1BGM.Play();
        }

        if (hp.hp <= 0)
        {
            SoundManager.instance.boss1BGM.volume -= Time.deltaTime / 2;
        }
        else if (SoundManager.instance.boss1BGM.volume < 1)
        {
            SoundManager.instance.boss1BGM.volume += Time.deltaTime / 2;
        }

        if (!battleLoop)
            return;

        CheckPhase();

        AttackLoop();
    }

    void StartBattle()
    {
        hp.active = true;
        CamController.instance.focus = null;
        CamController.instance.target = aimPos;
        CamController.instance.slowZoom = false;
        player.lostControl = false;
        battling = true;
        waveCD = 4;
        battleLoop = true;
        anim.SetTrigger("StartIdle");
    }

    void PhaseAttackEnd(int phase)
    {
        if (phase == 0)
        {
            battleLoop = true;
            spikeMouthPart.attackCD = 0.5f;
            spikeMouthPart.currentCD = 0;
            waveCD = 3.5f;
            currentWaveCD = 0;
        }
        if (phase == 1)
        {
            battleLoop = true;
            for (int i = 0; i < normalMouths.Length; i++)
            {
                normalMouths[i].attackCD = Random.Range(0f + (0.25f * i), 0.5f + (0.25f * i));
            }
            spikeMouthPart.attackCD = 0.3f;
            spikeMouthPart.currentCD = 0;
            waveCD = 3.3f;
            currentWaveCD = 0;
        }
        if (phase == 2)
        {
            battleLoop = true;
            for (int i = 0; i < normalMouths.Length; i++)
            {
                normalMouths[i].attackCD = Random.Range(0f + (0.25f * i), 0.5f + (0.25f * i));
            }
            groundCrack.SetActive(false);
            spikeMouthPart.attackCD = 0.3f;
            spikeMouthPart.currentCD = 0;
            waveCD = 3.3f;
            currentWaveCD = 0;
        }
        if (phase == 3)
        {
            crackAttackUsed = 2;
            CamController.instance.ResetCamShake();
            anim.SetTrigger("GroundFireAttack");
            anim.SetLayerWeight(anim.GetLayerIndex("Idle Layer"), 0);
            return;
        }
        anim.SetLayerWeight(anim.GetLayerIndex("Idle Layer"), 1);
    }

    void CheckPhase()
    {
        if (hp.hp <= hp.maxHp * 0.85f && phase == 1 && 
            spikeMouthPart.currentCD + 3.2f <= spikeMouthPart.attackCD)
        {
            phase = 2;
            battleLoop = false;
            for (int i = 0; i < normalMouths.Length; i++)
            {
                if (i == 1 || i == 2)
                {
                    continue;
                }
                normalMouths[i].currentCD = normalMouths[i].attackCD;
                normalMouths[i].currentCD = 0;
                normalMouths[i].attackCD = Random.Range(1.5f, 3.5f);
                anim.SetBool("M" + (i + 1) + "Attack", false);
                normalMouths[i].hp.hitted = true;
            }
            CamController.instance.ResetCamShake();
            anim.SetTrigger("Phase1Attack");
            return;
        }
        if (hp.hp <= hp.maxHp * 0.65f && crackAttackUsed == 0 &&
            spikeMouthPart.currentCD + 3.2f <= spikeMouthPart.attackCD)
        {
            crackAttackUsed = 1;
            battleLoop = false;
            for (int i = 0; i < normalMouths.Length; i++)
            {
                normalMouths[i].currentCD = normalMouths[i].attackCD;
                anim.SetBool("M" + (i + 1) + "Attack", false);
                normalMouths[i].currentCD = 0;
                normalMouths[i].attackCD = Random.Range(1.5f, 2.5f);
                anim.SetBool("M" + (i + 1) + "Attack", false);
                normalMouths[i].hp.hitted = true;
            }
            CamController.instance.ResetCamShake();
            anim.SetTrigger("GroundFireAttack");
            anim.SetLayerWeight(anim.GetLayerIndex("Idle Layer"), 0);
            return;
        }
        if (hp.hp <= hp.maxHp * 0.42f && phase == 2 &&
            spikeMouthPart.currentCD + 3.2f <= spikeMouthPart.attackCD)
        {
            phase = 3;
            battleLoop = false;
            for (int i = 0; i < normalMouths.Length; i++)
            {
                normalMouths[i].currentCD = normalMouths[i].attackCD;
                anim.SetBool("M" + (i + 1) + "Attack", false);
                normalMouths[i].currentCD = 0;
                normalMouths[i].attackCD = Random.Range(1.5f, 2.5f);
                anim.SetBool("M" + (i + 1) + "Attack", false);
                normalMouths[i].hp.hitted = true;
            }
            CamController.instance.ResetCamShake();
            anim.SetTrigger("Phase2Attack");
            return;
        }
    }


    void AttackLoop()
    {
        if(currentWaveCD > waveCD)
        {
            currentWaveCD = 0;
            Attacking();
        }

        if (spikeMouthPart.currentCD > spikeMouthPart.attackCD)
        {
            spikeMouthPart.currentCD = 0;
            if (attackCounter > 2)
            {
                spikeMouthPart.attackCD = 4f;
                anim.SetTrigger("MiddleMAttack");
                attackCounter = -1;
            }
            else
            {
                spikeMouthPart.attackCD = 1f;
                anim.SetTrigger("SpikeMAttack");
            }
            attackCounter++;
        }

        if (battling == true)
        {
            currentWaveCD += Time.deltaTime;
            spikeMouthPart.currentCD += Time.deltaTime;
        }

        // small mouth attack
        for (int i = 0; i < normalMouths.Length; i++)
        {
            if (phase == 1 && i == 0)
            {
                continue;
            }
            if (phase == 1 && i == 3)
            {
                continue;
            }
            if (phase <= 2 && i == 1)
            {
                continue;
            }
            if (phase <= 2 && i == 2)
            {
                continue;
            }
            MouthAttack(normalMouths[i], i);
        }
    }

    void MouthAttack(AttackParts parts, int num)
    {
        parts.currentCD += Time.deltaTime;
        if (parts.currentCD > parts.attackCD)
        {
            parts.currentCD = 0;
            parts.attackCD = Random.Range(6.5f, 7.5f);
            anim.SetBool("M" + (num + 1) + "Attack", true);
            parts.hp.hitted = false;
        }
        if (parts.hp.hitted && anim.GetBool("M" + (num + 1) + "Attack"))
        {
            parts.currentCD = 0;
            parts.attackCD = Random.Range(6.5f, 7.5f);
            anim.SetBool("M" + (num + 1) + "Attack", false);
            parts.hp.hitted = true;
        }
    }

    void Attacking()
    {
        rand = Random.Range(0, 3);

        switch (rand)
        {
            case 0:
                anim.SetTrigger("LeftClawAttack");
                break;
            case 1:
                anim.SetTrigger("RightClawAttack");
                break;
            case 2:
                if (phase == 1)
                {
                    rand = Random.Range(0, 2);
                    if (rand == 0)
                        anim.SetTrigger("LeftClawAttack");
                    else
                        anim.SetTrigger("RightClawAttack");
                    break;
                }
                anim.SetTrigger("BothClawAttack");
                break;
        }

        waveCD = 7f;
    }

    void SpawnCrack()
    {
        groundCrack.SetActive(true);
    }
    
    // Animation Event
    void SpawnFireBall(int num)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.spawn);
        Vector3 playerPos = player.transform.position;

        var rand = Random.Range(2f, 8f);
        var rand2 = Random.Range(2f, 8f);
        SpawningFireBall(fireball, spikeMouth.position, Vector3.up, playerPos, 0);
        SpawningFireBall(fireball, spikeMouth.position, Vector3.up + Vector3.right * 0.5f, playerPos
            + Vector3.up * rand + Vector3.left * rand2, 0);
        rand = Random.Range(2f, 8f);
        rand2 = Random.Range(2f, 8f);
        SpawningFireBall(fireball, spikeMouth.position, Vector3.up + Vector3.left * 0.5f, playerPos 
            + Vector3.up * rand + Vector3.right * rand2, 0);
        rand = Random.Range(2f, 8f);
        rand2 = Random.Range(2f, 8f);
        SpawningFireBall(fireball, spikeMouth.position, Vector3.up + Vector3.right * 0.25f, playerPos
            + Vector3.down * rand + Vector3.left * rand2, 0);
        rand = Random.Range(2f, 8f);
        rand2 = Random.Range(2f, 8f);
        SpawningFireBall(fireball, spikeMouth.position, Vector3.up + Vector3.left * 0.25f, playerPos
            + Vector3.down * rand + Vector3.right * rand2, 0);

        if (crackAttackUsed == 1)
        {
            rand = Random.Range(6f, 12f);
            rand2 = Random.Range(6f, 12f);
            SpawningFireBall(fireball, spikeMouth.position, Vector3.up, playerPos, 0);
            SpawningFireBall(fireball, spikeMouth.position, Vector3.up + Vector3.right * 0.5f, playerPos
                + Vector3.up * rand + Vector3.left * rand2, 0);
            rand = Random.Range(6f, 12f);
            rand2 = Random.Range(6f, 12f);
            SpawningFireBall(fireball, spikeMouth.position, Vector3.up + Vector3.left * 0.5f, playerPos
                + Vector3.up * rand + Vector3.right * rand2, 0);
        }
        if (crackAttackUsed == 2)
        {
            rand = Random.Range(6f, 12f);
            rand2 = Random.Range(6f, 12f);
            SpawningFireBall(fireball, spikeMouth.position, Vector3.up + Vector3.right * 0.25f, playerPos
                + Vector3.down * rand + Vector3.left * rand2, 0);
            rand = Random.Range(6f, 12f);
            rand2 = Random.Range(6f, 12f);
            SpawningFireBall(fireball, spikeMouth.position, Vector3.up + Vector3.left * 0.25f, playerPos
                + Vector3.down * rand + Vector3.right * rand2, 0);
        }
    }

    void GiantFireBall(int num)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.speed);
        Vector3 targetPos = player.transform.position;
        Vector3 spawnPos = middleMouth.position;
        switch (num)
        {
            case 0:
                SpawningFireBall(giantFireBall, spawnPos, targetPos - spawnPos, targetPos, 3, 8);
                return;
            case 1:
                spawnPos = normalMouths[0].atackParts.transform.position;
                targetPos = spawnPos + Vector3.down;
                break;
            case 2:
                spawnPos = normalMouths[1].atackParts.transform.position;
                targetPos = spawnPos + Vector3.down;
                break;
            case 3:
                spawnPos = normalMouths[2].atackParts.transform.position;
                targetPos = spawnPos + Vector3.down;
                break;
            case 4:
                spawnPos = normalMouths[3].atackParts.transform.position;
                targetPos = spawnPos + Vector3.down;
                break;
        }

        SpawningFireBall(giantFireBall, spawnPos, targetPos - spawnPos, targetPos, 1, 8);
    }

    void SpawningFireBall(GameObject fireball, Vector3 pos, Vector3 dir, Vector3 targetPos, 
        int mode, [Optional] float speed)
    {
        GameObject ofirebal = ObjectPoolManager.SpawnObject(fireball, pos,
            Quaternion.identity);
        ofirebal.GetComponent<FireBall>().Attack(dir);
        ofirebal.GetComponent<FireBall>().targerPos = targetPos;
        ofirebal.GetComponent<FireBall>().mode = mode;
        if (speed != 0)
        {
            ofirebal.GetComponent<FireBall>().speed = speed;
        }
    }

    void SpawnWave(int waveMode)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.groundHit);
        switch (waveMode)
        {
            case 0:
                ObjectPoolManager.SpawnObject(wave, leftHand.position + Vector3.forward, Quaternion.identity);
                break;
            case 1:
                ObjectPoolManager.SpawnObject(wave, rightHand.position + Vector3.forward, Quaternion.identity);
                break;
            case 2:
                ObjectPoolManager.SpawnObject(wave, leftHand.position + Vector3.forward, Quaternion.identity);
                ObjectPoolManager.SpawnObject(wave, rightHand.position + Vector3.forward, Quaternion.identity);
                break;
        }
    }

    void ShakingCam(float time)
    {
        if (time == 0)
        {
            CamController.instance.ResetCamShake();
            return;
        }

        CamController.instance.ShakeCam(time, 15);
    }
    void StopShakingCam()
    {
        CamController.instance.ResetCamShake();
    }

    void Shout()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.beast);
    }

    void ShowHP()
    {
        bossHP.GetComponent<Animator>().SetBool("FadeIn", true);
        bossHP.maxValue = hp.maxHp;
        bossHP.value = bossHP.maxValue;
    }

    void Focus(int mode)
    {
        if (mode < 2)
        {
            Focusing(focusPos[mode], 15);
        }
        else if (mode < 3)
            Focusing(focusPos[mode], 13);
        else
            Focusing(focusPos[2], 20);
    }

    void StartEndFight()
    {
        destroyPorjectile = true;
        hp.active = false;
        Focusing(focusPos[2], 20); 
        player.lostControl = true;
        battleLoop = false;
        for (int i = 0; i < normalMouths.Length; i++)
        {
            normalMouths[i].attackCD = Random.Range(0f + (0.5f * i), 0.5f + (0.5f * i));
        }
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }
        groundCrack.SetActive(false);
        spikeMouthPart.attackCD = 0.3f;
        spikeMouthPart.currentCD = 0;
        waveCD = 3.3f;
        currentWaveCD = 0;
    }

    void EndFightStop()
    {
        CamController.instance.focus = null;
        player.lostControl = false;
        nextLevel.SetActive(true);
        enabled = false;
    }

    void Focusing(GameObject focusObj,int distant)
    {
        CamController.instance.focusDistant = distant;
        CamController.instance.focus = focusObj;
    }

    void GroundHit()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.groundHit);
    }
}
