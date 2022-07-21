/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: SetAnimatorInt.cs
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
    ///  A Node that sets an Animator's integer parameter.
    /// </summary>
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
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                hash = Animator.StringToHash(GetProperty<string>("Name"));
            }
            animator.SetInteger(hash, GetProperty<int>("Value"));
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
    }
}
