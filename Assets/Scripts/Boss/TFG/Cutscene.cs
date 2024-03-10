using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeField] GameObject tfg, focusPoint;

    Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void SpawnTFG()
    {
        CamController.instance.focusDistant = 18;
        CamController.instance.focus = focusPoint;
        tfg.transform.position = player.gameObject.transform.position + Vector3.up * 25;
        player.lostControl = true;
        tfg.SetActive(true);
    }
}
