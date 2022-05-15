using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if false // Old code, not used anymore but kept for reference
public class BossSecondState : MonoBehaviour
{
    bool isSecondState = false;
    public AudioClip transformSoundClip;
    public GameObject particle;
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
            GetComponent<NavMeshAgent>().isStopped = true;
            Destroy(Instantiate(particle, transform.position, Quaternion.identity), 10.0f);
            
        }
        
        
        
    }
}
#endif