using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioControls : MonoBehaviour
{
    public AudioMixer audioMixer;
    const string masterVolume = "MasterVol";
    const string musicVolume = "MusicVol";
    const string sfxVolume = "SFXVol";
    const string ambienceVolume = "AmbienceVol";

    public float decibelUpperLimit = 0;
    public float decibelLowerLimit = -80;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider ambienceSlider;

    public float FromNormalisedToDecibel(float normalisedValue)
    {
        float decibel = 20.0f * Mathf.Log10(normalisedValue);
        return Mathf.Clamp(decibel, decibelLowerLimit, decibelUpperLimit);
    }

    public float FromDecibelToNormalised(float decibelValue)
    {
        float linear = Mathf.Pow(10.0f, decibelValue / 20.0f);
        return Mathf.Clamp(linear, 0.0f, 1.0f);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(masterVolume, FromNormalisedToDecibel(volume));
        PlayerPrefs.SetFloat(masterVolume, volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(musicVolume, FromNormalisedToDecibel(volume));
        PlayerPrefs.SetFloat(musicVolume, volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(sfxVolume, FromNormalisedToDecibel(volume));
        PlayerPrefs.SetFloat(sfxVolume, volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        audioMixer.SetFloat(ambienceVolume, FromNormalisedToDecibel(volume));
        PlayerPrefs.SetFloat(ambienceVolume, volume);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (masterSlider)
        {
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            masterSlider.value = PlayerPrefs.GetFloat(masterVolume, 1);
        }
        if (musicSlider)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            musicSlider.value = PlayerPrefs.GetFloat(musicVolume, 1);
        }
        if (sfxSlider)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            sfxSlider.value = PlayerPrefs.GetFloat(sfxVolume, 1);
        }
        if (ambienceSlider)
        {
            ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
            ambienceSlider.value = PlayerPrefs.GetFloat(ambienceVolume, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
