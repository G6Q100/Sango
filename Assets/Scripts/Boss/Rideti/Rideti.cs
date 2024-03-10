using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Rideti : MonoBehaviour
{
    [SerializeField] GameObject triggerRange, startPos;
    [SerializeField] Slider bossHP;

    Animator anim;
    bool startFight, battleLoop, isFistAttack, isFireAttack, isShortFireAttack, isThunderAttack, isShortThunderAttack, startHeal;

    HP hp;
    Player player;

    Vector3 targetPos;
    [SerializeField] Transform followPos, leftHand, rightHand, magicSpawnPos;
    Vector3 refVector3;

    [SerializeField] GameObject warningCircle, fistAttackPrefab, waveAttack, greenOrb, 
        redOrb, redExplosion, blueExplosion, fireAttackPrefab, thunderAttackPrebfab, lockDoor, block, cutscene;

    [SerializeField] Thunder thunderRot;

    string mode = "Follow";
    float modeCD, attackCD, thunderCD, healCD;
    int phase = 1, healTime;

    [SerializeField] LoadLevel loadLevel;

    public bool destroyPorjectile;

    public static Rideti instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        modeCD = 4f;

        hp = GetComponent<HP>();
        hp.active = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (!triggerRange.activeInHierarchy && !startFight)
        {
            lockDoor.SetActive(true);
            startFight = true;
            CamController.instance.focusDistant = 12;
            CamController.instance.focus = startPos.gameObject;
            anim.SetTrigger("StartFight");
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

        CheckPhase();
        Healing();

        if (!battleLoop)
            return;

        AttackLoop();
        FistAttackLoop();
        FireAttackLoop();
        ThunderAttackLoop();
    }
    void AttackLoop()
    {
        if (modeCD > 0)
        {
            modeCD -= Time.deltaTime;
        }

        switch (mode)
        {
            case "Follow":
                transform.position = Vector3.SmoothDamp(transform.position, followPos.position, ref refVector3, Mathf.Sqrt(0.2f));
                if (modeCD > 0)
                    break;
                targetPos = player.transform.position + Vector3.up * 15f + Vector3.right * 3f;
                anim.SetBool("StartIdle", false);
                anim.SetTrigger("HighFive");
                mode = "HighFiveAttack";
                break;
            case "HighFiveAttack":
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref refVector3, Mathf.Sqrt(0.05f));
                break;
            case "FistAttack":
                transform.position = Vector3.SmoothDamp(transform.position, player.transform.position, ref refVector3, Mathf.Sqrt(0.05f));
                
                if (modeCD > 0)
                    break;
                attackCD = 0;
                isFistAttack = false;
                BackToIdle();
                break;
            case "GroundPunch":
                transform.position = Vector3.SmoothDamp(transform.position, followPos.position, ref refVector3, Mathf.Sqrt(0.2f));
                attackCD += Time.deltaTime;
                if (attackCD >= 2.5f)
                {
                    attackCD = -999;
                    anim.SetTrigger("GroundPunch");
                    anim.SetBool("StartIdle", false);
                }
                if (modeCD > 0)
                    break;
                targetPos = player.transform.position + Vector3.up * 15f + Vector3.right * 3f;
                anim.SetTrigger("HighFive");
                mode = "HighFiveAttack";
                break;
            case "GreenOrbAttack":
                transform.position = Vector3.SmoothDamp(transform.position, followPos.position, ref refVector3, Mathf.Sqrt(0.2f));
                attackCD += Time.deltaTime;
                isFistAttack = false;
                if (attackCD >= 2.5f)
                {
                    attackCD = -999;
                    anim.SetTrigger("GreenMagic");
                    anim.SetBool("StartIdle", false);
                }
                break;
            case "GreenMagicAttack":
                break;
            case "RedOrbAttack":
                transform.position = Vector3.SmoothDamp(transform.position, followPos.position, ref refVector3, Mathf.Sqrt(0.2f));
                attackCD += Time.deltaTime;
                if (attackCD >= 2.5f)
                {
                    attackCD = -999;
                    anim.SetTrigger("RedMagic");
                    anim.SetBool("StartIdle", false);
                }
                break;
            case "RedMagicAttack":
                if (modeCD > 0)
                    break;
                isFireAttack = false;
                PhaseChange(1);
                break;
            case "BlueOrbAttack":
                transform.position = Vector3.SmoothDamp(transform.position, followPos.position, ref refVector3, Mathf.Sqrt(1));
                attackCD += Time.deltaTime;
                isFireAttack = false;
                if (attackCD >= 2.5f)
                {
                    attackCD = -999;
                    anim.SetTrigger("BlueMagic");
                    anim.SetBool("StartIdle", false);
                }
                break;
            case "GreenFistCombo":
                isFireAttack = false;
                isThunderAttack = false;
                if (modeCD > 0)
                    break;
                mode = "FireThunderCombo";
                isFistAttack = false;
                attackCD = 0.3f;
                thunderCD = 0.15f;
                modeCD = 5;
                break;
            case "FireThunderCombo":
                isFireAttack = true;
                isThunderAttack = true;
                if (modeCD > 0)
                    break;
                isFireAttack = false;
                isThunderAttack = false;
                PhaseChange(3);
                break;
            case "AllMagicAttack":
                transform.position = Vector3.SmoothDamp(transform.position, followPos.position, ref refVector3, Mathf.Sqrt(1));
                isFireAttack = false;
                isShortThunderAttack = true;
                isShortFireAttack = true;
                attackCD += Time.deltaTime;
                if (attackCD >= 2.5f)
                {
                    attackCD = -999;
                    anim.SetBool("AllMagic", true);
                    anim.SetBool("StartIdle", false);
                }
                break;
        }
    }
    void CheckPhase()
    {
        if (phase == 1 && hp.hp <= hp.maxHp * 0.5f)
        {
            phase = 2;
            battleLoop = false;
            anim.SetBool("StartIdle", false);
            anim.SetTrigger("Phase2");
            mode = "GreenMagicAttack";
            CamController.instance.focusDistant = 16;
            CamController.instance.focus = gameObject;
            destroyPorjectile = true;
            player.lostControl = true;
            isFistAttack = false;
            attackCD = 0;
            modeCD = 4;
            return;
        }
        if (phase == 2 && healTime == 1 && hp.hp <= hp.maxHp * 0.45f)
        {
            phase = 3;
            battleLoop = false;
            anim.SetBool("StartIdle", false);
            anim.SetTrigger("Phase3");
            mode = "GreenMagicAttack";
            CamController.instance.focusDistant = 16;
            CamController.instance.focus = gameObject;
            destroyPorjectile = true;
            player.lostControl = true;
            isFistAttack = false;
            attackCD = 0;
            modeCD = 4;
            return;
        }
        if (phase == 3 && healTime == 2 && hp.hp <= hp.maxHp * 0.6f)
        {
            phase = 4;
            battleLoop = false;
            anim.SetBool("StartIdle", false);
            anim.SetTrigger("Phase4");
            mode = "GreenMagicAttack";
            CamController.instance.focusDistant = 16;
            CamController.instance.focus = gameObject;
            destroyPorjectile = true;
            player.lostControl = true;
            isFistAttack = false;
            isFireAttack = false;
            attackCD = 0;
            modeCD = 4;
            return;
        }
        if (phase == 4 && healTime == 3 && hp.hp <= hp.maxHp * 0.5f)
        {
            phase = 5;
            battleLoop = false;
            anim.SetBool("StartIdle", false);
            anim.SetTrigger("Phase5");
            mode = "GreenMagicAttack";
            CamController.instance.focusDistant = 16;
            CamController.instance.focus = gameObject;
            destroyPorjectile = true;
            player.lostControl = true;
            isFistAttack = false;
            isFireAttack = false;
            isThunderAttack = false;
            attackCD = 0;
            modeCD = 4;
            return;
        }
    }

    void Healing()
    {
        if (startHeal && hp.hp < hp.maxHp)
        {
            healCD += Time.deltaTime * hp.maxHp / 3;
            if (healCD > 1)
            {
                healCD = 0;
                hp.GetHit(-1);
            }
            return;
        }

        if (startHeal)
        {
            healTime++;
            hp.hp = hp.maxHp;
            startHeal = false;
        }
    }

    void PhaseChange(int phase)
    {
        CamController.instance.focus = null;
        modeCD = 999f;
        attackCD = 0;
        anim.SetBool("StartIdle", true);
        if (phase == 0)
            mode = "GreenOrbAttack";
        else if (phase == 1)
            mode = "RedOrbAttack";
        else if (phase == 2)
            mode = "BlueOrbAttack";
        else if (phase == 3)
            mode = "AllMagicAttack";

        anim.ResetTrigger("FingerSlap");
        destroyPorjectile = false;
        player.lostControl = false;
        battleLoop = true;
    }

    void FistAttackLoop()
    {
        if (!isFistAttack)
            return;

        attackCD += Time.deltaTime;
        if (attackCD >= 0.9f)
        {
            attackCD = 0;
            ObjectPoolManager.SpawnObject(fistAttackPrefab, player.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 361)));
        }
    }

    void FireAttackLoop()
    {
        if (!isFireAttack && !isShortFireAttack)
            return;

        attackCD += Time.deltaTime;
        if ((attackCD >= 0.65f && isFireAttack) || (attackCD >= 5f && isShortFireAttack))
        {
            attackCD = 0;
            Vector3 randOffset = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
            ObjectPoolManager.SpawnObject(fireAttackPrefab, 
                player.transform.position + randOffset, Quaternion.Euler(0, 0, Random.Range(0, 361)));
            SoundManager.instance.PlaySound(SoundManager.instance.spawn);
        }
    }
    void ThunderAttackLoop()
    {
        if (!isThunderAttack && !isShortThunderAttack)
            return;

        thunderCD += Time.deltaTime;
        if ((thunderCD >= 0.25f && isThunderAttack) || (thunderCD >= 0.65f && isShortThunderAttack))
        {
            thunderCD = 0;
            Vector3 randOffset = new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0);
            ObjectPoolManager.SpawnObject(thunderAttackPrebfab, player.transform.position + randOffset, Quaternion.identity);
        }
    }

    // Animation Event
    void FocusingDistant(int distant)
    {
        CamController.instance.focusDistant = distant;
    }

    void StartBattle()
    {
        hp.active = true;
        CamController.instance.focus = null;
        CamController.instance.target = gameObject;
        bossHP.GetComponent<Animator>().SetBool("FadeIn", true);
        anim.SetBool("StartIdle", true);
        bossHP.maxValue = hp.maxHp;
        bossHP.value = bossHP.maxValue;
        player.lostControl = false;
        battleLoop = true;
    }

    void SpawnCircleWarning()
    {
        GameObject ocircle = ObjectPoolManager.SpawnObject(warningCircle, player.transform.position, Quaternion.identity);
        ocircle.GetComponent<Animator>().speed = 1.25f;
    }

    void HighFiveAttackRange()
    {
        CamController.instance.ShakeCam(0.3f, 20);
        SoundManager.instance.PlaySound(SoundManager.instance.groundHit);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + Vector3.down * 16.5f + Vector3.left * 2.5f, 5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<HP>().GetHit(1);
            }
        }
    }

    void StartFistAttack()
    {
        isFistAttack = true;
        attackCD = 0.8f;
    }

    void FistAttack()
    {
        mode = "FistAttack";
        modeCD = 3f;
    }

    void GroundPunchAttack(int mode)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.groundHit);
        if (mode == 0)
        {
            ObjectPoolManager.SpawnObject(waveAttack, leftHand.transform.position, Quaternion.identity);
        }
        else if(mode == 1)
        {
            ObjectPoolManager.SpawnObject(waveAttack, rightHand.transform.position, Quaternion.identity);
        }
    }

    void EndGroundPunch()
    {
        modeCD = 0;
    }

    void SpawnOrb(int mode)
    {
        if (mode == 0)
        {
            GameObject oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, magicSpawnPos.transform.position, Quaternion.identity);
            oGreenOrb.transform.up = player.transform.position - oGreenOrb.transform.position;
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(90, 0.03f, 64, 18, 0);
            return;
        }
        if (mode == 1)
        {
            GameObject oRedOrb = ObjectPoolManager.SpawnObject(redOrb, magicSpawnPos.transform.position, Quaternion.identity);
            oRedOrb.transform.up = player.transform.position - oRedOrb.transform.position;
            oRedOrb.GetComponent<SpawnOrb>().SetStats(45, 0.15f, 12, 16, 1);
            return;
        }
    }

    void GreenMagicAttack()
    {
        for (int i = -75; i <= 75; i += 25)
        {
            GameObject oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, new Vector3(i, 80, 0), Quaternion.identity);
            oGreenOrb.transform.up = -oGreenOrb.transform.up;
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(0, 0.12f, 52, 28, 0);
            oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, new Vector3(i, 0, 0), Quaternion.identity);
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(0, 0.12f, 52, 28, 0);
        }
        for (int i = 0; i <= 100; i += 25)
        {
            GameObject oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, new Vector3(-85, i, 0), Quaternion.identity);
            oGreenOrb.transform.up = oGreenOrb.transform.right;
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(0, 0.12f, 52, 45, 0);
            oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, new Vector3(85, i, 0), Quaternion.identity);
            oGreenOrb.transform.up = -oGreenOrb.transform.right;
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(0, 0.12f, 52, 45, 0);
        }
    }

    void GreenMagicStart()
    {
        mode = "GreenMagicAttack";
        anim.SetTrigger("FingerSlap");
    }

    void RedExplosion()
    {
        ObjectPoolManager.SpawnObject(redExplosion, magicSpawnPos.transform.position, Quaternion.identity);
    }

    void RedMagicAttack()
    {
        mode = "RedMagicAttack";
        isFireAttack = true;
        attackCD = 0.5f;
        modeCD = 5f; 
    }

    void BlueExplosion()
    {
        ObjectPoolManager.SpawnObject(blueExplosion, magicSpawnPos.transform.position, Quaternion.identity);
    }

    void BlueMagicAttack()
    {
        isThunderAttack = true;
        thunderCD = 0.15f;
    }

    void StopBlueAttack()
    {
        isThunderAttack = false;
    }

    void GreenFistAttack()
    {
        isFistAttack = true;
        attackCD = -1f;
        modeCD = 5.5f;
        mode = "GreenFistCombo";

        CamController.instance.focus = null;
        destroyPorjectile = false;
        player.lostControl = false;
        battleLoop = true;
    }

    void SpawnShortGreenAttack()
    {
        for (int i = -75; i <= 75; i += 25)
        {
            GameObject oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, new Vector3(i, 80, 0), Quaternion.identity);
            oGreenOrb.transform.up = -oGreenOrb.transform.up;
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(0, 0.12f, 28, 28, 0);
            oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, new Vector3(i, 0, 0), Quaternion.identity);
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(0, 0.12f, 28, 28, 0);
        }
        for (int i = 0; i <= 100; i += 25)
        {
            GameObject oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, new Vector3(-85, i, 0), Quaternion.identity);
            oGreenOrb.transform.up = oGreenOrb.transform.right;
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(0, 0.12f, 28, 45, 0);
            oGreenOrb = ObjectPoolManager.SpawnObject(greenOrb, new Vector3(85, i, 0), Quaternion.identity);
            oGreenOrb.transform.up = -oGreenOrb.transform.right;
            oGreenOrb.GetComponent<SpawnOrb>().SetStats(0, 0.12f, 28, 45, 0);
        }
    }

    void ShakeCam(float time)
    {
        CamController.instance.ShakeCam(time, 15);
    }

    void Heal()
    {
        startHeal = true;
    }

    void FingerSlaping()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.fingerSlap);
    }

    void BackToIdle()
    {
        modeCD = 999f;
        attackCD = 0;
        anim.SetBool("StartIdle", true);
        transform.position = player.transform.position + Vector3.up * 10;
        mode = "GroundPunch";
    }

    void EndFightStart()
    {
        CamController.instance.focusDistant = 16;
        isFistAttack = false;
        isFireAttack = false;
        isThunderAttack = false;
        destroyPorjectile = true;
        hp.active = false;
        battleLoop = false;
        CamController.instance.focus = gameObject;
        player.lostControl = true;

    }

    void EndFightStop()
    {
        CamController.instance.focus = null;
        player.lostControl = false;
        cutscene.SetActive(true);
        block.SetActive(false);
        GameManager.instance.lastboss = true;
        enabled = false;
    }

    void GroundHit()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.groundHit);
    }
}
