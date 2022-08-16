using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A condition that must be manually completed in code.
/// </summary>
public class Manual_Condition : LevelCondition
{
    public void Complete()
    {
        m_isComplete = true;
    }

    protected override void UpdateCondition()
    {
        // Do nothing
    }
}
