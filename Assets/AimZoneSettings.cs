using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AimZoneSettings : MonoBehaviour
{
    public Image exampleImage;
    public Slider red_slider;
    public Slider green_slider;
    public Slider blue_slider;
    public Slider alpha_slider;

    [ReadOnly] public AimZone aimZone;

    private TextMeshProUGUI red_text;
    private TextMeshProUGUI green_text;
    private TextMeshProUGUI blue_text;
    private TextMeshProUGUI alpha_text;

    // Start is called before the first frame update
    void Start()
    {
        // find and set AimZone values
        aimZone = FindObjectOfType<AimZone>(true);
        UpdateColor();

        // find text
        red_text = red_slider.GetComponentInChildren<TextMeshProUGUI>();
        green_text = green_slider.GetComponentInChildren<TextMeshProUGUI>();
        blue_text = blue_slider.GetComponentInChildren<TextMeshProUGUI>();
        alpha_text = alpha_slider.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update() {
        // update text
        red_text.text = (red_slider.value * 255.0f).ToString("F0");
        green_text.text = (green_slider.value * 255.0f).ToString("F0");
        blue_text.text = (blue_slider.value * 255.0f).ToString("F0");
        alpha_text.text = (alpha_slider.value * 255.0f).ToString("F0");

        // update image
        exampleImage.color = new Color(red_slider.value, green_slider.value, blue_slider.value, alpha_slider.value);
    }

    private void OnEnable() {
        // bind slider events to OnValueChanged
        red_slider.onValueChanged.AddListener(delegate { OnRedChanged(); });
        green_slider.onValueChanged.AddListener(delegate { OnGreenChanged(); });
        blue_slider.onValueChanged.AddListener(delegate { OnBlueChanged(); });
        alpha_slider.onValueChanged.AddListener(delegate { OnAlphaChanged(); });
    }

    private void OnDisable() {
        // unbind slider events to OnValueChanged
        red_slider.onValueChanged.RemoveAllListeners();
        green_slider.onValueChanged.RemoveAllListeners();
        blue_slider.onValueChanged.RemoveAllListeners();
        alpha_slider.onValueChanged.RemoveAllListeners();
    }

    private void UpdateColor()
    {
        if (aimZone != null)
        {
            aimZone.SetColorFromSettings(new Color(red_slider.value, green_slider.value, blue_slider.value, alpha_slider.value));
        }
    }

    public void SetSliders(Color color)
    {
        red_slider.value = color.r;
        green_slider.value = color.g;
        blue_slider.value = color.b;
        alpha_slider.value = color.a;
    }

    public void OnRedChanged(){
        UpdateColor();
    }
    public void OnGreenChanged(){
        UpdateColor();
    }
    public void OnBlueChanged(){
        UpdateColor();
    }
    public void OnAlphaChanged(){
        UpdateColor();
    }
}
