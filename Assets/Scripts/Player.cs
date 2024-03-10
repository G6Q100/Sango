using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    float speed = 12f;

    [HideInInspector] public Vector2 movement = Vector2.zero;
    Vector2 dashDir;
    float dashCD;
    [SerializeField] ParticleSystem dashEffect;

    Vector3 mousePos;
    Vector2 lookPos;
    [SerializeField] GameObject gun;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform attackPos;
    float attackCD;

    float runningCD;

    Rigidbody2D rb2;
    Animator anim;

    HP hp;

    [SerializeField] bool isDashing, isRunning;

    int abilityPoint, abilityBar, maxAbilityBar;
    int tempAbilityPoint, lastAbilityPoint;
    [SerializeField] Slider[] abilityMeters;

    public bool lostControl;
    bool starting;

    [SerializeField] MobileJoyStick movementJoyStick, attackJoyStick;
    [SerializeField] ClickAbility healButton, dashButton, runButton;
    Image healColor, dashColor, runColor;

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hp = GetComponent<HP>();
        abilityMeters = GameManager.instance.abilityMeters;
        abilityBar = (int)abilityMeters[0].maxValue;
        maxAbilityBar = abilityMeters.Length * abilityBar;
        AbilityMeterUpdate(maxAbilityBar);
        attackCD = 0.2f;
        runButton.clicked = false;

        runColor = runButton.GetComponent<Image>();
        dashColor = dashButton.GetComponent<Image>();
        healColor = healButton.GetComponent<Image>();
    }

    void Update()
    {
        if (lostControl == true)
        {
            anim.SetBool("Walking", false);
            anim.SetBool("WalkBack", false);
            return;
        }

        LookAtMouse();
        WalkAnim();

        if (Application.isMobilePlatform)
            CheckActive();

        Movement();
        Attack();
        if (isDashing)
            Dash();
        Heal();
    }

    void FixedUpdate()
    {
        if (lostControl == true)
        {
            rb2.velocity = Vector2.zero;
            hp.active = false;
            return;
        }

        rb2.velocity = movement * speed;

        if (dashCD > 0.2f)
        {
            hp.active = false;
            return;
        }

        hp.active = true;
    }

    void CheckActive()
    {
        if (runButton.clicked && abilityPoint >= 1)
        {
            runColor.color = new Color(runColor.color.r, runColor.color.g, runColor.color.b, 1);
        }
        else
        {
            runColor.color = new Color(runColor.color.r, runColor.color.g, runColor.color.b, 0.2f);
        }

        if (abilityPoint >= abilityBar)
        {
            dashColor.color = new Color(dashColor.color.r, dashColor.color.g, dashColor.color.b, 1);
        }
        else
        {
            dashColor.color = new Color(dashColor.color.r, dashColor.color.g, dashColor.color.b, 0.2f);
        }

        if (abilityPoint >= abilityBar * 4)
        {
            healColor.color = new Color(healColor.color.r, healColor.color.g, healColor.color.b, 1);
        }
        else
        {
            healColor.color = new Color(healColor.color.r, healColor.color.g, healColor.color.b, 0.2f);
        }
    }

    void WalkAnim()
    {
        CheckWalkAnimDir();

        if (!Mathf.Approximately(movement.x, 0))
        {
            anim.SetBool("Walking", true);
            return;
        }
        if (!Mathf.Approximately(movement.y, 0))
        {
            anim.SetBool("Walking", true);
            return;
        }

        anim.SetBool("Walking", false);
    }

    void CheckWalkAnimDir()
    {
        if (transform.position.x > mousePos.x && movement.x < 0)
        {
            anim.SetBool("WalkBack", false);
            return;
        }

        if (transform.position.x <= mousePos.x && movement.x >= 0)
        {
            anim.SetBool("WalkBack", false);
            return;
        }

        if (movement.x == 0)
        {
            anim.SetBool("WalkBack", false);
            return;
        }

        anim.SetBool("WalkBack", true);
    }

    void Movement()
    {
        if (dashCD > 0.2f)
            return;

        if ((Input.GetKey(KeyCode.LeftShift) || runButton.clicked) && abilityPoint >= 1 && isRunning)
        {
            anim.speed = 1;
            Running();
            if (Application.isMobilePlatform)
            {
                MobileMovement(true);
                return;
            }
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 1.5f;
            return;
        }

        if (runButton.clicked && abilityPoint < 1)
        {
            runButton.clicked = false;
        }

        runningCD = 0;

        anim.speed = 0.6f;
        if (Application.isMobilePlatform)
        {
            MobileMovement(false);
            return;
        }
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    void MobileMovement(bool isRunning)
    {
        movement.x = Mathf.Clamp(movementJoyStick.joyStickPos.x * 2, -1, 1);
        movement.y = Mathf.Clamp(movementJoyStick.joyStickPos.y * 2, -1, 1);

        movement = movement.normalized;

        if (!isRunning)
            return;

        movement.x *= 1.5f;
        movement.y *= 1.5f;
    }

    void Running()
    {
        if (Vector2.Distance(movement, Vector2.zero) < 0.1f)
        {
            runningCD = 0.26f;
            return;
        }

        runningCD += Time.deltaTime;
        if (runningCD >= 0.25f)
        {
            runningCD = 0;
            AbilityMeterUpdate(-1);
        }
    }

    void Attack()
    {
        if (attackCD > 0)
        {
            attackCD -= Time.deltaTime;
            return;
        }
        if (Application.isMobilePlatform)
        {
            if (attackJoyStick.isMoved)
            {
                attackCD = 0.2f;
                lookPos = attackJoyStick.joyStickPos;
                float rotZ = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

                CamController.instance.ShakeCam(0.1f, 2);
                GameObject oBullet = ObjectPoolManager.SpawnObject(bullet, attackPos.position, Quaternion.Euler(0, 0, rotZ));
                oBullet.GetComponent<Bullet>().lookPos = lookPos;
                oBullet.GetComponent<TrailRenderer>().time = 0;

                SoundManager.instance.PlaySound(SoundManager.instance.gunShot);
            }
            return;
        }

        if (Input.GetMouseButton(0))
        {
            attackCD = 0.2f;
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

            lookPos = mousePos - gun.transform.position;
            gun.gameObject.transform.position -= new Vector3(lookPos.x, lookPos.y, 0).normalized * 0.2f;
            float rotZ = Mathf.Atan2(mousePos.y - attackPos.position.y, mousePos.x - attackPos.position.x) * Mathf.Rad2Deg;

            CamController.instance.ShakeCam(0.1f, 2); 
            GameObject oBullet = ObjectPoolManager.SpawnObject(bullet, attackPos.position, Quaternion.Euler(0, 0, rotZ));
            oBullet.GetComponent<Bullet>().lookPos = mousePos - attackPos.position;
            oBullet.GetComponent<TrailRenderer>().time = 0;

            SoundManager.instance.PlaySound(SoundManager.instance.gunShot);
        }

    }

    void Dash()
    {
        if (dashCD > 0.2f)
        {
            dashCD -= Time.deltaTime;
            movement = dashDir * 2.5f;
            return;
        }

        if (dashCD > 0)
        {
            dashCD -= Time.deltaTime;
            return;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || dashButton.clicked) && abilityPoint >= abilityBar)
        {
            dashButton.clicked = false;
            dashCD = 0.5f;
            dashDir = movement;
            dashEffect.Play();
            SoundManager.instance.PlaySound(SoundManager.instance.dash);

            AbilityMeterUpdate(-abilityBar);
        }
    }

    void Heal()
    {
        if ((Input.GetKeyDown(KeyCode.F) || healButton.clicked) && abilityPoint >= abilityBar * 4)
        {
            healButton.clicked = false;

            hp.GetHit(-1);

            SoundManager.instance.PlaySound(SoundManager.instance.heal);
            AbilityMeterUpdate(-abilityBar * 4);
        }
    }

    public void AbilityMeterUpdate(int number)
    {
        abilityPoint += number;
        abilityPoint = Mathf.Clamp(abilityPoint, 0, maxAbilityBar);

        if (abilityPoint > lastAbilityPoint && abilityPoint % abilityBar == 0 && abilityPoint != 0 && starting)
            SoundManager.instance.PlaySound(SoundManager.instance.addAbilityUse);
        else if (!starting)
        {
            starting = true;
        }

        lastAbilityPoint = abilityPoint;

        tempAbilityPoint = abilityPoint;
        foreach (Slider abilityMeter in abilityMeters)
        {
            Image fillColor = abilityMeter.fillRect.gameObject.GetComponent<Image>();
            fillColor.color = new Color(fillColor.color.r, fillColor.color.g, fillColor.color.b, 0.35f);
            if (tempAbilityPoint < abilityBar)
            {
                abilityMeter.value = tempAbilityPoint;
                tempAbilityPoint = 0;
                continue;
            }
            tempAbilityPoint -= abilityBar;
            abilityMeter.value = abilityBar;
            fillColor.color = new Color(fillColor.color.r, fillColor.color.g, fillColor.color.b, 1);
        }
    }

    void LookAtMouse()
    {
        if (!attackJoyStick.isMoved && Application.isMobilePlatform)
            return;

        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));


        if (Application.isMobilePlatform)
        {
            lookPos = attackJoyStick.joyStickPos;
        }
        else
        {
            lookPos = mousePos - gun.transform.position;
        }

        if (lookPos.x < 0)
        {
            gun.transform.rotation = Quaternion.Euler(0, 180, Mathf.Atan2(lookPos.y, -lookPos.x) * 48);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            return;
        }

        gun.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(lookPos.y, lookPos.x) * 48);
        transform.rotation = Quaternion.identity;
    }

    public IEnumerator GetHurt()
    {
        CamController.instance.getHurt = true;
        yield return new WaitForSecondsRealtime(0.5f);
        CamController.instance.getHurt = false;
    }
}
