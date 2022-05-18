using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Animation
{
    public class SetAnimatorBool : ActionBase
    {
        Animator animator;
        int hash;
        bool value;
        public SetAnimatorBool()
        {
            AddProperty<string>("Name", "Animator");
            AddProperty<bool>("Value", false);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("SetAnimatorBool: No animator found on agent");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                hash = Animator.StringToHash(GetProperty<string>("Name"));
            }
            animator.SetBool(hash, GetProperty<bool>("Value"));
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
    }
}

