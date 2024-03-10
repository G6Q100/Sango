using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAbility : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool clicked, isPressed;

    void FlipBool()
    {
        if (clicked)
        {
            clicked = false;
        }
        else
        {
            clicked = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isPressed)
        {
            FlipBool();
            return;
        }

        clicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            return;
        }

        clicked = false;
    }
}
