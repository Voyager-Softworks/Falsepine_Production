using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A condition which requires all enemies to be killed.
/// @Todo: Make it more reliable when checking for enemies.
/// </summary>
public class KillThis_Condition : LevelCondition
{
    protected override void UpdateCondition()
    {
        m_isComplete = false;

        // get health script
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (health.hasDied){
            m_isComplete = true;
        }
    }
}
