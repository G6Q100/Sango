using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFGPurpleOrb : MonoBehaviour
{
    public Vector3 lookPos;

    [HideInInspector] public Vector3 playerPos;

    Rigidbody2D rb2d;
    public float speed;

    [SerializeField] int damage;

    [SerializeField] GameObject explosion;

    float life;

    Animator anim;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        life = 12;
    }

    public void Attack()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.speed);
        CamController.instance.ShakeCam(0.3f, 15f);
    }

    void CrystalShatter()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.shatter);
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;
    }

    private void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<HP>().GetHit(damage);
        }
    }
}
