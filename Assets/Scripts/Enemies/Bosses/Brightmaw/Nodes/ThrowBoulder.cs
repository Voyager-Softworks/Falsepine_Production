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
        float throwTimer = 1.25f;
        float lastTime = 0;
        /// <summary>
        /// Constructor for the node.
        /// </summary>
        public ThrowBoulder()
        {
            AddProperty<Transform>("Target", null);
            AddProperty<Transform>("Boulder Holder Bone", null);
            AddProperty<string>("Boulder Tag", "");
            AddProperty<float>("Range", 100f);
            AddProperty<AudioClip>("Sound", null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IEnumerator PullBoulderToHand(float duration, float delay)
        {
            yield return new WaitForSeconds(delay);
            //Lerp the boulder towards the hand
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                closest.transform.position = Vector3.Lerp(closest.transform.position, GetProperty<Transform>("Boulder Holder Bone").position, t / duration);
                yield return null;
            }
        }

        /// <summary>
        /// The agent moves to the closest boulder, then pulls it to their hand, then throws it at the
        /// player
        /// </summary>
        /// <param name="NodeAI_Agent">The agent that is running the tree.</param>
        /// <param name="current">The current leaf in the tree</param>
        /// <returns>
        /// The state of the node.
        /// </returns>
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (!initialised)
            {
                rotateTowards = agent.GetComponent<RotateTowards>();
                navAgent = agent.GetComponent<NavMeshAgent>();
                animator = agent.GetComponent<Animator>();
                if (navAgent == null)
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
            Vector3 moveToVector = (closest.transform.position - GetProperty<Transform>("Target").position);
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
                    (moveToVector)
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
                if (!reachedBoulder)
                {
                    rotateTowards.RotateToObject(closest, 1.00f, 6.0f, 0.0f);
                    agent.StartCoroutine(PullBoulderToHand(1.05f, 0.2f));
                    rotateTowards.RotateToObject(GetProperty<Transform>("Target").gameObject, 0.5f, 12.0f, 1.0f);
                    reachedBoulder = true;
                    animator.SetBool("Running", false);
                    animator.SetTrigger("BoulderToss");
                    agent.GetComponent<AudioSource>().PlayOneShot(GetProperty<AudioClip>("Sound"));
                    navAgent.isStopped = true;
                    lastTime = Time.time;
                    navAgent.Warp(moveToVector);
                }



            }
            if (reachedBoulder)
            {
                throwTimer -= Time.time - lastTime;
                lastTime = Time.time;
                if (throwTimer <= 0.0f)
                {
                    Vector3 throwVector = (GetProperty<Transform>("Target").position - closest.transform.position);
                    throwVector.y += 0.5f;
                    closest.GetComponent<Rigidbody>().AddForce(
                        (throwVector).normalized *
                        900.0f,
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
            throwTimer = 1.25f;
        }
    }
}

