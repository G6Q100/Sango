using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    private RectTransform joystickTransform;
    private RectTransform selfTransform, parentTransform;

    [SerializeField]
    private int dragMovementDistance = 30;
    [SerializeField]
    private int dragOffsetDistance = 100;

    public Vector2 joyStickPos;
    public bool isMoved;
    [SerializeField] bool isMovement;

    public event Action<Vector2> onMoving;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isMovement && isMoved)
            return;
        if (isMovement && !isMoved)
            isMoved = true;

        if (isMovement)
            parentTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 300));

        joystickTransform.anchoredPosition = Vector2.zero;
        joyStickPos = Vector2.zero;
        onMoving?.Invoke(Vector2.zero);
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickTransform.anchoredPosition = Vector2.zero;
        joyStickPos = Vector2.zero;
        isMoved = false;
        onMoving?.Invoke(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 offset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickTransform,
            eventData.position,
            Camera.main,
            out offset);

        isMoved = true;
        offset = Vector2.ClampMagnitude(offset, dragOffsetDistance) / dragOffsetDistance;
        joyStickPos = offset;
        joystickTransform.anchoredPosition = (offset) * dragMovementDistance;
    }

    void Awake()
    {
        if (isMovement)
        {
            parentTransform = (RectTransform)joystickTransform.parent;
            selfTransform = (RectTransform)transform;
            selfTransform.sizeDelta = new Vector2(Screen.width / 4, Screen.height / 2);
            return;
        }

        joystickTransform = (RectTransform)transform;
    }
}
