using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This class tracks kills for a specific enemy type, using the stats manager to track the kills.
/// </summary>
[Serializable]
public class Kill_MissionCondition : MissionCondition
{
    [SerializeField] public MonsterInfo m_monsterToKill = null;
    [HideInInspector, SerializeField] private List<StatsManager.MonsterStat> m_initialStats = new List<StatsManager.MonsterStat>();
    [SerializeField] public int m_requiredKills = 1;

    [Header("Optional")]
    [Tooltip("If DISABLED, all kills count"), SerializeField] public bool m_useOptional = false;
    [Tooltip("If no item, monster must be killed with this state"), SerializeField] public StatsManager.StatType m_statToKillWith = null;
    [Tooltip("Overrides the stat, monster must be killed with this item"), SerializeField] public Item m_itemToKillWith = null;

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

        // check if the player has killed enough enemies:

        // get the new monster (StatsManager.instance.m_monsterStats - m_initialStats) ~~~ WRONG!! @Todo Fix this to exclude old KILLS not old MONSTERS (see unfinished code below)
        List<StatsManager.MonsterStat> newKills = StatsManager.instance.m_monsterStats.Except(m_initialStats).ToList();

        if (!m_useOptional){
            if (newKills.Count >=  m_requiredKills)
            {
                SetState(ConditionState.COMPLETE);
            }
        }
        else{
            foreach (StatsManager.MonsterStat stat in newKills){
                if (m_itemToKillWith != null && stat. //should actually be looping through kills here, and cehcking if they match the optionals.
            }
        }
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