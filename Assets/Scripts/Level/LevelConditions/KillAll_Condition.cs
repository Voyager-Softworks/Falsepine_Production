using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A condition which requires all enemies to be killed.
/// @Todo: Make it more reliable when checking for enemies.
/// </summary>
public class KillAll_Condition : LevelCondition
{
    protected override void UpdateCondition()
    {
        m_isComplete = true;
        // get all "enemies" that have a healthscript
        List<HealthScript> enemies = GameObject.FindObjectsOfType<HealthScript>(true).ToList();
        // reverse backwards removing any enemies that dont have AI script
        for (int i = enemies.Count() - 1; i >= 0; i--){
            GameObject enemy = enemies[i].gameObject;
            if (!enemy.GetComponent<NodeAI.NodeAI_Agent>()){
                enemies.RemoveAt(i);
                continue;
            }
        }

        // check that all of them are dead, if not, set false, break
        foreach (HealthScript enemy in enemies){
            if (!enemy.isDead){
                m_isComplete = false;
                break;
            }
        }
    }
}
