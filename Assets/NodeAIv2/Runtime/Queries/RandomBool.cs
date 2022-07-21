using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Random
{
    /// <summary>
    ///  A query that returns a random boolean.
    /// </summary>
    public class RandomBool : Query
    {
        public RandomBool()
        {
            AddProperty<bool>("Result", false, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            SetProperty<bool>("Result", UnityEngine.Random.value > 0.5f);
        }
    }
}

