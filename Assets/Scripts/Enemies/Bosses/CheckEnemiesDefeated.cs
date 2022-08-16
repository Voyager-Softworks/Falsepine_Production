using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;
using System.Linq;
namespace Boss
{
    /* Check if all enemies are defeated */
    public class CheckEnemiesDefeated : NodeAI.ConditionBase
    {
        public CheckEnemiesDefeated()
        {
            tooltip = "Check if all enemies are defeated";
        }
        /// <summary>
        /// If there are no enemies alive, return success, otherwise return failure
        /// </summary>
        /// <param name="NodeAI_Agent">The agent that is running the tree.</param>
        /// <param name="current">The current leaf that is being evaluated.</param>
        /// <returns>
        /// A NodeData.State.
        /// </returns>
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

