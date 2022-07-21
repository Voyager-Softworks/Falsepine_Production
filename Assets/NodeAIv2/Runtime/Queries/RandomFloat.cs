using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Random
{
    /// <summary>
    ///  A query that returns a random float.
    /// </summary>
    /// <remarks>
    /// This query is useful for randomizing the behavior of a node.
    /// The result of this query is a random float between 0 and 1.
    /// </remarks>
    public class RandomFloat : Query
    {
        public RandomFloat()
        {
            AddProperty<float>("Result", 0.0f, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            SetProperty<float>("Result", UnityEngine.Random.value);
        }
    }
}