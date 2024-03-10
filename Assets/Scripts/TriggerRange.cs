using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRange : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        gameObject.SetActive(false);
    }
}
