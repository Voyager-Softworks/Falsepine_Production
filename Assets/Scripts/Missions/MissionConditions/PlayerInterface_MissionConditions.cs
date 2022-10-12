using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This class tracks primary used event
/// </summary>
[Serializable]
public class NoPrimaryUsed_MissionCondition : MissionCondition
{
    public override string GetDescription(){
        return "Don't fire your primary weapon";
    }

    public override string GetShortDescription()
    {
        return GetDescription();
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

        // find PlayerInventoryInterface and bind to its OnPrimaryUsed event
        PlayerInventoryInterface pi = GameObject.FindObjectOfType<PlayerInventoryInterface>();
        if (pi != null)
        {
            pi.OnPrimaryUsed += () => { SetState(ConditionState.FAILED); };
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to complete
        SetState(ConditionState.COMPLETE);
    }
}

/// <summary>
/// This class tracks secondary used event
/// </summary>
[Serializable]
public class NoSecondaryUsed_MissionCondition : MissionCondition
{
    public override string GetDescription(){
        return "Don't fire your secondary weapon";
    }

    public override string GetShortDescription()
    {
        return GetDescription();
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

        // find PlayerInventoryInterface and bind to its OnSecondaryUsed event
        PlayerInventoryInterface pi = GameObject.FindObjectOfType<PlayerInventoryInterface>();
        if (pi != null)
        {
            pi.OnSecondaryUsed += () => { SetState(ConditionState.FAILED); };
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to complete
        SetState(ConditionState.COMPLETE);
    }
}

/// <summary>
/// This class tracks equipment used event
/// </summary>
[Serializable]
public class NoEquipmentUsed_MissionCondition : MissionCondition
{
    public override string GetDescription(){
        return "Don't use any equipment";
    }

    public override string GetShortDescription()
    {
        return GetDescription();
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

        // find PlayerInventoryInterface and bind to its OnEquipmentUsed event
        PlayerInventoryInterface pi = GameObject.FindObjectOfType<PlayerInventoryInterface>();
        if (pi != null)
        {
            pi.OnEquipmentUsed += () => { SetState(ConditionState.FAILED); };
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to complete
        SetState(ConditionState.COMPLETE);
    }
}