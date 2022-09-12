using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This class tracks kills for a specific enemy type, using the stats manager to track the kills.
/// </summary>
[Serializable]
public class Kill_MissionCondition : MissionCondition
{
    [SerializeField] public MonsterInfo m_monsterToKill = null;
    [ReadOnly][SerializeField] private List<StatsManager.MonsterStat> m_initialStats = new List<StatsManager.MonsterStat>();
    [SerializeField] public int m_requiredKills = 1;

    [SerializeField] public StatsManager.StatType m_statToKillWith = null;
    [SerializeField] public Item m_itemToKillWith = null;

    public override string GetDescription(){
        return "Kill " + m_requiredKills + " " + m_monsterToKill?.m_name + "(s)";
    }

    public override void Update()
    {
        base.Update();

        UpdateState();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        // check if the player has killed enough enemies
        // if (StatsManager.instance.GetKills(m_monsterToKill) >= m_initialCount + m_requiredKills)
        // {
        //     SetState(ConditionState.COMPLETE);
        // }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to incomplete
        SetState(ConditionState.INCOMPLETE);

        // set initial count
        //m_initialCount = StatsManager.instance.GetKills(m_monsterToKill);
    }

    public override void EndCondition()
    {
        base.EndCondition();

        // if not complete, set to failed
        if (GetState() != ConditionState.COMPLETE)
        {
            SetState(ConditionState.FAILED);
        }
    }
}