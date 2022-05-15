using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Animation
{
    public class SetAnimatorTrigger : ActionBase
    {
        Animator animator;
        int hash;
        public SetAnimatorTrigger()
        {
            AddProperty<string>("Name", "Animator");
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("SetAnimatorTrigger: No animator found on agent");
                    return NodeData.State.Failure;
                }
                hash = Animator.StringToHash(GetProperty<string>("Name"));
            }
            animator.SetTrigger(hash);
            return NodeData.State.Success;
        }
    }
}

