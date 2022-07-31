using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script integrating root motion movement animation with navmesh agent AI navigation.
/// </summary>
/// <remarks>
/// This script is meant to be used in conjunction with the <see cref="NavMeshAgent"/> component.
/// It uses the NavMeshAgent component to control rotation and velocity by rotating the agent and controlling the speed of the root motion animation.
/// </remarks>
/// <seealso cref="NavMeshAgent"/>
/// <seealso cref="Animator"/>
/// @todo
/// <list type="bullet">
/// <item>
/// Add in functionality to control navigation through this script.
/// </item>
/// <item>
/// Make this script into a wrapper for the NavMeshAgent for use with root motioned characters.
/// </item>
/// </list>
public class RootMotionAgent : MonoBehaviour
{
    public NavMeshAgent navAgent; ///< The nav mesh agent.
    public Animator animator; ///< The animator.
    public string speedParam = "Speed"; ///< The speed parameter.
    public string angularSpeedParam = "AngularSpeed"; ///< The angular speed parameter.
    public bool useRootMotion = true; ///< Whether to use root motion.
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        navAgent.updatePosition = true;
        navAgent.updateRotation = true;
        var localVelocity = transform.InverseTransformDirection(navAgent.velocity);
        animator.SetFloat(speedParam, localVelocity.z);
        animator.SetFloat(angularSpeedParam, localVelocity.x);
        float direction = Vector3.Angle(transform.forward, navAgent.desiredVelocity) * Mathf.Sign(Vector3.Dot(navAgent.desiredVelocity, transform.right)); 

    }

    float GetTurnDirection(Vector3 currentVelocty, Vector3 targetVelocity)
    {
        Vector3 forward = transform.right;
        Vector3 targetForward = targetVelocity.normalized;
        float dot = Vector3.Dot(forward, targetForward);
        return dot;
    }

    void OnAnimatorMove() {
        Vector3 position = animator.rootPosition;
        position.y = navAgent.nextPosition.y;
        transform.position = position;
        navAgent.nextPosition = transform.position;
    }
}
