/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: CanSee.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Senses
{
    /// <summary>
    ///  A Node that checks if the agent can see the target.
    /// </summary>
    public class CanSee : ConditionBase
    {
        public NodeAI_Senses senses;
        public CanSee()
        {
            AddProperty<GameObject>("Object", null);
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(senses == null)
            {
                senses = agent.GetComponentInChildren<NodeAI_Senses>();
                if(senses == null)
                {
                    Debug.LogError("No NodeAI_Senses found on " + agent.gameObject.name);
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            GameObject obj = GetProperty<GameObject>("Object");
            if(obj == null)
            {
                Debug.LogError("No Object specified for CanSee");
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
            if(senses.CanSee(obj))
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
