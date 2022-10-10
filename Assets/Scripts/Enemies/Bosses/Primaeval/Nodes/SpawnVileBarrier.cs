using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.Primaeval
{

    /// <summary>
    /// 
    /// </summary>  @todo comment
    public class SpawnVileBarrier : NodeAI.ActionBase
    {
        public PrimaevalSpellManager spellManager;
        public SpawnVileBarrier()
        {
            tooltip = "Spawns a Vile Barrier";
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (spellManager == null) spellManager = agent.GetComponent<PrimaevalSpellManager>();
            if (spellManager != null)
            {
                spellManager.SpawnVileBarrier();
                state = NodeData.State.Success;
            }
            else
            {
                Debug.LogError("No PrimaevalSpellManager found in scene");
                state = NodeData.State.Failure;
            }

            return state;
        }
    }
}


