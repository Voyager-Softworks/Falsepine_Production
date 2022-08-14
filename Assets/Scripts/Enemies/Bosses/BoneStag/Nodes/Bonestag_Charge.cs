using System.Collections;
using System.Collections.Generic;
using NodeAI;
using UnityEngine;
using UnityEngine.AI;

namespace Boss.Bonestag
{
    /// <summary>
    ///  Node that implements the charging phase of the Bonestag AI
    /// </summary>
    public class Bonestag_Charge : NodeAI.ActionBase
    {
        bool initialized = false; ///< Whether or not the node has been initialized.
        Animator animator; ///< The animator of the character.
        NavMeshAgent navAgent; ///< The nav mesh agent of the character.
        Vector3 target; ///< The target position of the character.
        Transform agentTransform; ///< The transform of the character.
        GameObject agentGameObject; ///< The game object of the character.
        Transform targetTransform; ///< The transform of the target.
        BossArenaController arenaController; ///< The arena controller of the character.
        Vector3 arenaEdgeGoalPosition; ///< The position of the arena edge goal.
        public float chargeSpeed = 50f; ///< The speed of the character when charging.
        public float chargeDamage = 10f; ///< The damage of the character when charging.
        public GameObject debrisPrefab; ///< The prefab of the debris to spawn.
        bool hasDamagedPlayer = false; ///< Whether or not the character has damaged the player.

        public AudioClip chargeSoundPhaseOne, chargeSoundPhaseTwo; ///< The sounds to play when charging.
        public Bonestag_Charge()
        {
            AddProperty<bool>("InBearTrap", false);
            AddProperty<float>("ChargeSpeed", 0.5f);
            AddProperty<AudioClip>("ChargeSoundPhaseOne", null);
            AddProperty<AudioClip>("ChargeSoundPhaseTwo", null);
            AddProperty<GameObject>("DebrisPrefab", null);
            initialized = false;
            tooltip = "Charge at the player";
            
        }
        public override void OnInit()
        {
            chargeSoundPhaseOne = GetProperty<AudioClip>("ChargeSoundPhaseOne");
            chargeSoundPhaseTwo = GetProperty<AudioClip>("ChargeSoundPhaseTwo");
            debrisPrefab = GetProperty<GameObject>("DebrisPrefab");
            initialized = false;
            hasDamagedPlayer = false;
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("Bonestag_Charge: No animator found on agent");
                    return NodeData.State.Failure;
                }
            }
            if (navAgent == null)
            {
                navAgent = agent.GetComponentInChildren<NavMeshAgent>();
                if (navAgent == null)
                {
                    Debug.LogError("Bonestag_Charge: No navAgent found on agent");
                    return NodeData.State.Failure;
                }
            }

            if(!initialized)
            {
                agentTransform = agent.transform;
                agentGameObject = agent.gameObject;
                
                if(arenaController == null)
                    arenaController = FindObjectOfType<BossArenaController>();
                targetTransform = arenaController.arenaCentre;
                Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
                arenaEdgeGoalPosition = ComputeB(targetTransform.position, Vector3.up, arenaController.arenaRadius, agent.transform.position, (playerPos - agentTransform.position).normalized);
                NavMeshHit goalPosHit;
                if(!NavMesh.FindClosestEdge(arenaEdgeGoalPosition, out goalPosHit, NavMesh.AllAreas))
                {
                    arenaEdgeGoalPosition.y = agentTransform.position.y;
                }
                else
                {
                    arenaEdgeGoalPosition = goalPosHit.position;
                }
                navAgent.SetDestination(arenaEdgeGoalPosition);
                
                
                if(GetProperty<bool>("SecondPhase")) {
                    agent.GetComponent<AudioSource>().PlayOneShot(chargeSoundPhaseTwo);
                    navAgent.speed = 70;
                }
                else {
                    agent.GetComponent<AudioSource>().PlayOneShot(chargeSoundPhaseOne);
                    navAgent.speed = 50;
                }

                navAgent.velocity =  (arenaEdgeGoalPosition - agent.transform.position).normalized * navAgent.speed;
                
                navAgent.acceleration = 150;
                navAgent.angularSpeed = 100000;
                
                navAgent.isStopped = false;
                initialized = true;
            }

            if(GetProperty<bool>("InBearTrap"))
            {
                navAgent.SetDestination(agent.transform.position);
                navAgent.isStopped = true;
                navAgent.velocity =  Vector3.zero;
                //animator.SetTrigger("Trapped");
                animator.SetBool("Charging", false);
                state = NodeData.State.Failure;
                return state;
            }
            else
            {
                RaycastHit[] hits = Physics.SphereCastAll(agent.transform.position, 2.0f, agent.transform.forward, 2.0f);
                foreach(RaycastHit hit in hits)
                {
                    if(hit.collider.gameObject.tag == "DestructibleProp")
                    {
                        Destroy(Instantiate(debrisPrefab, hit.point, Quaternion.identity), 5.0f);
                        Destroy(hit.collider.gameObject);
                    }
                    else if(hit.collider.gameObject.tag == "Player" && !hasDamagedPlayer)
                    {
                        hit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(chargeDamage);
                        hasDamagedPlayer = true;
                    }
                    else if(hit.collider.gameObject.tag == "BearTrap")
                    {
                        animator.ResetTrigger("AttackingFinished");
                        SetProperty<bool>("InBearTrap", true);
                        agent.SetParameter<bool>("InBearTrap", true);
                        navAgent.enabled = false;
                        agent.transform.position = hit.collider.gameObject.transform.position;
                        navAgent.enabled = true;
                        //do closing anim
                        Transform trapRoot = hit.transform.root;
                        Animator trapAnim = trapRoot.GetComponentInChildren<Animator>();
                        if (trapAnim != null)
                        {
                            trapAnim.SetTrigger("Close");
                        }
                        //disable interaction
                        Interactable interactable = trapRoot.GetComponent<Interactable>();
                        if (interactable){
                            interactable._text.enabled = false;
                            interactable.enabled = false;
                        }
                        //disable all colliders
                        Collider[] colliders = trapRoot.GetComponentsInChildren<Collider>();
                        foreach (Collider collider in colliders)
                        {
                            collider.enabled = false;
                        }
                    }
                }
                if(Vector3.Distance(agent.transform.position, arenaEdgeGoalPosition) < 3.0f)
                {
                    agent.transform.position = RandomPointOnCircle(targetTransform.position, Vector3.up, arenaController.arenaRadius);
                    agent.gameObject.GetComponent<RotateTowardsPlayer>().RotateToPlayer(0.5f, 10.0f, 0.0f);
                    navAgent.SetDestination(agent.transform.position);
                    navAgent.isStopped = true;
                    animator.SetBool("Charging", false);
                    state = NodeData.State.Success;
                    return state;
                }
                else
                {
                    navAgent.SetDestination(arenaEdgeGoalPosition);
                    navAgent.isStopped = false;
                    animator.SetBool("Charging", true);
                    state = NodeData.State.Running;
                    return state;
                }
            }
        }

        public override void DrawGizmos(NodeAI_Agent agent)
        {
            Gizmos.color = Color.red;
            if(arenaController != null) Gizmos.DrawWireSphere(arenaController.arenaCentre.position, arenaController.arenaRadius);
        }

        private Vector3 ComputeB( Vector3 circleCenter, Vector3 circleNormal, float circleRadius, Vector3 point, Vector3 direction )
        {
            float a = Vector3.SignedAngle( (circleCenter - point).normalized * circleRadius, direction, circleNormal );
            float w = 0;
            if ( a >= 0 ) w = 180 - 2 * a; // because w + a + a = 180;
            else w = -( 180 + 2 * a );
            Vector3 BO = Quaternion.AngleAxis(w, -circleNormal) * ((point - circleCenter).normalized * circleRadius);
            return circleCenter + BO;
        }

        private Vector3 RandomPointOnCircle( Vector3 circleCenter, Vector3 circleNormal, float circleRadius )
        {
            float angle = Random.Range( 0, 360 );
            Vector3 BO = Quaternion.AngleAxis( angle, -circleNormal ) * ((circleCenter - circleNormal * circleRadius).normalized * circleRadius);
            return circleCenter + BO;
        }
    }
}
