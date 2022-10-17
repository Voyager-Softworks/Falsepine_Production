using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used on sound prefabs to automatically destroy after the sound plays
/// </summary>
public class AutoSound : MonoBehaviour
{
    public bool playOnStart = true;
    public bool destroyOnEnd = true;
    private bool hasPlayed = false;
    public float minVolume = 0.9f;
    public float maxVolume = 1.0f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    public AudioClip[] clips;

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        hasPlayed = false;

        audioSource = GetComponent<AudioSource>();
        // if no audio source is found, create one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (playOnStart)
        {
            PlayRandomClip();
        }
    }

    /// <summary>
    /// Plays one of the clips randomly
    /// </summary>
    private void PlayRandomClip()
    {
        hasPlayed = true;

        audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        audioSource.volume = UnityEngine.Random.Range(minVolume, maxVolume);
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource) return;

        // destroy this object after sound stops
        if (destroyOnEnd && hasPlayed && !audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
