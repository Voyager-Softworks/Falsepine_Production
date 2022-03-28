using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{
    public Texture2D aimCursor;
    public Texture2D reloadCursor;

    // Start is called before the first frame update
    void Start()
    {
        SetCursor(aimCursor);
    }

    // Update is called once per frame
    void Update()
    {
        //if holding r, set reload cursor
        if (Keyboard.current.rKey.isPressed)
        {
            SetCursor(reloadCursor);
        }
        else
        {
            SetCursor(aimCursor);
        }
    }

    public void SetCursor(Texture2D cursorImage)
    {
        Cursor.SetCursor(cursorImage, new Vector2(cursorImage.width/2, cursorImage.height/2), CursorMode.Auto);
    }
}
