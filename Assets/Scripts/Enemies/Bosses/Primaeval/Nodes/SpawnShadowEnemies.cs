using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.Primaeval
{

    /// <summary>
    /// 
    /// </summary>  @todo comment
    public class SpawnShadowEnemies : NodeAI.ActionBase
    {
        public PrimaevalSpellManager spellManager;
        public SpawnShadowEnemies()
        {
            tooltip = "Spawns Shadow Enemies";
        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (spellManager == null) spellManager = FindObjectOfType<PrimaevalSpellManager>();
            if (spellManager != null)
            {
                spellManager.SpawnShadowEnemies();
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

