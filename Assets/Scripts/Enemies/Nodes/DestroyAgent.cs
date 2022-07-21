using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

public class DestroyAgent : NodeAI.ActionBase  /// @todo Comment
{
    public DestroyAgent()
    {
        AddProperty<float>("Wait Time", 0.0f);
    }
    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        Destroy(agent.gameObject, GetProperty<float>("Wait Time"));
        return NodeData.State.Success;
    }
}
