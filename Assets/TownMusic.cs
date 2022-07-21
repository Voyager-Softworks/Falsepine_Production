using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TownMusic : MonoBehaviour  /// @todo Comment
{
    [Serializable]
    public class MusicBuildingLink
    {
        public TownBuilding building;
        public AudioClip startClip;
        public AudioClip music;
    }
    public List<MusicBuildingLink> musicBuildingLinks = new List<MusicBuildingLink>();

    public AudioClip defaultMusic;

    public AudioSource audioSource;
    public float maxVolume = 1;

    private AudioClip nextMusic;
    public float fadeTime = 1.0f;
    private float fadeTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = defaultMusic;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeTimer > 0.0f)
        {
            fadeTimer -= Time.deltaTime;
            audioSource.volume = (fadeTimer / fadeTime) * maxVolume;
            if (fadeTimer <= 0.0f)
            {
                audioSource.Stop();
                audioSource.clip = nextMusic;
                audioSource.Play();
                fadeTimer = 0.0f;
            }
        }
        else{
            audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolume, Time.deltaTime);
        }

        // if any of the building UI's are active, play the music, otherwise play the default music
        bool anyBuildingUIActive = false;
        foreach (MusicBuildingLink link in musicBuildingLinks)
        {
            if (link.building.UI.activeSelf)
            {
                anyBuildingUIActive = true;
                if (audioSource.clip != link.music && nextMusic != link.music){
                    audioSource.PlayOneShot(link.startClip);
                    FadeTo(link.music);
                }
                break;
            }
        }

        if (!anyBuildingUIActive && audioSource.clip != defaultMusic && nextMusic != defaultMusic)
        {
            FadeTo(defaultMusic);
        }
        
    }

    public void FadeTo(AudioClip clip)
    {
        nextMusic = clip;
        fadeTimer = fadeTime;
    }
}