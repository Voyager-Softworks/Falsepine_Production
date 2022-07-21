using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Script to play footstep sounds when an agent is moving.
/// </summary>
/// <remarks>
/// This script is attached to the agent's root.
/// It is used by adding animator events into the agents animations to trigger the PlayFootstepSound() method when their feet touch the ground in the animation.
/// </remarks>
public class FootstepSound : MonoBehaviour
{
    public AudioClip[] footstepSounds1, footstepSounds2; ///< The footstep sounds for the first and second phases of the boss fight.
    public AudioSource audioSource; ///< The audio source for the footstep sounds.
    /// <summary>
    ///  Play a footstep sound.
    /// </summary>
    public void PlayFootstepSound()
    {
        if (GetComponentInParent<NodeAI.NodeAI_Agent>().GetParameter<bool>("SecondPhase"))
        {
            if(audioSource) audioSource.PlayOneShot(footstepSounds2[Random.Range(0, footstepSounds2.Length)]);
            else GetComponent<AudioSource>().PlayOneShot(footstepSounds2[Random.Range(0, footstepSounds2.Length)]);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(footstepSounds1[Random.Range(0, footstepSounds1.Length)]);
        }
    }
}
