using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace NodeAI.Animation
{
    public class WaitUntilAnimatorState : NodeAI.ActionBase
    {
        public Animator animator;
        public WaitUntilAnimatorState()
        {
            tooltip = "Waits until the Animator is in the specified state.";
            AddProperty<string>("State Name", "");
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponent<Animator>();
            }
            if (animator == null)
            {
                return NodeData.State.Failure;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(GetProperty<string>("State Name")))
            {
                return NodeData.State.Success;
            }
            return NodeData.State.Running;
        }
    }
}
