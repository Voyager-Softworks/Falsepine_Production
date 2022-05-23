using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Senses
{
    public class CanSeeTag : ConditionBase
    {
        public NodeAI_Senses senses;
        public CanSeeTag()
        {
            AddProperty<string>("Tag", "");
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (senses == null)
            {
                senses = agent.GetComponentInChildren<NodeAI_Senses>();
                if (senses == null)
                {
                    Debug.LogError("No NodeAI_Senses found on " + agent.gameObject.name);
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            string tag = GetProperty<string>("Tag");
            if (tag == "")
            {
                Debug.LogError("No Tag specified for CanSeeTag");
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
            if (senses.CanSeeTag(tag))
            {
                state = NodeData.State.Success;
                return NodeData.State.Success;
            }
            else
            {
                state = NodeData.State.Failure;
                return NodeData.State.Failure;
            }
        }
        
    }
}
