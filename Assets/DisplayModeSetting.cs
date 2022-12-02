using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DisplayModeSetting : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private void Awake() {
        // get value from player prefs
        int value = PlayerPrefs.GetInt("DisplayMode", 0);
    }

    private void OnEnable() {
        // bind event
        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable() {
        // unbind event
        dropdown.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(int value) {
        // set and save value
        SetDisplayMode(value);
    }

    public void SetDisplayMode(int value)
    {
        // 0 = Borderless Windowed
        // 1 = Windowed
        // 2 = Fullscreen

        switch (value)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                // set resolution to monitor
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                // set resolution to monitor
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                break;
        }

        // save value to player prefs
        PlayerPrefs.SetInt("DisplayMode", value);
        PlayerPrefs.Save();
    }
}
