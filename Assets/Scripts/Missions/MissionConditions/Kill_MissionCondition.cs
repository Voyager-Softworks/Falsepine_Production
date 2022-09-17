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
    [HideInInspector, SerializeField] private List<Health_Base.DamageStat> m_initialKills = new List<Health_Base.DamageStat>();
    [SerializeField] public int m_requiredKills = 1;

    [Header("Optional")]
    [Tooltip("If DISABLED, all kills count"), SerializeField] public bool m_useOptional = false;

    [Tooltip("If no item, monster must die to this stat")] public StatsManager.StatType m_statToKillWith = null;
    
    [Tooltip("Overrides the stat, monster must be die to this item"), SerializeField] public Item m_itemToKillWith = null;

    public override string GetDescription(){
        return "Kill " + m_requiredKills + " " + m_monsterToKill?.m_name + "(s)";
    }

    public override string GetShortDescription()
    {
        return "Kill monster(s)";
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

        List<Health_Base.DamageStat> relevantKills = new List<Health_Base.DamageStat>();

        // if no enemy specified, use all kills in MonsterStats
        if (m_monsterToKill == null){
            relevantKills = StatsManager.instance.m_monsterStats.SelectMany(x => x.m_kills).ToList();
        }
        else{
            // find the relevant monster stat
            StatsManager.MonsterStat monsterStat = StatsManager.instance.m_monsterStats.Find(x => x.m_monster == m_monsterToKill);

            // if no stat found, no kills
            if (monsterStat == null){
                relevantKills = new List<Health_Base.DamageStat>();
            }
            else{
                relevantKills = monsterStat.m_kills;
            }
        }

        // subtract the initial kills (by reference)
        foreach (Health_Base.DamageStat initialKill in m_initialKills)
        {
            relevantKills.Remove(initialKill);
        }

        // if using optional, filter by weapon, if no weapon, then stat
        if (m_useOptional){
            if (m_itemToKillWith != null){
                //only need to match ID's
                relevantKills = relevantKills.Where(x => (x.m_sourceStats as Item)?.id == m_itemToKillWith.id).ToList();
            }
            else if (m_statToKillWith != null){
                relevantKills = relevantKills.Where(x => x.m_sourceStats.GetStatTypes().Contains(m_statToKillWith)).ToList();
            }
        }

        // if enough kills, complete
        if (relevantKills.Count >= m_requiredKills){
            SetState(MissionCondition.ConditionState.COMPLETE);
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to incomplete
        SetState(ConditionState.INCOMPLETE);

        // set initial stats
        m_initialStats = new List<StatsManager.MonsterStat>(StatsManager.instance.m_monsterStats);

        // set initial kills
        m_initialKills = new List<Health_Base.DamageStat>();
        foreach (StatsManager.MonsterStat monster in m_initialStats)
        {
            m_initialKills.AddRange(monster.m_kills);
        }
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