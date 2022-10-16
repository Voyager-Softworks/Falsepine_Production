using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using UnityEngine.Rendering.HighDefinition;

namespace Boss.Omen
{
    public class BreathAttack : NodeAI.ActionBase
    {
        Animator animator;
        float delayTimer = 0.0f;
        float durationTimer = 0.0f;
        float tickTimer = 0.0f;
        float lastTime = 0.0f;

        bool init = false;
        bool hasDamaged = false;

        GameObject indicator = null;
        DecalProjector decalProjector = null;
        GameObject sfx = null;
        AudioSource source = null;
        RotateTowardsPlayer rotateTowardsPlayer = null;
        ScreenshakeManager screenshakeManager = null;

        public BreathAttack()
        {
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
            float deltaTime = Time.time - lastTime;
            lastTime = Time.time;
            if (animator == null) animator = agent.GetComponent<Animator>();
            if (source == null) source = agent.GetComponent<AudioSource>();
            if (rotateTowardsPlayer == null) rotateTowardsPlayer = agent.GetComponent<RotateTowardsPlayer>();
            if (screenshakeManager == null) screenshakeManager = FindObjectOfType<ScreenshakeManager>();
            if (!init)
            {
                delayTimer = 0.0f;
                durationTimer = 0.0f;
                tickTimer = 0.0f;
                hasDamaged = false;
                animator.SetTrigger(GetProperty<string>("Animation Trigger"));
                source.PlayOneShot(GetProperty<AudioClip>("Sound"));
                rotateTowardsPlayer.RotateToPlayer(0.6f, 12f, 0.2f);
                init = true;
            }

            if (delayTimer < GetProperty<float>("Attack Start Delay"))
            {
                if (indicator == null)
                {
                    indicator = GameObject.Instantiate(GetProperty<GameObject>("Indicator"), agent.transform);
                    decalProjector = indicator.GetComponentInChildren<DecalProjector>();
                    indicator.transform.position = agent.transform.position;
                    indicator.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                decalProjector.fadeFactor = (delayTimer / GetProperty<float>("Attack Start Delay"));
                delayTimer += deltaTime;
                state = NodeData.State.Running;
                return NodeData.State.Running;
            }
            else
            {
                if (indicator != null)
                {
                    GameObject.Destroy(indicator);
                    indicator = null;
                }
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


                if (durationTimer < GetProperty<float>("Attack Duration"))
                {
                    durationTimer += deltaTime;
                    if (!hasDamaged || GetProperty<bool>("Continuous"))
                    {
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





