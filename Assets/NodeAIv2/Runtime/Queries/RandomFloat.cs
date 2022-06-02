using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Random
{
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