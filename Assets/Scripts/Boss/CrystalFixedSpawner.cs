using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CrystalFixedSpawner : MonoBehaviour
{
    public Vector3 lookPos;

    [HideInInspector] public Vector3 playerPos;

    public float speed, dampingSpeed, attackCD, currentCD;
    public float range = 15;
    public int mode, rotateSpeed, maxRotateTime;

    [SerializeField] int damage, attackTime;

    [SerializeField] public GameObject crystal, explosion;

    void CrystalShooting(Vector3 spawnPos, float speed, float dampingSpeed, Vector3 dir, int mode)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.spawn);
        GameObject oCrystal = ObjectPoolManager.SpawnObject(crystal, spawnPos, Quaternion.Euler(0, 0, transform.eulerAngles.z - 180));
        oCrystal.GetComponent<Crystal>().speed = speed;
        oCrystal.GetComponent<Crystal>().dampingSpeed = dampingSpeed;
        oCrystal.GetComponent<Crystal>().lookPos = dir;

        if (mode == 0)
        {
            oCrystal.GetComponent<Crystal>().smallShake = true;
            oCrystal.GetComponent<Animator>().speed = 1;
            oCrystal.GetComponent<Crystal>().Attack();
            ObjectPoolManager.SpawnObject(explosion, transform.position, Quaternion.identity);
        }
        else if (mode == 1)
        {
            oCrystal.GetComponent<Crystal>().smallShake = false;
            oCrystal.GetComponent<Animator>().speed = 0.75f;
            oCrystal.GetComponent<Crystal>().mode = mode;
        }
        else
        {
            oCrystal.GetComponent<Crystal>().smallShake = false;
            oCrystal.GetComponent<Animator>().speed = 1;
            oCrystal.GetComponent<Crystal>().mode = 1;
        }
    }

    private void OnDisable()
    {
        currentCD = 0;
        attackTime = 0;
    }

    private void Update()
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

        currentCD += Time.deltaTime;
        if (currentCD >= attackCD)
        {
            currentCD = 0;

            CrystalShooting(transform.position + transform.up * range, speed, dampingSpeed, transform.up, mode);

            transform.Rotate(0, 0, rotateSpeed);
            attackTime ++;
            if (attackTime > maxRotateTime)
            {
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
        }
    }
}
