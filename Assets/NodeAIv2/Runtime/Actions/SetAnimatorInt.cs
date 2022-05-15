using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Animation
{
    public class SetAnimatorInt : ActionBase
    {
        Animator animator;
        int hash;
        int value;
        public SetAnimatorInt()
        {
            AddProperty<string>("Name", "Animator");
            AddProperty<int>("Value", 0);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("SetAnimatorInt: No animator found on agent");
                    return NodeData.State.Failure;
                }
                hash = Animator.StringToHash(GetProperty<string>("Name"));
            }
            animator.SetInteger(hash, GetProperty<int>("Value"));
            return NodeData.State.Success;
        }
    }
}
