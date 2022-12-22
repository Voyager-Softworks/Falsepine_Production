using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// Manages the display of the custom cursor.
/// </summary>
public class CursorScript : MonoBehaviour
{
    public Image cursorImage;
    public Sprite aimCursor;
    public Sprite mouseCursor;

    public bool setOnStart = true;

    public List<Utilities.SceneField> mouseOnlyScenes;

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
    void LateUpdate()
    {
        bool isMouseOnly = false;
        foreach (Utilities.SceneField scene in mouseOnlyScenes)
        {
            if (scene.EqualsScene(SceneManager.GetActiveScene()))
            {
                isMouseOnly = true;
                break;
            }
        }

        // if any menu is open, set to mouse cursor
        if (ToggleableWindow.AnyWindowOpen() || isMouseOnly)
        {
            // set pivot to x:0, y:1
            cursorImage.rectTransform.pivot = new Vector2(0, 1);
            SetCursor(mouseCursor);
        }
        else
        {
            // set pivot to x:0.5, y:0.5
            cursorImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            SetCursor(aimCursor);
        }

        Cursor.visible = false;
        // hide cursor if gamepad was last used and it is the aim cursor
        if (CustomInputManager.LastInputWasGamepad && cursorImage.sprite == aimCursor)
        {
            cursorImage.enabled = false;

            // set cursor pos to center of screen
            Vector2 mousePos = new Vector2(Screen.width / 2, Screen.height / 2);
            InputState.Change(Mouse.current.position, mousePos);
        }
        else {
            
                cursorImage.enabled = true;

            if (cursorImage == null || aimCursor == null || mouseCursor == null)
            {
                Debug.LogError("CursorScript: Missing cursor image or cursor sprites");
                return;
            }

            //get mouse pos
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (CustomInputManager.GamepadCursorAllowed && CustomInputManager.LastInputWasGamepad){
                // move cursor based on gamepad input
                Vector2 move = Gamepad.current.rightStick.ReadValue();

                // if left trigger is pressed, move cursor slower
                if (Gamepad.current.leftTrigger.ReadValue() > 0.1f) {
                    move *= 0.25f;
                }

                if (move.magnitude > 0.1f) {
                    float gamepadCursorSpeed = PlayerPrefs.GetFloat("gamepadCursorSpeed", 0.25f);
                    gamepadCursorSpeed = Mathf.Lerp(5, 50, gamepadCursorSpeed);
                    mousePos.x += move.x * gamepadCursorSpeed;
                    mousePos.y += move.y * gamepadCursorSpeed;
                    Mouse.current.WarpCursorPosition(mousePos);

                    InputState.Change(Mouse.current.position, mousePos);
                }
            }

            //set cursor pos
            cursorImage.transform.position = mousePos;
        }
    }

    public void SetCursor(Sprite _cursor, float _size = 1.0f)
    {
        cursorImage.sprite = _cursor;
        cursorImage.transform.localScale = new Vector3(_size, _size, _size);
        // set native size
        cursorImage.SetNativeSize();
    }
}
