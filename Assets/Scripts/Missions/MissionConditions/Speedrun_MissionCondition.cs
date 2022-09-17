using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This class tracks kills for a specific enemy type, using the stats manager to track the kills.
/// </summary>
[Serializable]
public class Speedrun_MissionCondition : MissionCondition
{
    public float m_timeLimit = 60f;
    [SerializeField, HideInInspector] private float m_timeElapsed = 0f;
    private bool m_updateTimer = false;

    [Header("Optional")]
    [Tooltip("If monster specified, timer will only tick when enemy is in the scene")]public MonsterInfo m_triggerMonster = null;
    [SerializeField, HideInInspector] private bool m_triggerMonsterSeen = true;

    public override string GetDescription(){
        string desc = "Spend less than " + (m_timeLimit - m_timeElapsed).ToString("0") + " seconds in the expedition";
        if (m_triggerMonster != null){
            desc += " while fighting " + (m_triggerMonster.m_type == MonsterInfo.MonsterType.Boss ? "the " : "a ") + m_triggerMonster.m_name;
        }
        return desc;
    }

    public override string GetShortDescription()
    {
        return "Time limit";
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

        // only update the time if this scene is one of the zone scenes
        m_updateTimer = false;
        foreach (Utilities.SceneField scene in MissionManager.instance.GetCurrentZone().GetSceneList())
        {
            if (arg0.path.Contains(scene.scenePath))
            {
                m_updateTimer = true;
                break;
            }
        }

        // find all Health_Base objects and check if they are the trigger monster
        if (m_updateTimer && m_triggerMonster != null)
        {
            m_triggerMonsterSeen = false;
            Health_Base[] healths = GameObject.FindObjectsOfType<Health_Base>();
            foreach (Health_Base health in healths)
            {
                EnemyHealth enemyHealth = health as EnemyHealth;
                if (enemyHealth != null && enemyHealth.m_monsterType == m_triggerMonster)
                {
                    m_triggerMonsterSeen = true;
                    break;
                }
            }
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to complete
        SetState(ConditionState.COMPLETE);
    }

    public override void Update()
    {
        base.Update();

        // update timer
        if (m_updateTimer && m_triggerMonsterSeen && !m_lockState)
        {
            m_timeElapsed += Time.deltaTime;
            if (m_timeElapsed >= m_timeLimit)
            {
                SetState(ConditionState.FAILED);
            }
        }
    }
}