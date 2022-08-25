using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

/// <summary>
///  Node that sets the invulnerability status of an Agent.
/// </summary>
public class SetInvulnerable : NodeAI.ActionBase
{
    public SetInvulnerable()
    {
        AddProperty<bool>("Invulnerable", true);
    }

    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        agent.GetComponent<Health_Base>().isInvulnerable = GetProperty<bool>("Invulnerable");
        state = NodeData.State.Success;
        return NodeData.State.Success;
    }
}

