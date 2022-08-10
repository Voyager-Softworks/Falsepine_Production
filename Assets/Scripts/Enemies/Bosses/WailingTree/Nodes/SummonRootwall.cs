using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

/// <summary>
/// The Wailing Tree Boss Enemy
/// </summary>
namespace Boss.WailingTree
{
    /// <summary>
    ///  A node for the Wailing Tree bosses rootwall summon attack.
    /// </summary>
    public class SummonRootwall : NodeAI.ActionBase
    {
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            GameObject.FindObjectOfType<RootwallManager>().SpawnRootwall();
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
    }
}

