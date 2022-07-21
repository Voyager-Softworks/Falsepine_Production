/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: CheckTagInRange.cs
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
    ///  A Node that checks if a GameObject has a certain tag within a certain range.
    /// </summary>
    public class CheckTagInRange : ConditionBase
    {
        public CheckTagInRange()
        {
            AddProperty<string>("Tag", "");
            AddProperty<float>("Range", 1f);
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            var tag = GetProperty<string>("Tag");
            var range = GetProperty<float>("Range");
            var targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (var target in targets)
            {
                var distance = Vector3.Distance(agent.transform.position, target.transform.position);
                if (distance < range)
                {
                    state = NodeData.State.Success;
                    return NodeData.State.Success;
                }
            }
            state = NodeData.State.Failure;
            return NodeData.State.Failure;
        }

        public override void DrawGizmos(NodeAI_Agent agent)
        {
            var tag = GetProperty<string>("Tag");
            var range = GetProperty<float>("Range");
            var targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (var target in targets)
            {
                var distance = Vector3.Distance(agent.transform.position, target.transform.position);
                if (distance < range)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(agent.transform.position, target.transform.position);
                }
            }
        }
    }
}