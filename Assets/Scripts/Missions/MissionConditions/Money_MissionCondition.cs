using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This class tracks money gained
/// </summary>
[Serializable]
public class Money_MissionCondition : MissionCondition
{
    [SerializeField] public int m_requiredMoney = 1;
    private int m_currentMoney = 0;

    public override string GetDescription(){
        string description = "Earn " + + m_currentMoney + "/" + m_requiredMoney + " silver";

        return description;
    }

    public override string GetShortDescription()
    {
        return "Earn " + m_requiredMoney + " silver";
    }

    public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        base.OnSceneLoaded(arg0, arg1);

        // find EconomyManager and bind to its OnDamageTaken event
        EconomyManager em = GameObject.FindObjectOfType<EconomyManager>();
        if (em != null)
        {
            em.OnPlayerSilverAdded += (int amount) => { 
                m_currentMoney += amount; 
                UpdateState(); 
            };
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        // if enough money, complete
        if (m_currentMoney >= m_requiredMoney){
            SetState(MissionCondition.ConditionState.COMPLETE);
        }
    }

    public override void BeginCondition()
    {
        base.BeginCondition();

        // set to incomplete
        SetState(ConditionState.INCOMPLETE);

        // reset money
        m_currentMoney = 0;
    }
}