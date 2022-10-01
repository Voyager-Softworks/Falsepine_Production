using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.Omen
{
    public class Land : NodeAI.ActionBase
    {
        RootMotionFlight flight; ///< The flight script.
        Animator animator; ///< The animator.
        AudioSource audioSource; ///< The audio source.

        bool init = false; ///< Whether or not the node has been initialized.

        float timer = 0f; ///< The timer for the landing animation.
        float time = 0f;
        float oldTime = 0f;

        Vector3 startPos;
        Vector3 startRot;

        public Land()
        {
            AddProperty<Vector3>("Destination", Vector3.zero);
            AddProperty<Vector3>("Rotation", Vector3.zero);
            AddProperty<AudioClip>("Sound Effect", null);
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("No Animator found on agent");
                    state = NodeData.State.Failure;
                    return state;
                }
            }
            if (audioSource == null)
            {
                audioSource = agent.GetComponentInChildren<AudioSource>();
                if (audioSource == null)
                {
                    Debug.LogError("No AudioSource found on agent");
                    state = NodeData.State.Failure;
                    return state;
                }
            }
            if (!init)
            {
                animator.SetTrigger("Land");
                audioSource.PlayOneShot(GetProperty<AudioClip>("Sound Effect"));
                timer = 0f;
                init = true;
                startPos = agent.transform.position;
                startRot = agent.transform.rotation.eulerAngles;
            }

            Vector3 destination = GetProperty<Vector3>("Destination");
            Vector3 rotation = GetProperty<Vector3>("Rotation");
            float lerpSpeed = GetProperty<float>("Lerp Speed");
            string animationName = GetProperty<string>("Animation Name");

            timer += Time.time - oldTime;
            oldTime = Time.time;
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
            {
                time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }
            else
            {
                time = 0f;
            }

            if (time >= 1f)
            {
                state = NodeData.State.Success;
            }
            else state = NodeData.State.Running;
            float scaledTime = Mathf.Clamp(time * 1.2f, 0f, 1f);
            agent.transform.position = Vector3.Lerp(startPos, destination, scaledTime);
            agent.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRot, rotation, scaledTime));
            return state;
        }

        public override void OnInit()
        {
            init = false;
        }
    }
}