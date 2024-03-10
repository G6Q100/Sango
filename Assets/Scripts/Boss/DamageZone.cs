using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<HP>().GetHit(1);
        }
    }
}
