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
    private float m_timeElapsed = 0f;
    private bool m_updateTimer = false;

    public override string GetDescription(){
        return "Spend less than " + (m_timeLimit - m_timeElapsed).ToString("0") + " seconds in the expedition";
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

        // only update the time if this scene is one of the zone scenes
        m_updateTimer = false;
        foreach (Utilities.SceneField scene in MissionManager.instance.GetCurrentZone().GetSceneList()){
            if (arg0.path.Contains(scene.scenePath)){
                m_updateTimer = true;
                break;
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
        if (m_updateTimer && !m_lockState){
            m_timeElapsed += Time.deltaTime;
            if (m_timeElapsed > m_timeLimit){
                SetState(ConditionState.FAILED);
            }
        }
    }
}