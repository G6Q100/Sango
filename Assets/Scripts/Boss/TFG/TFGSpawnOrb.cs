using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFGSpawnOrb : MonoBehaviour
{
    public float attackCD, currentCD, speed;
    public int mode, rotateSpeed, maxRotateTime, type;
    [SerializeField] int attackTime;
    [SerializeField] GameObject orb, explosion;
    Quaternion startRotation;

    private void OnEnable()
    {
        rotateSpeed = 90;
        attackCD = 0.03f;
        currentCD = attackCD;
        attackTime = 0;
        speed = 16;
        maxRotateTime = 64;
        type = 0;
    }

    public void SetStats(int rotSpeed, float attack, int maxRotTime, float speeds, int types)
    {
        rotateSpeed = rotSpeed;
        attackCD = attack;
        currentCD = attackCD;
        attackTime = 0;
        maxRotateTime = maxRotTime;
        speed = speeds;
        type = types;
        if (type == 1)
            startRotation = transform.rotation;
    }

    void CrystalShooting(int mode)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.spawn);
        GameObject oGreenOrb = ObjectPoolManager.SpawnObject(orb, transform.position, Quaternion.Euler(transform.up));
        ObjectPoolManager.SpawnObject(explosion, transform.position, Quaternion.identity);
        oGreenOrb.GetComponent<TFGPurpleOrb>().lookPos = transform.up;
        if (mode == 0)
        {
            oGreenOrb.GetComponent<TFGPurpleOrb>().speed = speed;
            oGreenOrb.gameObject.transform.localScale = Vector3.one * 0.8f;
            return;
        }
        if (mode == 1)
        {
            float rand = Random.Range(0.65f, 1f);
            oGreenOrb.GetComponent<TFGPurpleOrb>().speed = speed * rand;
            rand *= Random.Range(0.8f, 1f);
            oGreenOrb.gameObject.transform.localScale = Vector3.one * 0.5f / rand;
            return;
        }
    }

    void Update()
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

        currentCD += Time.deltaTime;
        if (currentCD >= attackCD)
        {
            currentCD = 0;


            if (type == 0)
            {
                CrystalShooting(0);
                transform.Rotate(0, 0, rotateSpeed);
            }
            else if (type == 1)
            {
                CrystalShooting(1);
                transform.rotation = startRotation;
                transform.Rotate(0, 0, Random.Range(-rotateSpeed, rotateSpeed));
            }

            attackTime++;
            if (attackTime > maxRotateTime)
            {
                gameObject.SetActive(false);
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
        }
    }
}
