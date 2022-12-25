using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DisplayModeSetting : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Slider fpsSlider;
    public TextMeshProUGUI fpsText;

    private void Start() {
        // get value from player prefs
        int value = PlayerPrefs.GetInt("DisplayMode", 0);
        // set value
        dropdown.value = value;
        // set display mode
        SetDisplayMode(value);

        // set fps
        fpsSlider.value = PlayerPrefs.GetInt("FPS", 60);
        fpsText.text = "FPS: " + fpsSlider.value.ToString();
        SetFPS((int)fpsSlider.value);
    }

    private void OnEnable() {
        // bind event
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
        fpsSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable() {
        // unbind event
        dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        fpsSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnDropdownChanged(int value) {
        // set and save value
        SetDisplayMode(value);
    }

    private void OnSliderChanged(float value) {
        // set and save value
        SetFPS((int)value);
    }

    public void SetFPS(int value)
    {
        // set fps
        Application.targetFrameRate = value;
        // save value to player prefs
        PlayerPrefs.SetInt("FPS", value);
        PlayerPrefs.Save();

        fpsText.text = "FPS: " + value.ToString();
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
