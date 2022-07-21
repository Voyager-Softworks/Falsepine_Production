/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: SetAnimatorBool.cs
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
    ///  A Node that sets a bool on an Animator.
    /// </summary>
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

