using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This class tracks kills for a specific enemy type, using the stats manager to track the kills.
/// </summary>
[Serializable]
public class NoDamage_MissionCondition : MissionCondition
{
    public override string GetDescription(){
        return "Don't take any damage";
    }

    public override void Start()
    {
        base.Start();

        // find PlayerHealth and bind to its OnDamageTaken event
        PlayerHealth ph = GameObject.FindObjectOfType<PlayerHealth>();
        if (ph != null)
        {
            ph.OnDamageTaken += () => { SetState(ConditionState.FAILED); };
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to complete
        SetState(ConditionState.COMPLETE);
    }
}