using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Slider[] abilityMeters;
    GameObject player;

    [SerializeField] Animator transition;
    public bool restarting, lastboss;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            instance.player = GameObject.FindGameObjectWithTag("Player");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            transition.SetTrigger("Transition");
            restarting = true;
        }

        if (player == null)
        {
            return;
        }

        if (player.activeInHierarchy)
            return;

        if (restarting)
            return;

        if (lastboss)
        {
            transition.GetComponent<LoadLevel>().SoulTrade();
            restarting = true;
            return;
        }

        transition.SetTrigger("Transition");
        restarting = true;
    }
}
