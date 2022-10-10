using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.Primaeval
{
    public class HealthBasedDelay : NodeAI.Query
    {
        public float healthThreshold = 0.5f;
        public float delay = 0.0f;
        public HealthBasedDelay()
        {
            tooltip = "Delays for a set amount of time based on the boss's health";
            AddProperty<float>("Max Health", 0f, false);
            AddProperty<float>("Current Health", 0f, false);
            AddProperty<float>("Max Delay", 0f, false);
            AddProperty<float>("Min Delay", 0f, false);
            AddProperty<float>("Delay", 0f, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            SetProperty<float>("Delay", // Set the Delay
            Mathf.Lerp(GetProperty<float>("Min Delay"), GetProperty<float>("Max Delay"), //To a value between the min and max delay
            GetProperty<float>("Current Health") / GetProperty<float>("Max Health")) //Based on the current proportion of health
            );
        }
    }
}


