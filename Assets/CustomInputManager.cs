using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;
using System;

public class CustomInputManager : MonoBehaviour
{
    static public CustomInputManager instance;

    private static bool lastInputWasGamepad = false;
    public static bool LastInputWasGamepad { get { return lastInputWasGamepad; } }

    private static bool gamepadCursorAllowed = true;
    public static bool GamepadCursorAllowed { get { return gamepadCursorAllowed; } }

    static CustomInputManager()
    {
        if (instance == null)
        {
            instance = new CustomInputManager();
        }

        InputSystem.onEvent += OnInputEvent;

        // get value from prefs
        SetControllerInputAllowed(PlayerPrefs.GetInt("gamepadCursorAllowed", 1) == 1);
    }

    private static void OnInputEvent(InputEventPtr eventPtr, InputDevice device) {
        // if not play mode, ignore
        if (Application.isPlaying == false) {
            return;
        }

        if (gamepadCursorAllowed && device is Gamepad) {
            lastInputWasGamepad = true;
        } 
        else {
            // if gamepad was last input, remove currently selected object
            if (lastInputWasGamepad) {
                EventSystem.current.SetSelectedGameObject(null);
            }
            lastInputWasGamepad = false;
        }
    }

    public static void SetControllerInputAllowed(bool allowed) {
        // save to prefs
        PlayerPrefs.SetInt("gamepadCursorAllowed", allowed ? 1 : 0);

        gamepadCursorAllowed = allowed;
        if (!gamepadCursorAllowed){
            lastInputWasGamepad = false;
        }

        // enable/disable all gamepads
        Gamepad[] gamepads = Gamepad.all.ToArray();
        for (int i = 0; i < gamepads.Length; i++)
        {
            if (allowed)
            {
                InputSystem.EnableDevice(gamepads[i]);
            }
            else
            {
                InputSystem.DisableDevice(gamepads[i]);
            }
        }

        // if gamepad has been disabled, but no mouse or keyboard exists, enable gamepad
        if (!allowed && (Mouse.current == null || Keyboard.current == null)) {
            SetControllerInputAllowed(true);
        }
    }
}