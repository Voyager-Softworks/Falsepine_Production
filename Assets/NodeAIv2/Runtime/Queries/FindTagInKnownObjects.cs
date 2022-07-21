using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Senses
{
    /// <summary>
    ///  A Query that returns an object with the specified tag if the Agent is aware of it.
    /// </summary>
    public class FindTagInKnownObjects : Query
    {
        NodeAI_Senses senses;
        public FindTagInKnownObjects()
        {
            AddProperty<string>("Tag", "", false);
            AddProperty<GameObject>("Object", null, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            
            senses = agent.GetComponent<NodeAI_Senses>();
            if (senses == null)
            {
                Debug.LogError("FindTagInKnownObjects: No NodeAI_Senses component found on agent.");
                return;
            }

            GameObject obj = senses.GetAwareObjectWithTag(GetProperty<string>("Tag"));

            SetProperty<GameObject>("Object", obj);
        }
    }
}
