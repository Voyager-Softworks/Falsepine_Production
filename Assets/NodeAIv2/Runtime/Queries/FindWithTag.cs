using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public class FindWithTag : Query
    {
        GameObject foundObject;
        public FindWithTag()
        {
            AddProperty<string>("Tag", "", false);
            AddProperty<GameObject>("Result", null, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            if (foundObject == null)
            {
                foundObject = GameObject.FindGameObjectWithTag(GetProperty<string>("Tag"));
            }
            SetProperty<GameObject>("Result", foundObject);
        }
    }
}
