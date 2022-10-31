using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This class tracks damage taken event
/// </summary>
[Serializable]
public class NoDamage_MissionCondition : MissionCondition
{
    public override string GetDescription(){
        return "Don't take any damage";
    }

    public override string GetShortDescription()
    {
        return GetDescription();
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

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

    public override void ResetCondition()
    {
        base.ResetCondition();
    }
}

/// <summary>
/// This class tracks melee used event
/// </summary>
[Serializable]
public class NoMeleeUsed_MissionCondition : MissionCondition
{
    public override string GetDescription(){
        return "Don't use melee";
    }

    public override string GetShortDescription()
    {
        return GetDescription();
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

        // find PlayerInventoryInterface and bind to its OnMeleeUsed event
        PlayerInventoryInterface pi = GameObject.FindObjectOfType<PlayerInventoryInterface>();
        if (pi != null)
        {
            pi.OnMeleeUsed += () => { SetState(ConditionState.FAILED); };
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to complete
        SetState(ConditionState.COMPLETE);
    }

    public override void ResetCondition()
    {
        base.ResetCondition();
    }
}

/// <summary>
/// This class tracks reload event
/// </summary>
[Serializable]
public class NoReload_MissionCondition : MissionCondition
{
    public override string GetDescription(){
        return "Don't reload";
    }

    public override string GetShortDescription()
    {
        return GetDescription();
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

        // find PlayerInventoryInterface and bind to its OnReload event
        PlayerInventoryInterface pi = GameObject.FindObjectOfType<PlayerInventoryInterface>();
        if (pi != null)
        {
            pi.OnReload += () => { SetState(ConditionState.FAILED); };
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to complete
        SetState(ConditionState.COMPLETE);
    }

    public override void ResetCondition()
    {
        base.ResetCondition();
    }
}