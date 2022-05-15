using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Animation
{
    public class SetAnimatorFloat : ActionBase
    {
        Animator animator;
        int hash;
        bool value;
        public SetAnimatorFloat()
        {
            AddProperty<string>("Name", "Animator");
            AddProperty<float>("Value", 0);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("SetAnimatorFloat: No animator found on agent");
                    return NodeData.State.Failure;
                }
                hash = Animator.StringToHash(GetProperty<string>("Name"));
            }
            animator.SetFloat(hash, GetProperty<float>("Value"));
            return NodeData.State.Success;
        }
    }
}

