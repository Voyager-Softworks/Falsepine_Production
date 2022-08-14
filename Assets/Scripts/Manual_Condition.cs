using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manual_Condition : LevelCondition
{
    public void Complete(){
        m_isComplete = true;
    }

    protected override void UpdateCondition()
    {
        // Do nothing
    }
}
