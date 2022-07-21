using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Utility
{
    /// <summary>
    ///  A Query Node that returns the distance of an object from the agent.
    /// </summary>
    public class DistanceFromAgent : Query
    {
        public DistanceFromAgent()
        {
            AddProperty<Transform>("Target", null, false);
            AddProperty<float>("Distance", 0, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if(GetProperty<Transform>("Target") == null)
            {
                SetProperty<float>("Distance", 0);
                return;
            }
            SetProperty<float>("Distance", Vector3.Distance(agent.transform.position, GetProperty<Transform>("Target").position));
        }
    }
}
