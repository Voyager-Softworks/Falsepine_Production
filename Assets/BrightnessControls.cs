using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class BrightnessControls : MonoBehaviour
{
    public List<VolumeProfile> volumeProfiles;

    public float minBrightness = -2f;
    public float maxBrightness = 2f;

    public float currentBrightness = 0f;

    public Slider brightnessSlider;



    public void SetBrightness(float brightness)
    {
        currentBrightness = brightness;
        PlayerPrefs.SetFloat("Brightness", brightness);
        foreach (VolumeProfile volumeProfile in volumeProfiles)
        {
            ColorAdjustments colorAdjustments;
            if (volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments))
                colorAdjustments.postExposure.value = brightness;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Brightness"))
        {
            currentBrightness = PlayerPrefs.GetFloat("Brightness");
            brightnessSlider.value = currentBrightness;
            SetBrightness(currentBrightness);
        }
        brightnessSlider.onValueChanged.AddListener(delegate { SetBrightness(brightnessSlider.value); });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
