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

    private bool m_makeDecal = false;
    private bool m_leftDecal = false;

    public Vector3 m_decalOffset = new Vector3(0f, 0f, 0f);

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
        // do the function here, rather than in the animation event, as position is not accurate in the animation event
        if (m_makeDecal)
        {
            MakeDecal();
        }
    }

    private void MakeDecal()
    {
        if (m_makeDecal == false) return;
        m_makeDecal = false;

        // get the position of the correct foot
        Vector3 footPos = m_leftDecal ? _leftFoot.position : _rightFoot.position;

        // spawn and position decal
        GameObject decal = Instantiate(_stepDecal, footPos, Quaternion.identity);
        decal.transform.forward = transform.forward;

        // dont point upwards at all
        decal.transform.forward = Vector3.ProjectOnPlane(decal.transform.forward, Vector3.up);

        // reset position
        decal.transform.position = footPos;
        // add offset
        decal.transform.position += decal.transform.forward * m_decalOffset.z;
        decal.transform.position += decal.transform.right * m_decalOffset.x;
        decal.transform.position += decal.transform.up * m_decalOffset.y;

        Footprint footprint = decal.GetComponent<Footprint>();
        footprint.m_left = m_leftDecal;
    }

    public void PlayStepSound(bool left)
    {
        // play sound
        if (_audioSource)
        {
            AudioClip clip = stepSounds[Random.Range(0, stepSounds.Count)];
            _audioSource.PlayOneShot(clip);
        }

        // mark to make decal
        if (_stepDecal)
        {
            m_makeDecal = true;
            m_leftDecal = left;
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
