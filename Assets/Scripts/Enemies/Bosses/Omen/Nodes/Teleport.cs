using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

/// <summary>
///  Node to handle teleportation of the agent.
/// </summary>
public class Teleport : NodeAI.ActionBase
{
    public Teleport()
    {
        AddProperty<Transform>("Destination", null);
    }

    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {

        agent.transform.position = GetProperty<Transform>("Destination").position;
        agent.transform.rotation = GetProperty<Transform>("Destination").rotation;
        agent.GetComponent<Animator>().rootRotation = GetProperty<Transform>("Destination").rotation;
        state = NodeData.State.Success;
        return state;
    }


}
