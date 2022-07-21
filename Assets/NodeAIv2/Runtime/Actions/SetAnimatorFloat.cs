/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: SetAnimatorFloat.cs
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
    ///  A Node that sets the value of an Animator float.
    /// </summary>
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
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                hash = Animator.StringToHash(GetProperty<string>("Name"));
            }
            animator.SetFloat(hash, GetProperty<float>("Value"));
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
    }
}

