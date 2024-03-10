using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 lookPos;

    Rigidbody2D rb2d;
    [SerializeField] float speed;
    [SerializeField] GameObject explosion;

    float lifetime = 3f;

    void OnEnable()
    {
        lifetime = 3f;
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GetComponent<TrailRenderer>().time <= 0.05f)
            GetComponent<TrailRenderer>().time = 0.05f;

        lifetime -= Time.deltaTime;
        if (lifetime <= 3f)
        {
            transform.localScale = Vector3.one * (lifetime * 2 + 0.1f);
        }
        if (lifetime <= 0)
        {
            lifetime = 3f;
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "TriggerWall" || 
            collision.tag == "BulletIngore" || collision.tag == "Bullet" ||
            collision.tag == "Wave")
            return;

        ObjectPoolManager.SpawnObject(explosion, transform.position, Quaternion.identity);
        SoundManager.instance.PlaySound(SoundManager.instance.gunHit);
        lifetime = 0.02f;

        if (collision.tag != "Enemy" && collision.tag != "Boss")
            return;

        HP collsionHP = collision.GetComponent<HP>();

        if (collsionHP == null)
            return;

        if (collsionHP.enabled == false)
            return;

        collsionHP.GetHit(1);
    }
}
