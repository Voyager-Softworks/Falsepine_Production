using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A condition which requires all enemies to be killed.
/// @Todo: Make it more reliable when checking for enemies.
/// </summary>
public class KillThis_Condition : LevelCondition  /// @todo comment
{
    protected override void UpdateCondition()
    {
        m_isComplete = false;

        // get health script
        Health_Base health = GetComponent<Health_Base>();
        if (health == null) return;

        if (health.hasDied)
        {
            m_isComplete = true;
        }
    }
}
