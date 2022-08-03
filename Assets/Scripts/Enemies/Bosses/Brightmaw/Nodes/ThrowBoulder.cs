using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.AI;

/// <summary>
/// The first boss in the game.
/// </summary>
namespace Boss.Brightmaw
{
    /// <summary>
    ///  A node for the Brightmaw bosses boulder throw attack.
    /// </summary>
    public class ThrowBoulder : NodeAI.ActionBase
    {
        GameObject[] boulders;
        GameObject closest;
        bool initialised = false;
        bool reachedBoulder = false;
        RotateTowards rotateTowards;
        NavMeshAgent navAgent;
        Animator animator;
        float throwTimer = 1.05f;
        float lastTime = 0;
        public ThrowBoulder()
        {
            AddProperty<Transform>("Target", null);
            AddProperty<string>("Boulder Tag", "");
            AddProperty<float>("Range", 100f);
            AddProperty<AudioClip>("Sound", null);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (!initialised)
            {
                rotateTowards = agent.GetComponent<RotateTowards>();
                navAgent = agent.GetComponent<NavMeshAgent>();
                animator = agent.GetComponent<Animator>();
                if(navAgent == null)
                {
                    Debug.LogError("ThrowBoulder: NavMeshAgent not found on agent");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                if (rotateTowards == null)
                {
                    Debug.LogError("ThrowBoulder: RotateTowards not found on agent");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                if (animator == null)
                {
                    Debug.LogError("ThrowBoulder: Animator not found on agent");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                boulders = GameObject.FindGameObjectsWithTag(GetProperty<string>("Boulder Tag"));
                initialised = true;
            
                if (boulders.Length <= 0)
                {
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                if (closest == null)
                {
                    closest = boulders[0];
                }
                foreach (var boulder in boulders)
                {
                    var distance = Vector3.Distance(agent.transform.position, boulder.transform.position);
                    if (distance < Vector3.Distance(agent.transform.position, closest.transform.position))
                    {
                        closest = boulder;
                    }
                }
            }
            Vector3 moveToVector =(closest.transform.position - GetProperty<Transform>("Target").position);
            moveToVector.y = closest.transform.position.y;
            moveToVector.Normalize();
            moveToVector *= GetProperty<float>("Range") * 0.9f;
            moveToVector += closest.transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(moveToVector, out hit, GetProperty<float>("Range"), NavMesh.AllAreas))
            {
                moveToVector = hit.position;
            }
            if (Vector3.Distance(agent.transform.position, (moveToVector 
                    )) > 2.0f && !reachedBoulder)
            {
                navAgent.SetDestination(
                    (moveToVector )
                    );  
                navAgent.speed = 1.0f;
                navAgent.acceleration = 15.0f;
                navAgent.stoppingDistance = 0.0f;
                animator.SetBool("Running", true);
                navAgent.isStopped = false;
                state = NodeData.State.Running;
                return NodeData.State.Running;
            }
            else
            {
                if(!reachedBoulder)
                {
                    rotateTowards.RotateToObject(closest, 1.5f, 6.0f, 0.0f);
                    reachedBoulder = true;
                    animator.SetBool("Running", false);
                    animator.SetTrigger("BoulderToss");
                    agent.GetComponent<AudioSource>().PlayOneShot(GetProperty<AudioClip>("Sound"));
                    navAgent.isStopped = true;
                    lastTime = Time.time;
                    navAgent.Warp(moveToVector);
                }
                
                
                
            }
            if(reachedBoulder)
            {
                throwTimer -= Time.time - lastTime;
                lastTime = Time.time;
                if(throwTimer <= 0.0f)
                {
                    closest.GetComponent<Rigidbody>().AddForce(
                        ( GetProperty<Transform>("Target").position - closest.transform.position).normalized * 
                        40.0f, 
                        ForceMode.Impulse
                        );
                    closest.GetComponent<DamagePlayerWhenCollide>().isActive = true;
                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
                else
                {
                    state = NodeData.State.Running;
                    return NodeData.State.Running;
                }
            }
            return NodeData.State.Running;
        }

        public override void OnInit()
        {
            initialised = false;
            reachedBoulder = false;
            throwTimer = 1.05f;
        }
    }
}

