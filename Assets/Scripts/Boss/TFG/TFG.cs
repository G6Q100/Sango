using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TFG : MonoBehaviour
{
    [SerializeField] GameObject cutscene1, cutscene2, focusPoint;
    [SerializeField] GameObject hand1Pos, hand2Pos, eye, eye2, eye3, crystal, purpleOrb, greenOrb, redOrb, mouth, thunder;
    [SerializeField] Slider bossHP;

    [SerializeField] LoadLevel loadLevel;
     
    Animator anim;
    HP hp;

    Vector3 refVector3, targetPos;

    Player player;

    int rand, lastRand, phase = 1;
    float mouthSpawnCD, maxMouthSpawnCD = 2, thunderCD;
    bool isSpawningMouth, isFire, isThunderAttack;

    public bool destroyPorjectile;

    public static TFG instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        CamController.instance.focusDistant = 18;
        CamController.instance.focus = focusPoint;

        anim = GetComponent<Animator>();
        hp = GetComponent<HP>();
        hp.active = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.lostControl = true;

        targetPos = transform.position;
        SoundManager.instance.boss1BGM.volume = 0;
        SoundManager.instance.boss1BGM.Play();
    }

    private void Update()
    {
        if (hp.hp <= 0)
        {
            SoundManager.instance.boss1BGM.volume -= Time.deltaTime / 2;
        }
        else if (SoundManager.instance.boss1BGM.volume < 1)
        {
            SoundManager.instance.boss1BGM.volume += Time.deltaTime / 2;
        }

        SpawnMouth();
        ThunderAttackLoop();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref refVector3, Mathf.Sqrt(0.1f));
    }

    void SpawnMouth()
    {
        if (!isSpawningMouth)
            return;

        mouthSpawnCD += Time.deltaTime;
        if (mouthSpawnCD >= maxMouthSpawnCD)
        {
            mouthSpawnCD = 0;
            ObjectPoolManager.SpawnObject(mouth, player.transform.position, Quaternion.identity);
        }
    }

    void ThunderAttackLoop()
    {
        if (!isThunderAttack)
            return;

        thunderCD += Time.deltaTime;
        if (thunderCD >= 0.5f)
        {
            thunderCD = 0;
            Vector3 randOffset = new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0);
            ObjectPoolManager.SpawnObject(thunder, player.transform.position + randOffset, Quaternion.identity);
        }
    }

    //Animation Event
    void Focusing(int distant)
    {
        CamController.instance.focusDistant = distant;
    }

    void CutsceneSpawn(int mode)
    {
        if (mode == 0)
            cutscene1.SetActive(true);
        else
            cutscene2.SetActive(true);
    }

    void CutsceneEnd(int mode)
    {
        if (mode == 0)
            cutscene1.SetActive(false);
        else
            cutscene2.SetActive(false);
    }

    void StartBattle()
    {
        hp.active = true;
        CamController.instance.bossDistant = 23;
        CamController.instance.target = gameObject;
        CamController.instance.focus = null;
        CamController.instance.lastBoss = true;
        player.lostControl = false;
    }

    void Attack()
    {
        CamController.instance.bossDistant = 23;

        if (hp.hp <= hp.maxHp * 0.8f && phase == 1)
        {
            phase = 2;
            transform.position = player.transform.position + Vector3.up * 20;
            targetPos = transform.position + Vector3.up * 10;
            anim.SetTrigger("SpawnMouth");
            lastRand = 0;
            return;
        }
        if (hp.hp <= hp.maxHp * 0.6f && !isFire)
        {
            isFire = true;
            transform.position = player.transform.position + Vector3.down * 5;
            targetPos = transform.position;
            anim.SetTrigger("SpawnFire");
            lastRand = 0;
            return;
        }
        if (hp.hp <= hp.maxHp * 0.3f && phase == 2)
        {
            phase = 3;
            transform.position = player.transform.position + Vector3.up * 20;
            targetPos = transform.position + Vector3.up * 10;
            anim.SetTrigger("SpawnMouth");
            lastRand = 0;
            return;
        }

        while (rand == lastRand)
        {
            rand = Random.Range(0, 4);
        }

        lastRand = rand;

        switch (rand)
        {
            case 0:
                transform.position = player.transform.position + Vector3.up * 20;
                targetPos = transform.position + Vector3.up * 10;
                anim.SetTrigger("TopAttack");
                break;
            case 1:
                transform.position = player.transform.position + Vector3.down * 20;
                targetPos = transform.position + Vector3.down * 10;
                anim.SetTrigger("BottomAttack");
                break;
            case 2:
                transform.position = player.transform.position + Vector3.left * 20;
                targetPos = transform.position + Vector3.left * 10;
                anim.SetTrigger("LeftAttack");
                break;
            case 3:
                transform.position = player.transform.position + Vector3.right * 20;
                targetPos = transform.position + Vector3.right * 10;
                anim.SetTrigger("RightAttack");
                break;
        }
    }

    void Dash(int mode)
    {
        switch (mode)
        {
            case 0:
                targetPos += Vector3.down * 55;
                break;
            case 1:
                targetPos += Vector3.up * 55;
                break;
            case 2:
                targetPos += Vector3.right * 55;
                break;
            case 3:
                targetPos += Vector3.left * 55;
                break;
        }
    }

    void SpawnCrystal(int mode)
    {
        if (mode == 0) 
        {
            for (int i = 0; i < 360; i += 15)
            {
                GameObject oCrystal = ObjectPoolManager.SpawnObject(crystal, hand1Pos.transform.position, Quaternion.identity);
                oCrystal.transform.rotation = Quaternion.Euler(0, 0, i);
                oCrystal.GetComponent<TFGCrystal>().lookPos = oCrystal.transform.up;

                oCrystal = ObjectPoolManager.SpawnObject(crystal, hand2Pos.transform.position, Quaternion.identity);
                oCrystal.transform.rotation = Quaternion.Euler(0, 0, i);
                oCrystal.GetComponent<TFGCrystal>().lookPos = oCrystal.transform.up;
            }
        }

        if (mode == 1)
        {
            for (int i = 7; i < 353; i += 15)
            {
                GameObject oCrystal = ObjectPoolManager.SpawnObject(crystal, hand1Pos.transform.position, Quaternion.identity);
                oCrystal.transform.rotation = Quaternion.Euler(0, 0, i);
                oCrystal.GetComponent<TFGCrystal>().lookPos = oCrystal.transform.up;

                oCrystal = ObjectPoolManager.SpawnObject(crystal, hand2Pos.transform.position, Quaternion.identity);
                oCrystal.transform.rotation = Quaternion.Euler(0, 0, i);
                oCrystal.GetComponent<TFGCrystal>().lookPos = oCrystal.transform.up;
            }
        }
    }

    void SpawnOrb()
    {
        GameObject oPurpleOrb = ObjectPoolManager.SpawnObject(purpleOrb, hand1Pos.transform.position, Quaternion.identity);
        oPurpleOrb.transform.up = player.transform.position - oPurpleOrb.transform.position;
        oPurpleOrb.GetComponent<TFGSpawnOrb>().SetStats(16, 0.04f, 64, 20, 0);
        oPurpleOrb = ObjectPoolManager.SpawnObject(purpleOrb, hand2Pos.transform.position, Quaternion.identity);
        oPurpleOrb.transform.up = player.transform.position - oPurpleOrb.transform.position;
        oPurpleOrb.GetComponent<TFGSpawnOrb>().SetStats(16, 0.04f, 64, 20, 0);
    }

    void SpawnManyOrb()
    {
        GameObject oPurpleOrb = ObjectPoolManager.SpawnObject(purpleOrb, eye3.transform.position, Quaternion.identity);
        oPurpleOrb.transform.up = player.transform.position - oPurpleOrb.transform.position;
        oPurpleOrb.GetComponent<TFGSpawnOrb>().SetStats(32, 0.16f, 72, 15, 0);

        oPurpleOrb = ObjectPoolManager.SpawnObject(redOrb, eye2.transform.position, Quaternion.identity);
        oPurpleOrb.transform.up = player.transform.position - oPurpleOrb.transform.position;
        oPurpleOrb.GetComponent<TFGSpawnOrb>().SetStats(28, 0.16f, 72, 15, 0);

        oPurpleOrb = ObjectPoolManager.SpawnObject(greenOrb, eye.transform.position, Quaternion.identity);
        oPurpleOrb.transform.up = player.transform.position - oPurpleOrb.transform.position;
        oPurpleOrb.GetComponent<TFGSpawnOrb>().SetStats(26, 0.16f, 72, 15, 0);
    }

    void SpawningMouth()
    {
        if (isSpawningMouth)
        {
            maxMouthSpawnCD -= 0.5f;
            isThunderAttack = true;
            thunderCD = 0.5f;
            return;
        }
        isSpawningMouth = true;
        mouthSpawnCD = maxMouthSpawnCD;
    }
    void EndFightStart()
    {
        CamController.instance.focusDistant = 16;
        isFire = false;
        isThunderAttack = false;
        destroyPorjectile = true;
        hp.active = false;
        CamController.instance.focus = focusPoint;
        player.lostControl = true;

    }

    void EndFightStop()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            loadLevel.SecretEnding();
        }
        else
        {
            loadLevel.BadEnding();
        }

        CamController.instance.focus = null;
        player.lostControl = false;
        enabled = false;
    }

    void ZoomOut()
    {
        CamController.instance.bossDistant = 30;
    }

    void ShowHP()
    {
        bossHP.GetComponent<Animator>().SetBool("FadeIn", true);
    }

    void ShakeCam(float time)
    {
        CamController.instance.ShakeCam(time, 15);
    }
    void Shout()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.beast);
    }
}
