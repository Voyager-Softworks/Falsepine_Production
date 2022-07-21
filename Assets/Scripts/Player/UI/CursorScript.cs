using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour  /// @todo Comment
{
    public Image cursorImage;
    public Sprite aimCursor;
    public Sprite reloadCursor;

    public bool setOnStart = true; 

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        if (setOnStart)
        {
            SetCursor(aimCursor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        // if(Gamepad.current != null)
        // {
        //     cursorImage.enabled = false;
        // }
        // else
        {
            cursorImage.enabled = true;
            
            if (cursorImage == null || aimCursor == null || reloadCursor == null) {
                Debug.LogError("CursorScript: Missing cursor image or cursor sprites");
                return;
            }

            
            //get mouse pos
            Vector2 mousePos = Mouse.current.position.ReadValue();
            //set cursor pos
            cursorImage.transform.position = mousePos;
        }
    }

    public void SetCursor(Sprite _cursor, float _size = 1.0f)
    {
        cursorImage.sprite = _cursor;
        cursorImage.transform.localScale = new Vector3(_size, _size, _size);
    }
}
