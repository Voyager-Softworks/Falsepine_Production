/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: SetAnimatorTrigger.cs
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
    ///    A Node that sets an Animator's trigger.
    /// </summary>
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
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
    }
}

