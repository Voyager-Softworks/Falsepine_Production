using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioClip[] footstepSounds1, footstepSounds2;
    public void PlayFootstepSound()
    {
        if (GetComponentInParent<NodeAI.NodeAI_Agent>().GetBool("SecondPhase"))
        {
            GetComponent<AudioSource>().PlayOneShot(footstepSounds2[Random.Range(0, footstepSounds2.Length)]);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(footstepSounds1[Random.Range(0, footstepSounds1.Length)]);
        }
    }
}
