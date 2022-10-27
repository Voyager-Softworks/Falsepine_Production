using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.Rendering.HighDefinition;

namespace Boss.Omen
{
    /// <summary>
    ///  Node to handle the Omen's breath attack.
    /// </summary>
    public class BreathAttack : NodeAI.ActionBase
    {
        Animator animator; // Animator
        float delayTimer = 0.0f; // Timer for the delay
        float durationTimer = 0.0f; // Timer for the duration
        float tickTimer = 0.0f; // Timer for the tick
        float lastTime = 0.0f; // Time last frame

        bool init = false; // Whether or not the node has been initialized
        bool hasDamaged = false; // Whether or not the node has damaged the player

        GameObject indicator = null; // Indicator for the attack
        DecalProjector decalProjector = null; // Decal projector for the attack
        GameObject sfx = null; // SFX for the attack
        AudioSource source = null; // Audio source for the attack
        RotateTowardsPlayer rotateTowardsPlayer = null; // Rotate towards player script
        ScreenshakeManager screenshakeManager = null; // Screenshake manager

        /// <summary>
        ///  Method to initialize the node.
        /// </summary>
        public BreathAttack()
        {
            //Add Properties to the node
            AddProperty<GameObject>("Particle Effect", null);
            AddProperty<GameObject>("Indicator", null);
            AddProperty<string>("Animation Trigger", "");
            AddProperty<AudioClip>("Sound", null);

            AddProperty<float>("Attack Start Delay", 0.0f);
            AddProperty<float>("Attack Duration", 1.0f);
            AddProperty<float>("Attack Damage", 10.0f);
            AddProperty<bool>("Continuous", false);
            AddProperty<float>("Tick Rate", 0.5f);

            AddProperty<Transform>("Mouth Bone", null);
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            float deltaTime = Time.time - lastTime; // Delta time
            lastTime = Time.time; // Set last time
            if (animator == null) animator = agent.GetComponent<Animator>(); // Get animator
            if (source == null) source = agent.GetComponent<AudioSource>(); // Get audio source
            if (rotateTowardsPlayer == null) rotateTowardsPlayer = agent.GetComponent<RotateTowardsPlayer>(); // Get rotate towards player script
            if (screenshakeManager == null) screenshakeManager = FindObjectOfType<ScreenshakeManager>(); // Get screenshake manager
            // If the node has not been initialized
            if (!init)
            {
                delayTimer = 0.0f;
                durationTimer = 0.0f;
                tickTimer = 0.0f;
                hasDamaged = false;
                animator.SetTrigger(GetProperty<string>("Animation Trigger"));
                source.PlayOneShot(GetProperty<AudioClip>("Sound"));
                rotateTowardsPlayer.RotateToPlayer(GetProperty<float>("Attack Start Delay"), 2f, 0.0f);
                init = true;
            }

            // If the delay timer is less than the delay
            if (delayTimer < GetProperty<float>("Attack Start Delay"))
            {
                // Spawn the indicator if it is not already present and position it properly
                if (indicator == null)
                {
                    indicator = GameObject.Instantiate(GetProperty<GameObject>("Indicator"), agent.transform);
                    decalProjector = indicator.GetComponentInChildren<DecalProjector>();
                    indicator.transform.position = agent.transform.position;
                    indicator.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                // Fade in the indicator
                decalProjector.fadeFactor = (delayTimer / GetProperty<float>("Attack Start Delay"));
                delayTimer += deltaTime;
                state = NodeData.State.Running;
                return NodeData.State.Running;
            }
            else
            {
                // If the indicator is present, destroy it
                if (indicator != null)
                {
                    GameObject.Destroy(indicator);
                    indicator = null;
                }
                // If the SFX is not present, create it, and add screenshake.
                if (sfx == null)
                {
                    screenshakeManager.AddShakeImpulse(GetProperty<float>("Attack Duration") + 1f, new Vector3(5, 5, 5), 20f);
                    Vector3 breathDir = GetProperty<Transform>("Mouth Bone").up;
                    breathDir.y = 0.0f;
                    sfx = GameObject.Instantiate(
                        GetProperty<GameObject>("Particle Effect"),
                        GetProperty<Transform>("Mouth Bone").position,
                        GetProperty<bool>("Continuous") ? Quaternion.LookRotation(Vector3.up) : Quaternion.LookRotation(agent.transform.forward),
                        GetProperty<bool>("Continuous") ? GetProperty<Transform>("Mouth Bone") : null
                        );
                    if (GetProperty<bool>("Continuous")) sfx.transform.localRotation = Quaternion.LookRotation(Vector3.up);
                }

                // For the duration of the attack
                if (durationTimer < GetProperty<float>("Attack Duration"))
                {
                    durationTimer += deltaTime; // Increment the duration timer
                    if (!hasDamaged || GetProperty<bool>("Continuous")) //If the attack hasnt yet damaged the player, or it is continuous (can damage multiple times)
                    {
                        // If the tick timer is greater than the tick rate, damage the player
                        if (tickTimer < GetProperty<float>("Tick Rate"))
                        {
                            tickTimer += deltaTime;
                        }
                        else
                        {
                            // Check for the player in a spherecast in the up direction of the bone
                            RaycastHit hit;
                            Vector3 breathDir = GetProperty<Transform>("Mouth Bone").up;
                            breathDir.y = 0.0f;
                            Vector3 breathPos = GetProperty<Transform>("Mouth Bone").position;
                            breathPos.y = 0.0f;
                            // Check if the player is in the path of the breath
                            if (Physics.SphereCast(breathPos, 3.0f, GetProperty<bool>("Continuous") ? breathDir : agent.transform.forward, out hit, 100.0f, LayerMask.GetMask("Player")))
                            {
                                Debug.Log("Hit: " + hit.collider.gameObject.name);
                                if (hit.collider.gameObject.tag == "Player")
                                {
                                    hit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(GetProperty<float>("Attack Damage"));
                                    hasDamaged = true;
                                }
                            }
                            tickTimer = 0.0f;
                        }
                    }
                    state = NodeData.State.Running;
                    return NodeData.State.Running;
                }
                else
                {

                    GameObject.Destroy(sfx, 5f);
                    sfx = null;

                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
            }
        }
        public override void OnInit()
        {
            init = false;
            lastTime = Time.time;
        }
    }
}





