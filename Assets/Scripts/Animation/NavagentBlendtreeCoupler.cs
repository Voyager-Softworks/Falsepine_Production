using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavagentBlendtreeCoupler : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent navAgent;
    public bool hasForwardSpeed;
    public string forwardVelocityParameter = "";


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
