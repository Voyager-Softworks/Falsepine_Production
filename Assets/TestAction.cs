using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

public class TestAction : NodeAI.ActionBase
{
    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        return base.Eval(agent, current);
    }
}
