using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auto step sounds on animation events
/// </summary>
public class PlayerStepSound : MonoBehaviour
{
    public List<AudioClip> stepSounds;

    private AudioSource _audioSource;

    public GameObject _stepDecal;

    [Header("Refs")]
    public Transform _leftFoot;
    public Transform _rightFoot;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayStepSound(bool left)
    {
        if (_audioSource)
        {
            AudioClip clip = stepSounds[Random.Range(0, stepSounds.Count)];
            _audioSource.PlayOneShot(clip);
        }

        if (_stepDecal)
        {
            GameObject decal = Instantiate(_stepDecal, left ? _leftFoot.position : _rightFoot.position, Quaternion.identity);
            decal.transform.forward = transform.forward;

            // dont point upwards at all
            decal.transform.forward = Vector3.ProjectOnPlane(decal.transform.forward, Vector3.up);
        }
    }

    public void PlayStepSoundLeft()
    {
        PlayStepSound(true);
    }

    public void PlayStepSoundRight()
    {
        PlayStepSound(false);
    }
}
