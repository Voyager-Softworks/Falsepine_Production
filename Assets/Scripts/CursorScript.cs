using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{
    public Image cursorImage;
    public Sprite aimCursor;
    public Sprite reloadCursor;

    float maxAimSize = 3.0f;
    float minAimSize = 1.0f;
    float currentAimSize = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cursorImage == null || aimCursor == null || reloadCursor == null) {
            Debug.LogError("CursorScript: Missing cursor image or cursor sprites");
            return;
        }

        Cursor.visible = false;
        //get mouse pos
        Vector2 mousePos = Mouse.current.position.ReadValue();
        //set cursor pos
        cursorImage.transform.position = mousePos;

        //if holding right click, set cursor to reload
        if (Mouse.current.rightButton.isPressed)
        {
            currentAimSize = Mathf.Lerp(currentAimSize, minAimSize, Time.deltaTime * 5.0f);
        }
        else
        {
            currentAimSize = Mathf.Lerp(currentAimSize, maxAimSize, Time.deltaTime * 1.0f);
        }

        SetCursor(reloadCursor, currentAimSize);
        //make transparent the larger the aim size
        cursorImage.color = new Color(1, 1, 1, 1 - ((currentAimSize - minAimSize) / (maxAimSize - minAimSize)) * 0.85f);
    }

    public void SetCursor(Sprite _cursor, float _size = 1.0f)
    {
        cursorImage.sprite = _cursor;
        cursorImage.transform.localScale = new Vector3(_size, _size, _size);
    }
}
