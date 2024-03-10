using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFGCrystal : MonoBehaviour
{
    public Vector3 lookPos;

    [HideInInspector] public Vector3 playerPos;

    Rigidbody2D rb2d;
    public float speed, dampingSpeed, mode;

    [SerializeField] int damage;

    [SerializeField] GameObject explosion, smallCrystal;
    bool startAttack, shattering;

    float life;

    Animator anim;
    public bool smallShake;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        life = 10;
    }

    public void Attack()
    {
        startAttack = true;
        if (mode != 0)
            SoundManager.instance.PlaySound(SoundManager.instance.speed);

        if (smallShake)
            CamController.instance.ShakeCam(0.2f, 10f);
        else
            CamController.instance.ShakeCam(0.3f, 15f);
    }

    void FixedUpdate()
    {
        if (TFG.instance == null)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (!TFG.instance.gameObject.activeInHierarchy)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (TFG.instance.destroyPorjectile)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (!startAttack)
        {
            return;
        }


        rb2d.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;
        if (mode != 1)
        {
            return;
        }

        if (speed > 50)
        {
            speed -= dampingSpeed * Time.fixedDeltaTime;
            return;
        }
        if (speed > 0)
        {
            speed -= 150 * Time.fixedDeltaTime;
            if (!shattering)
            {
                shattering = true;
                anim.SetTrigger("Shatter");
            }
            return;
        }

        speed = 0;
    }

    private void OnDisable()
    {
        startAttack = false;
        shattering = false;
    }

    private void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<HP>().GetHit(damage);
        }
    }
}
