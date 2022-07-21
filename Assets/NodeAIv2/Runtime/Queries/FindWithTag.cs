using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    /// <summary>
    ///  A query that returns an object with the specifies tag.
    /// </summary>
    public class FindWithTag : Query
    {
        public FindWithTag()
        {
            AddProperty<string>("Tag", "", false);
            AddProperty<GameObject>("Result", null, true);
        }

        public override void GetNewValues(NodeAI_Agent agent)
        {
            SetProperty<GameObject>("Result", GameObject.FindGameObjectWithTag(GetProperty<string>("Tag")));
        }
    }
}
