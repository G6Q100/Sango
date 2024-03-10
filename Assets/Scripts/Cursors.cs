using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursors : MonoBehaviour
{
    Vector2 currentCursorPos, lastCursorPos;

    private void Start()
    {
        if (!Application.isMobilePlatform)
        {
            Cursor.visible = false;
            gameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            lastCursorPos = gameObject.transform.position;
            currentCursorPos = gameObject.transform.position;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        gameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }
}
