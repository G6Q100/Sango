using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionHand : MonoBehaviour
{
    public LoadLevel loadLevel;

    Player player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void LookAtHand()
    {
        CamController.instance.focusDistant = 16;
        CamController.instance.focus = gameObject;
        player.lostControl = true;
    }

    void FingerSlap()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.fingerSlap);
    }

    void Transition()
    {
        loadLevel.NextLevel();
    }
}
