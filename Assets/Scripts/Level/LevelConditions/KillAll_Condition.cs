using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A condition which requires all enemies to be killed.
/// @Todo: Make it more reliable when checking for enemies.
/// </summary>
public class KillAll_Condition : LevelCondition  /// @todo comment
{
    protected override void UpdateCondition()
    {
        m_isComplete = true;
        // get all "enemies" that have a healthscript
        List<EnemyHealth> enemies = GameObject.FindObjectsOfType<EnemyHealth>(/* true */).ToList();

        // check that all of them are dead, if not, set false, break
        foreach (EnemyHealth enemy in enemies)
        {
            if (!enemy.hasDied)
            {
                m_isComplete = false;
                break;
            }
        }
    }
}
