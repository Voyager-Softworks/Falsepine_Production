using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.Bonestag
{
    public class CloakBegone : NodeAI.ActionBase
    {
        public CloakBegone()
        {
            AddProperty<UnityEngine.GameObject>("Cloak", null);
            AddProperty<Material>("Invis Material", null);

        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (GetProperty<UnityEngine.GameObject>("Cloak") == null)
            {
                state = NodeData.State.Failure;
                return state;
            }
            Material[] mats = GetProperty<UnityEngine.GameObject>("Cloak").GetComponent<SkinnedMeshRenderer>().materials;
            mats[2] = GetProperty<Material>("Invis Material");
            GetProperty<UnityEngine.GameObject>("Cloak").GetComponent<SkinnedMeshRenderer>().materials = mats;

            state = NodeData.State.Success;
            return state;
        }
    }
}
