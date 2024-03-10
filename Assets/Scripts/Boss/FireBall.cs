using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FireBall : MonoBehaviour
{
    public Vector3 lookPos;

    public Vector3 targerPos;

    Rigidbody2D rb2d;
    public float speed = 150, mode, rotateSpeed;

    float attackCD = 0.5f;
    int goingDown = 0;

    [SerializeField] int damage;

    [SerializeField] GameObject explosion, dangerZone, smallFireBall;

    public bool smallShake;
    Vector3 startSize;
    float life;

    void OnEnable()
    {
        goingDown = 0;
        attackCD = 0.3f;
        life = 12;
        rotateSpeed = 72;
        if (startSize == Vector3.zero)
            startSize = transform.localScale;
        else
            transform.localScale = startSize;
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Attack(Vector3 dir)
    {
        CamController.instance.ShakeCam(0.2f, 5f);
        lookPos = dir;
    }

    void FixedUpdate()
    {
        if (goingDown == 1)
            return;

        if (mode == 2)
        {
            rb2d.velocity = transform.up * speed;
            if (rotateSpeed < 1f)
            {
                return;
            }

            transform.Rotate(0, 0, Time.deltaTime * rotateSpeed);
            rotateSpeed *= 1 - Time.deltaTime * 0.5f;
            return;
        }

        rb2d.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;
    }

    private void Update()
    {
        if (ChimeraBeast.instance == null)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (!ChimeraBeast.instance.gameObject.activeInHierarchy)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        if (ChimeraBeast.instance.destroyPorjectile)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            return;
        }

        switch (mode)
        {
            case 0:
                Attacking();
                break;
            case 1:
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Player")
                    {
                        collider.GetComponent<HP>().GetHit(damage);
                    }
                    if (collider.tag == "Wave")
                    {
                        SpawningFireBall(smallFireBall, transform.position, 0, 2, 10);
                        SpawningFireBall(smallFireBall, transform.position, 120, 2, 10);
                        SpawningFireBall(smallFireBall, transform.position, 240, 2, 10);
                        CamController.instance.ShakeCam(0.25f, 10f);
                        ObjectPoolManager.SpawnObject(explosion, transform.position, Quaternion.identity);
                        ObjectPoolManager.ReturnObjectToPool(gameObject);
                    }
                }
                break;
            case 2:
                colliders = Physics2D.OverlapCircleAll(transform.position, 1.3f);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Player")
                    {
                        collider.GetComponent<HP>().GetHit(damage);
                    }
                }
                break;
            case 3:
                colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Player")
                    {
                        collider.GetComponent<HP>().GetHit(damage);
                    }
                    if (collider.tag == "Wave")
                    {
                        SpawningFireBall(smallFireBall, transform.position, 0, 2, 10);
                        SpawningFireBall(smallFireBall, transform.position, 72, 2, 10);
                        SpawningFireBall(smallFireBall, transform.position, 144, 2, 10);
                        SpawningFireBall(smallFireBall, transform.position, 216, 2, 10);
                        SpawningFireBall(smallFireBall, transform.position, 288, 2, 10);
                        CamController.instance.ShakeCam(0.25f, 10f);
                        ObjectPoolManager.SpawnObject(explosion, transform.position, Quaternion.identity);
                        ObjectPoolManager.ReturnObjectToPool(gameObject);
                    }
                }
                break;
        }
        life -= Time.deltaTime;
        if (life <= 0.5f)
        {
            transform.localScale = startSize * life * 2;
        }
        if (life < 0)
        {
            ObjectPoolManager.SpawnObject(explosion, transform.position, Quaternion.identity);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
    void SpawningFireBall(GameObject fireball, Vector3 pos, float rotZ,
        int mode, float speed)
    {
        GameObject ofirebal = ObjectPoolManager.SpawnObject(fireball, pos,
            Quaternion.Euler(0, 0, rotZ));
        ofirebal.GetComponent<FireBall>().mode = mode;
        ofirebal.GetComponent<FireBall>().speed = speed;
    }

    void Attacking()
    {
        if (attackCD > 0)
        {
            attackCD -= Time.deltaTime;
            return;
        }

        if (goingDown == 0)
        {
            attackCD = 1;
            goingDown = 1;
            ObjectPoolManager.SpawnObject(dangerZone, targerPos, Quaternion.identity);
            transform.position = new Vector3(targerPos.x, targerPos.y + 90, 0);
            return;
        }
        if (goingDown == 1)
        {
            goingDown = 2;
            speed = 200;
            transform.position = new Vector3(targerPos.x, targerPos.y + 30, 0);
        }

        lookPos = Vector3.down;
        if (transform.position.y < targerPos.y)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.groundHit);
            CamController.instance.ShakeCam(0.2f, 10);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(targerPos, 3);

            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    collider.GetComponent<HP>().GetHit(damage);
                }
            }

            ObjectPoolManager.SpawnObject(explosion, transform.position, Quaternion.identity);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}
