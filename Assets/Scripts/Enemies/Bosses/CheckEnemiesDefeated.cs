using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using System.Linq;
namespace Boss
{
    public class CheckEnemiesDefeated : NodeAI.ConditionBase
    {
        public CheckEnemiesDefeated()
        {
            tooltip = "Check if all enemies are defeated";
        }
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            bool enemiesDefeated = true;
            GameObject.FindObjectsOfType<EnemyGroup>().ToList().ForEach(group =>
            {
                if (group.AreEnemiesAlive())
                {
                    enemiesDefeated = false;
                }
            });
            return enemiesDefeated ? NodeData.State.Success : NodeData.State.Failure;
        }
    }
}

