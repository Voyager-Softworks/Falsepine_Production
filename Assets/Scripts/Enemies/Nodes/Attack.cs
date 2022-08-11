using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.AI;

/// <summary>
///  A Node for implementing enemy Attacks.
/// </summary>
/// <remarks>
/// This node includes functionality for:
///     - Melee Attacks
///         - Hurtboxes
///         - Variable Hurtbox Collision Check Duration
///     - Ranged Attacks
///         - Projectiles
///         - Single Shot Projectiles
///         - Continuous Fire Projectiles
///         - Specifiable Projectile Spawn Bone
///     - AOE Attacks
///         - Damage dealt within a radius
///         - Spawns an AOE particle effect provided
///     - Rotation and Translation
///         - Rotation to target\n
///           Specify a speed, delay, and duration that is used to rotate towards the target as part of the attack.
///         - Translation to target\n
///           Specify a speed, delay, and duration that is used to move towards the target as part of the attack.
/// </remarks>
/// @bug
///     Issue where an attack animation MUST fully play or the attack will not register as complete. 
///     - It cannot transition before it is at least 90% complete.
public class Attack : NodeAI.ActionBase  /// @todo Comment
{
    Animator animator;
    NavMeshAgent navAgent;
    RotateTowardsPlayer rotateTowardsPlayer;
    AudioSource audioSource;
    AttackData attackData;
    bool initialized = false;
    float timeSinceInitialized = 0;

    public Attack()
    {
        AddProperty<AttackData>("Attack Data", null);
        AddProperty<Transform>("Projectile spawn bone", null);
        AddProperty<bool>("Interrupted", false);
    }
    public override void OnInit()
    {
        initialized = false;
        SetProperty<bool>("Interrupted", false);
        timeSinceInitialized = 0;
        attackData = GetProperty<AttackData>("Attack Data");
    }
    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        if(animator == null)
        {
            animator = agent.GetComponentInChildren<Animator>();
            if(animator == null)
            {
                Debug.LogError("No Animator found on " + agent.gameObject.name);
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
        if(navAgent == null)
        {
            navAgent = agent.GetComponentInChildren<NavMeshAgent>();
            if(navAgent == null)
            {
                Debug.LogError("No NavMeshAgent found on " + agent.gameObject.name);
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
        if(rotateTowardsPlayer == null)
        {
            rotateTowardsPlayer = agent.GetComponentInChildren<RotateTowardsPlayer>();
            if(rotateTowardsPlayer == null)
            {
                Debug.LogError("No RotateTowardsPlayer found on " + agent.gameObject.name);
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
        if(audioSource == null)
        {
            audioSource = agent.GetComponentInChildren<AudioSource>();
            if(audioSource == null)
            {
                Debug.LogError("No AudioSource found on " + agent.gameObject.name);
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }

        if(!initialized)
        {
            initialized = true;
            animator.SetTrigger(attackData.animationTrigger);
            audioSource.PlayOneShot(attackData.attackSound);
            foreach(AttackData.AttackPhase phase in attackData.attackPhases)
            {
                if(phase.attackType == AttackData.AttackType.Melee)
                {
                    agent.GetComponent<DamageDealer>().MeleeAttack(phase.attackDamage, phase.attackDelay, phase.attackDuration, phase.attackStunDuration);
                }
                else if(phase.attackType == AttackData.AttackType.Ranged)
                {
                    if(!phase.projectileContinuousFire)
                    {
                        agent.GetComponent<DamageDealer>().RangedAttack(phase.projectile, phase.attackDelay, phase.projectileSpeed, GetProperty<Transform>("Projectile spawn bone"), true);
                    }
                    else
                    {
                        agent.GetComponent<DamageDealer>().RangedAttack(phase.projectile, phase.attackDelay, phase.projectileSpeed, GetProperty<Transform>("Projectile spawn bone"), phase.attackDuration, phase.projectileFireDelay);
                    }
                }
                else if(phase.attackType == AttackData.AttackType.AOE)
                {
                    agent.GetComponent<DamageDealer>().AOEAttack(phase.attackDamage, phase.attackDelay, phase.attackRange, phase.AOEeffect, phase.AOEspawnOffset, phase.attackStunDuration);
                    agent.GetComponent<DamageDealer>().DisplayIndicator(phase.attackDelay, phase.attackRange, phase.AOEspawnOffset, rotateTowardsPlayer.GetPlayerDir, phase.translationSpeed, phase.translationDuration, phase.AOEindicatorColor, phase.AOEindicatorDuration);
                }
                rotateTowardsPlayer.RotateToPlayer(phase.turnDuration, phase.turnSpeed, phase.turnDelay);
                rotateTowardsPlayer.MoveToPlayer(phase.translationDuration, phase.translationSpeed, phase.translationDelay);
            }
            navAgent.isStopped = true;
            navAgent.SetDestination(agent.transform.position);
            
        }

        if(GetProperty<bool>("Interrupted"))
        {
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
        }
        timeSinceInitialized += Time.deltaTime;
        
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9 && timeSinceInitialized >= 0.3f)
        {
            animator.ResetTrigger(attackData.animationTrigger);
            state = NodeData.State.Success;
            //navAgent.isStopped = false;
            return NodeData.State.Success;
        }
        else
        {
            state = NodeData.State.Running;
            navAgent.isStopped = true;
            return NodeData.State.Running;
        }


        
    }
}
