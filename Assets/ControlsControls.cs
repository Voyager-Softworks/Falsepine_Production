using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ControlsControls : MonoBehaviour
{
    public Toggle controllerToggle;
    public Slider gamepadCursorSpeedSlider;

    public void OnControllerToggleChanged(bool value) {
        CustomInputManager.SetControllerInputAllowed(value);
    }

    public void OnGamepadCursorSpeedSliderChanged(float value) {
        PlayerPrefs.SetFloat("gamepadCursorSpeed", value);
    }

    private void OnEnable() {
        controllerToggle.onValueChanged.AddListener(OnControllerToggleChanged);
        gamepadCursorSpeedSlider.onValueChanged.AddListener(OnGamepadCursorSpeedSliderChanged);

        controllerToggle.isOn = CustomInputManager.GamepadCursorAllowed;
        gamepadCursorSpeedSlider.value = PlayerPrefs.GetFloat("gamepadCursorSpeed", 0.25f);
    }

    private void OnDisable() {
        controllerToggle.onValueChanged.RemoveListener(OnControllerToggleChanged);
        gamepadCursorSpeedSlider.onValueChanged.RemoveListener(OnGamepadCursorSpeedSliderChanged);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
