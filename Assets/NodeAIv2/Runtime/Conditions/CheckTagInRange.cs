using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
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
                    return NodeData.State.Success;
                }
            }
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