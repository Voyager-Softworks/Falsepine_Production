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
    float lastTime = 0f;

    /* This is the constructor for the Attack class. It is adding properties to the Attack class. */
    public Attack()
    {
        AddProperty<AttackData>("Attack Data", null);
        AddProperty<Transform>("Projectile spawn bone", null);
        AddProperty<bool>("Interrupted", false);
    }
    /// <summary>
    /// > This function is called when the state is first initialized
    /// </summary>
    public override void OnInit()
    {
        initialized = false;
        SetProperty<bool>("Interrupted", false);
        timeSinceInitialized = 0;
        attackData = GetProperty<AttackData>("Attack Data");
    }
    /// <summary>
    /// The function first checks if the animator, navAgent, rotateTowardsPlayer and audioSource are all
    /// set. If not, it will return a failure. If they are set, it will then check if the node has been
    /// initialized. If not, it will initialize the node by setting the animation trigger, playing the
    /// attack sound, setting the navAgent to stop, and then looping through all the attack phases and
    /// calling the appropriate functions. If the node has been initialized, it will check if the
    /// animation has finished playing and if the time since the node was initialized is greater than
    /// 0.3 seconds. If both of these conditions are met, it will reset the animation trigger, set the
    /// state to success and return success. If the animation has not finished playing, it will set the
    /// state to running and return running
    /// </summary>
    /// <param name="NodeAI_Agent">The agent that is running the tree.</param>
    /// <param name="current">The current leaf that is being evaluated.</param>
    /// <returns>
    /// The state of the node.
    /// </returns>
    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        if (animator == null)
        {
            animator = agent.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator found on " + agent.gameObject.name);
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
        if (navAgent == null)
        {
            navAgent = agent.GetComponentInChildren<NavMeshAgent>();
        }
        if (rotateTowardsPlayer == null)
        {
            rotateTowardsPlayer = agent.GetComponentInChildren<RotateTowardsPlayer>();
            if (rotateTowardsPlayer == null)
            {
                Debug.LogError("No RotateTowardsPlayer found on " + agent.gameObject.name);
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
        if (audioSource == null)
        {
            audioSource = agent.GetComponentInChildren<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("No AudioSource found on " + agent.gameObject.name);
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }

        if (!initialized)
        {
            initialized = true;
            lastTime = Time.time;

            float mult = Random.Range(0.9f, 1.0f);
            float origAnimSpeed = animator.speed;
            animator.speed = mult;
            animator.SetTrigger(attackData.animationTrigger);
            audioSource.pitch = mult;
            audioSource.PlayOneShot(attackData.attackSound);
            foreach (AttackData.AttackPhase phase in attackData.attackPhases)
            {
                if (phase.attackType == AttackData.AttackType.Melee)
                {
                    agent.GetComponent<DamageDealer>().MeleeAttack(phase.attackDamage, phase.attackDelay, phase.attackDuration, phase.attackStunDuration);
                }
                else if (phase.attackType == AttackData.AttackType.Ranged)
                {
                    if (!phase.projectileContinuousFire)
                    {
                        agent.GetComponent<DamageDealer>().RangedAttack(phase.projectile, phase.attackDelay, phase.projectileSpeed, GetProperty<Transform>("Projectile spawn bone"), true);
                    }
                    else
                    {
                        agent.GetComponent<DamageDealer>().RangedAttack(phase.projectile, phase.attackDelay, phase.projectileSpeed, GetProperty<Transform>("Projectile spawn bone"), phase.attackDuration, phase.projectileFireDelay);
                    }
                }
                else if (phase.attackType == AttackData.AttackType.AOE)
                {
                    agent.GetComponent<DamageDealer>().AOEAttack(phase.attackDamage, phase.attackDelay, phase.attackRange, phase.AOEeffect, phase.AOEspawnOffset, phase.attackStunDuration);
                    agent.GetComponent<DamageDealer>().DisplayIndicator(phase.attackDelay, phase.attackRange, phase.AOEspawnOffset, rotateTowardsPlayer.GetPlayerDir, phase.translationSpeed, phase.translationDuration, phase.AOEindicatorColor, phase.AOEindicatorDuration);
                }
                rotateTowardsPlayer.RotateToPlayer(phase.turnDuration, phase.turnSpeed, phase.turnDelay);
                rotateTowardsPlayer.MoveToPlayer(phase.translationDuration, phase.translationSpeed, phase.translationDelay);
                agent.GetComponent<DamageDealer>().Screenshake(phase.screenShakeDuration, phase.screenShakeAmplitude, phase.screenShakeFrequency, phase.screenShakeDelay);
            }
            if (navAgent) navAgent.isStopped = true;
            if (navAgent) navAgent.SetDestination(agent.transform.position);

        }

        if (GetProperty<bool>("Interrupted"))
        {
            animator.speed = 1f;
            audioSource.pitch = 1f;
            agent.GetComponent<DamageDealer>().CancelAttack();
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
        }
        timeSinceInitialized += Time.time - lastTime;
        lastTime = Time.time;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95 && timeSinceInitialized >= attackData.attackPhases[0].attackDelay + attackData.attackPhases[0].attackDuration)
        {
            animator.speed = 1f;
            audioSource.pitch = 1f;
            animator.ResetTrigger(attackData.animationTrigger);
            audioSource.Stop();
            state = NodeData.State.Success;
            //navAgent.isStopped = false;
            return NodeData.State.Success;
        }
        else
        {
            state = NodeData.State.Running;
            if (navAgent) navAgent.isStopped = true;
            return NodeData.State.Running;
        }



    }
}
