using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Mission class stores data about a specific mission. <br/>
/// E.g. Snow Zone, size, type, title, description, and completion status.
/// </summary>
[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/New Mission")]
[Serializable]
public class Mission : ScriptableObject
{
    [SerializeField] public string m_title;

    [TextArea(4, 10)]
    [SerializeField] public string m_description;

    [SerializeReference] public List<MissionCondition> m_conditions = new List<MissionCondition>();

    public MissionCondition.ConditionState GetState(){
        UpdateState();

        // if all conditions are complete, return complete
        // if any conditions are failed, return failed
        // otherwise, return incomplete

        bool allComplete = true;
        bool anyFailed = false;

        foreach (MissionCondition condition in m_conditions)
        {
            if (condition.GetState() == MissionCondition.ConditionState.COMPLETE)
            {
                continue;
            }
            else if (condition.GetState() == MissionCondition.ConditionState.FAILED)
            {
                anyFailed = true;
                allComplete = false;
                break;
            }
            else if (condition.GetState() == MissionCondition.ConditionState.INCOMPLETE)
            {
                allComplete = false;
                continue;
            }
        }

        if (allComplete)
        {
            return MissionCondition.ConditionState.COMPLETE;
        }
        else if (anyFailed)
        {
            return MissionCondition.ConditionState.FAILED;
        }
        else
        {
            return MissionCondition.ConditionState.INCOMPLETE;
        }
    }

    public void SetState(MissionCondition.ConditionState _state){
        foreach (MissionCondition condition in m_conditions)
        {
            condition.SetState(_state);
        }
    }

    public void UpdateState()
    {
        foreach (MissionCondition condition in m_conditions)
        {
            condition.UpdateState();
        }
    }

    public void BeginMission()
    {
        foreach (MissionCondition condition in m_conditions)
        {
            condition.BeginCondition();
        }
    }

    public void EndMission()
    {
        foreach (MissionCondition condition in m_conditions)
        {
            condition.EndCondition();
        }   
    }

    /// <summary>
    /// Resets the mission to not completed.
    /// </summary>
    public void Reset(){
        SetState(MissionCondition.ConditionState.INCOMPLETE);
    }

    // equality operator
    public static bool operator ==(Mission a, Mission b)
    {
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        return a.m_title == b.m_title && a.m_description == b.m_description && a.m_conditions == b.m_conditions;
    }
    //inequality operator
    public static bool operator !=(Mission a, Mission b)
    {
        return !(a == b);
    }
    //override equals method
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        Mission other = obj as Mission;
        if (other == null)
        {
            return false;
        }
        return this == other;
    }
    

    /// <summary>
    /// Serializable class equivalent for the Mission ScriptableObject
    /// </summary>
    [Serializable]
    public class Serializable_Mission
    {
        [SerializeField] public string m_title;
        [SerializeField] public string m_description;
        [SerializeField] public bool m_isCompleted;
        [SerializeField] public List<MissionCondition> m_conditions = new List<MissionCondition>();

        public Serializable_Mission(Mission _mission)
        {
            if (_mission == null)
            {
                return;
            }
            m_title = _mission.m_title;
            m_description = _mission.m_description;
            m_conditions = new List<MissionCondition>(_mission.m_conditions);
        }

        public Mission ToMission()
        {
            Mission m = new Mission();
            m.m_title = m_title;
            m.m_description = m_description;
            m.m_conditions = new List<MissionCondition>(m_conditions);
            return m;
        }
    }

    // custom editor for mission
#if UNITY_EDITOR
    [CustomEditor(typeof(Mission))]
    public class MissionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Mission mission = (Mission)target;

            // add condition button, which then displays a popup to select a condition type
            if (GUILayout.Button("Add Condition"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Kill Enemy"), false, () => { mission.m_conditions.Add(new Kill_MissionCondition()); });
                menu.AddItem(new GUIContent("No Damage"), false, () => { mission.m_conditions.Add(new NoDamage_MissionCondition()); });
                menu.ShowAsContext();
            }
        }
    }
#endif
}