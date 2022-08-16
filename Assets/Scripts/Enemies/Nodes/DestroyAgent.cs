using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

/// <summary>
///  Destroys an agent after a set amount of time.
/// </summary>
public class DestroyAgent : NodeAI.ActionBase
{
    /* This is the constructor for the class. It is adding a property to the class. */
    public DestroyAgent()
    {
        AddProperty<float>("Wait Time", 0.0f);
    }
    /// <summary>
    /// "Destroy the agent after a certain amount of time."
    /// </summary>
    /// <param name="NodeAI_Agent">The agent that is running the tree.</param>
    /// <param name="current">The current leaf that is being evaluated.</param>
    /// <returns>
    /// The state of the node.
    /// </returns>
    public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
    {
        Destroy(agent.gameObject, GetProperty<float>("Wait Time"));
        return NodeData.State.Success;
    }
}
