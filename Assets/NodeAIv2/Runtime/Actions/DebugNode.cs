using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
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
