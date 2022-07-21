/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: IsAwareOfObject.cs
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
    ///  A Node that checks if the agent is aware of the target object.
    /// </summary>
    public class IsAwareOfObject : ConditionBase
    {
        public NodeAI_Senses senses;
        public IsAwareOfObject()
        {
            AddProperty<GameObject>("Object", null);

            tooltip = "Checks if the agent is currently aware of the specified object";
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (senses == null)
            {
                senses = agent.GetComponentInChildren<NodeAI_Senses>();
                if (senses == null)
                {
                    Debug.LogError("No NodeAI_Senses found on " + agent.gameObject.name);
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            if (senses.IsAwareOf(GetProperty<GameObject>("Object")))
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
