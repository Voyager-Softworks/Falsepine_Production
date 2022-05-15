using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
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
                animator = agent.GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogError("SetAnimatorBool: No animator found on agent");
                    return NodeData.State.Failure;
                }
                hash = Animator.StringToHash(GetProperty<string>("Name"));
            }
            animator.SetBool(hash, GetProperty<bool>("Value"));
            return NodeData.State.Success;
        }
    }
}

