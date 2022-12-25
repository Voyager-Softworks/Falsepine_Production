using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DisplayModeSetting : MonoBehaviour
{
    public TMP_Dropdown modeDropdown;
    public Slider fpsSlider;
    public TextMeshProUGUI fpsText;
    public TMP_Dropdown qualityDropdown;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        // get value from player prefs
        int value = PlayerPrefs.GetInt("DisplayMode", 0);
        // set value
        modeDropdown.value = value;
        // set display mode
        SetDisplayMode(value);

        // get value from player prefs
        int quality = PlayerPrefs.GetInt("Quality", 0);
        // set value
        qualityDropdown.value = quality;
        // set quality
        QualitySettings.SetQualityLevel(quality);


        // set fps
        fpsSlider.value = PlayerPrefs.GetInt("FPS", 60);
        fpsText.text = "FPS: " + fpsSlider.value.ToString();
        SetFPS((int)fpsSlider.value);
    }

    private void OnEnable() {
        // bind event
        modeDropdown.onValueChanged.AddListener(OnModeDropdownChanged);
        fpsSlider.onValueChanged.AddListener(OnFPSSliderChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityDropdownChanged);
    }

    private void OnDisable() {
        // unbind event
        modeDropdown.onValueChanged.RemoveListener(OnModeDropdownChanged);
        fpsSlider.onValueChanged.RemoveListener(OnFPSSliderChanged);
        qualityDropdown.onValueChanged.RemoveListener(OnQualityDropdownChanged);
    }

    private void OnModeDropdownChanged(int value) {
        // set and save value
        SetDisplayMode(value);
    }

    private void OnFPSSliderChanged(float value) {
        // set and save value
        SetFPS((int)value);
    }

    
    private void OnQualityDropdownChanged(int arg0)
    {
        // set quality
        QualitySettings.SetQualityLevel(arg0);
        // save value to player prefs
        PlayerPrefs.SetInt("Quality", arg0);
        PlayerPrefs.Save();
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
