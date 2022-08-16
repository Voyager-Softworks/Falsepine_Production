using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

/// <summary>
/// Sets a boolean parameter on the Agent.
/// </summary>
public class SetBoolParameter : NodeAI.ActionBase
{
    public SetBoolParameter()
    {
        AddProperty<string>("ParameterName", "");
        AddProperty<bool>("Value", false);
        tooltip = "Set a bool parameter";
    }

    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        agent.SetParameter<bool>(GetProperty<string>("ParameterName"), GetProperty<bool>("Value"));
        state = NodeData.State.Success;
        return NodeData.State.Success;
    }
}
