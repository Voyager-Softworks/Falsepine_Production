using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A condition which requires this specific thing to be killed.
/// </summary>
public class KillThis_Condition : LevelCondition
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
