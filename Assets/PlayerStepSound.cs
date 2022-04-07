using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepSound : MonoBehaviour
{
    public List<AudioClip> stepSounds;

    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayStepSound()
    {
        if (_audioSource)
        {
            AudioClip clip = stepSounds[Random.Range(0, stepSounds.Count)];
            _audioSource.PlayOneShot(clip);
        }
    }
}
