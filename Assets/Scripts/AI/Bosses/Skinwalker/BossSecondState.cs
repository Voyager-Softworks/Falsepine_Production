using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSecondState : MonoBehaviour
{
    bool isSecondState = false;
    public AudioClip transformSoundClip;
    public void OnSecondStateEnter()
    {
        if(!isSecondState)
        {
            isSecondState = true;
            Debug.Log("Second State");
            GetComponent<NodeAI.NodeAI_Agent>().SetBool("SecondPhase", true);
            GetComponentInChildren<Animator>().SetTrigger("SecondPhaseTransform");
            GetComponentInChildren<Animator>().SetBool("Charging", false);
            GetComponent<NodeAI.NodeAI_Agent>().SetState("SecondPhaseTransform");
            GetComponent<AudioSource>().PlayOneShot(transformSoundClip);
            
        }
        
        
        
    }
}
