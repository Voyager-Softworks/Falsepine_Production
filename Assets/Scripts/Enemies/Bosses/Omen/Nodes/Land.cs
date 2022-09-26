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
            if (!init)
            {
                animator.SetTrigger("Land");
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
            time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (time >= 0.95f)
            {
                time = 1f;
                state = NodeData.State.Success;
            }
            else state = NodeData.State.Running;
            agent.transform.position = Vector3.Lerp(startPos, destination, time);
            agent.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRot, rotation, time));
            return state;
        }

        public override void OnInit()
        {
            init = false;
        }
    }
}