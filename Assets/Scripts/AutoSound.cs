using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used on sound prefabs to automatically destroy after the sound plays
/// </summary>
public class AutoSound : MonoBehaviour  /// @todo comment
{
    public bool playOnStart = true;
    public bool destroyOnEnd = true;
    private bool hasPlayed = false;

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

    private void PlayRandomClip()
    {
        hasPlayed = true;

        audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
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
