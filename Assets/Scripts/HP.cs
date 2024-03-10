using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    public int maxHp;
    [HideInInspector] public int hp;
    [SerializeField] HP core;
    [SerializeField] bool bodyParts, boss, lastBoss;
    public bool active, hitted;

    [SerializeField] Slider HPBar;
    [SerializeField] GameObject hitEffect;

    Animator anim, HPAnim;
    public float iFrame;
    [SerializeField] float maxIFrame = 0.1f;
    Player player;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (lastBoss && Application.isMobilePlatform)
            maxHp /= 2;

        hp = maxHp;

        if (gameObject.tag == "Player")
            maxIFrame = 0.8f;

        if (HPBar != null)
        {
            HPBar.maxValue = hp;
            HPBar.value = hp;
            HPAnim = HPBar.GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    private void Update()
    {
        if (iFrame > 0)
            iFrame -= Time.deltaTime;
    }

    public void GetHit(int damage)
    {
        if (!active)
            return;

        if (bodyParts)
        {
            core.GetHit(damage);
            hitted = true;
            if (anim != null)
                anim.SetTrigger("GetHit");
            return;
        }

        CheckHealOrDamage(damage);

        if (HPBar != null)
        {
            HPBar.value = hp;
            HPAnim.SetTrigger("GetHit");
        }


        if (hp <= 0)
        {
            CamController.instance.ShakeCam(0.1f, 6f);
            if (HPBar != null)
            {
                HPBar.gameObject.SetActive(false);
                CamController.instance.target = null;
            }
            if (gameObject.tag == "Boss")
            {
                anim.SetBool("EndFight", true);
                active = false;
                return;
            }

            gameObject.SetActive(false);
        }
    }

    void CheckHealOrDamage(int damage)
    {
        if (damage <= 0)
        {
            hp -= damage;
        }
        else if(iFrame <= 0)
        {
            iFrame = maxIFrame;

            if (gameObject.tag != "Player")
            {
                player.AbilityMeterUpdate(1);
                anim.SetTrigger("GetHit");
                hp -= damage;
                return;
            }

            anim.SetTrigger("GetHit");
            SoundManager.instance.PlaySound(SoundManager.instance.getHurt);
            ObjectPoolManager.SpawnObject(hitEffect, transform.position, Quaternion.identity);
            StartCoroutine(player.GetHurt());
            hp -= damage;
        }
    }
}