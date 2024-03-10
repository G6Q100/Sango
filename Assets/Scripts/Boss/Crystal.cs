using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Crystal : MonoBehaviour
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

    void CrystalShatter()
    {
        float speed = 15;
        float dampingSpeed = 0;
        SoundManager.instance.PlaySound(SoundManager.instance.shatter);
        ObjectPoolManager.SpawnObject(explosion, transform.position, Quaternion.identity);
        CrystalShooting(transform.position, speed, dampingSpeed, Vector3.left + Vector3.up, 45, 0);
        CrystalShooting(transform.position, speed, dampingSpeed, Vector3.right + Vector3.up, 135, 0);
        CrystalShooting(transform.position, speed, dampingSpeed, Vector3.left + Vector3.down, 135, 0);
        CrystalShooting(transform.position, speed, dampingSpeed, Vector3.right + Vector3.down, 45, 0);

        CrystalShooting(transform.position, speed, dampingSpeed, Vector3.left, 90, 0);
        CrystalShooting(transform.position, speed, dampingSpeed, Vector3.right, 90, 0);
        CrystalShooting(transform.position, speed, dampingSpeed, Vector3.up, 0, 0);
        CrystalShooting(transform.position, speed, dampingSpeed, Vector3.down, 0, 0);
        CamController.instance.ShakeCam(0.2f, 15f);
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    void CrystalShooting(Vector3 spawnPos, float speed, float dampingSpeed, Vector3 dir, float rotZ, int mode)
    {
        GameObject oCrystal = ObjectPoolManager.SpawnObject(smallCrystal, spawnPos, Quaternion.Euler(0, 0, rotZ));
        oCrystal.GetComponent<Crystal>().speed = speed;
        oCrystal.GetComponent<Crystal>().dampingSpeed = dampingSpeed;
        oCrystal.GetComponent<Crystal>().lookPos = dir;

        if (mode == 0)
            oCrystal.GetComponent<Crystal>().Attack();
        else
            oCrystal.GetComponent<Crystal>().mode = mode;
    }

    void FixedUpdate()
    {
        if (CrystalMonarch.instance == null)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (!CrystalMonarch.instance.gameObject.activeInHierarchy)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (CrystalMonarch.instance.destroyPorjectile)
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
