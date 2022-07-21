/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: IsAnimationFinished.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Animation
{
    /// <summary>
    ///  A Node that checks if an Animator's animation is finished.
    /// </summary>
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
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                state = NodeData.State.Success;
                return NodeData.State.Success;
            }
            else
            {
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
    }
}

