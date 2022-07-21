/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: DebugNode.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    /// <summary>
    /// A Node that prints out a message to the console.
    /// </summary>
    public class DebugNode : ActionBase
    {
        public DebugNode()
        {
            AddProperty<string>("Message", "Debug");
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            Debug.Log("Debug: " + GetProperty<string>("Message"));
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
    }
}
