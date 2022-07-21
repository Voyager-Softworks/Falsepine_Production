using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
///  Script that handles the blending of animations and the navigation agent.
/// </summary>
public class NavagentBlendtreeCoupler : MonoBehaviour
{
    public Animator animator; ///< The animator of the character.
    public NavMeshAgent navAgent; ///< The nav mesh agent of the character.
    public bool hasForwardSpeed; ///< Whether or not the character has a forward speed.
    public string forwardVelocityParameter = ""; ///< The name of the forward velocity parameter.


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(navAgent.isOnNavMesh)
        {
            if(hasForwardSpeed)
            {
                animator.SetFloat(forwardVelocityParameter, navAgent.velocity.magnitude / navAgent.speed);
            }
        }
    }
}
