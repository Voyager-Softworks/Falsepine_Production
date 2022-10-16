using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace NodeAI.Agent
{
    public class AgentTransform : NodeAI.Query
    {
        public AgentTransform()
        {
            AddProperty<Transform>("Transform", null, true);
        }
        public override void GetNewValues(NodeAI_Agent agent)
        {
            SetProperty("Transform", agent.transform);
        }

    }
}
