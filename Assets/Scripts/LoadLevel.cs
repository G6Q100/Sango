using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public int loadLevel;
    Animator anim;
    Player player;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void PlayerStopMoving()
    {
        if (player == null)
            return;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        GameManager.instance.restarting = false;
        player.lostControl = true;
    }
    void PlayerStartMoving()
    {
        if (player == null)
            return;

        player.lostControl = false;
    }

    public void StartTransition()
    {
        anim.SetTrigger("Transition");
    }
    public void NextLevel()
    {
        anim.SetTrigger("Transition");
        loadLevel++;
    }

    public void Restart()
    {
        anim.SetTrigger("Transition");
        loadLevel = 0;
    }

    public void TryAgain()
    {
        anim.SetTrigger("Transition");
        loadLevel = 3;
    }

    public void GiveUpSoul()
    {
        anim.SetTrigger("Transition");
        loadLevel = 4;
    }
    public void SoulTrade()
    {
        anim.SetTrigger("Transition");
        loadLevel = 5;
    }
    public void BadEnding()
    {
        anim.SetTrigger("Transition");
        loadLevel = 6;
    }

    public void SecretEnding()
    {
        anim.SetTrigger("Transition");
        loadLevel = 7;
    }

    void Loading()
    {
        anim.SetTrigger("Transition");
        SceneManager.LoadScene(loadLevel);
    }
}
