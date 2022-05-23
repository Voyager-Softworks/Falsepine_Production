using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.AI;

public class Attack : NodeAI.ActionBase
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
                    agent.GetComponent<DamageDealer>().MeleeAttack(phase.attackDamage, phase.attackDelay, phase.attackDuration);
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
                    agent.GetComponent<DamageDealer>().AOEAttack(phase.attackDamage, phase.attackDelay, phase.attackDuration, phase.AOEeffect);
                }
                rotateTowardsPlayer.RotateToPlayer(phase.turnDuration, phase.turnSpeed, phase.turnDelay);
                rotateTowardsPlayer.MoveToPlayer(phase.translationDuration, phase.translationSpeed, phase.translationDelay);
            }
            navAgent.isStopped = true;
            
        }

        if(GetProperty<bool>("Interrupted"))
        {
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
        }
        timeSinceInitialized += Time.deltaTime;
        
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && timeSinceInitialized >= 1.0f)
        {
            animator.ResetTrigger(GetProperty<string>("AttackName"));
            state = NodeData.State.Success;
            navAgent.isStopped = false;
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
