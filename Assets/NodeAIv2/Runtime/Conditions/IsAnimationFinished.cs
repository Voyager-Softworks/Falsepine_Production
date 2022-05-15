using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Animation
{
    public class IsAnimationFinished : ConditionBase
    {
        Animator animator;
        
        
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (animator == null)
            {
                animator = agent.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("IsAnimationFinished: No animator found on agent");
                    return NodeData.State.Failure;
                }
                
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                return NodeData.State.Success;
            }
            else
            {
                return NodeData.State.Failure;
            }
        }
    }
}

