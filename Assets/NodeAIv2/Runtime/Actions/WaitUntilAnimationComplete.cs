using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Animation
{
    public class WaitUntilAnimationComplete : ActionBase
    {
        Animator animator;
        public WaitUntilAnimationComplete()
        {
            AddProperty<string>("State Name", "");
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("WaitUntilAnimationComplete: No animator found on agent");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(GetProperty<string>("State Name")) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                state = NodeData.State.Success;
                return NodeData.State.Success;
            }
            else
            {
                state = NodeData.State.Running;
                return NodeData.State.Running;
            }
        }
    }
}

