using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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
    [Serializable]
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [SerializeField] public string m_title;

    [TextArea(4, 10)]
    [SerializeField] public string m_description;
    [SerializeField] public Difficulty m_difficulty;

    [SerializeReference] public List<MissionCondition> m_conditions = new List<MissionCondition>();

    [SerializeField] public bool m_lockOnComplete = true;
    [SerializeField] private bool m_isLockedComplete = false;

    public void OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        foreach (MissionCondition condition in m_conditions)
        {
            condition.OnSceneLoaded(arg0, arg1);
        }
    }

    public void Update(){
        foreach (MissionCondition condition in m_conditions)
        {
            condition.Update();
        }
    }

    public MissionCondition.ConditionState GetState(){
        if (m_lockOnComplete && m_isLockedComplete) return MissionCondition.ConditionState.COMPLETE;

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
            LockMissionComplete();
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

    private void LockMissionComplete()
    {
        if (!m_lockOnComplete) return;

        m_isLockedComplete = true;

        //lock all conditions
        foreach (MissionCondition condition in m_conditions)
        {
            condition.m_lockState = true;
        }
    }

    public void SetState(MissionCondition.ConditionState _state){
        foreach (MissionCondition condition in m_conditions)
        {
            condition.SetState(_state);
        }
    }

    private void UpdateState()
    {
        if (m_lockOnComplete && m_isLockedComplete) return;

        foreach (MissionCondition condition in m_conditions)
        {
            condition.UpdateState();
        }
    }

    public void BeginMission()
    {
        if (m_lockOnComplete && m_isLockedComplete) return;

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
        m_isLockedComplete = false;

        //unlock all conditions
        foreach (MissionCondition condition in m_conditions)
        {
            if (condition == null) continue;
            condition.m_lockState = false;
        }   

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

        if (a.m_title == b.m_title && a.m_description == b.m_description){
            // and all conditions are equal
            if (a.m_conditions.Count == b.m_conditions.Count){
                for (int i = 0; i < a.m_conditions.Count; i++)
                {
                    if (a.m_conditions[i] != b.m_conditions[i]){
                        return false;
                    }
                }
                return true;
            }
        }

        return false;
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
        [SerializeField] public Difficulty m_difficulty;
        [SerializeReference] public List<MissionCondition> m_conditions = new List<MissionCondition>();

        public Serializable_Mission(Mission _mission)
        {
            if (_mission == null)
            {
                return;
            }
            m_title = _mission.m_title;
            m_description = _mission.m_description;
            m_difficulty = _mission.m_difficulty;
            m_conditions = new List<MissionCondition>();
            foreach (MissionCondition condition in _mission.m_conditions)
            {
                m_conditions.Add(condition);
            }
        }

        public Mission ToMission()
        {
            Mission m = new Mission();
            m.m_title = m_title;
            m.m_description = m_description;
            m.m_difficulty = m_difficulty;
            m.m_conditions = new List<MissionCondition>();
            foreach (MissionCondition condition in m_conditions)
            {
                m.m_conditions.Add(condition);
            }
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
                menu.AddItem(new GUIContent("Speed Run"), false, () => { mission.m_conditions.Add(new Speedrun_MissionCondition()); });
                menu.AddItem(new GUIContent("No Primary"), false, () => { mission.m_conditions.Add(new NoPrimaryUsed_MissionCondition()); });
                menu.AddItem(new GUIContent("No Secondary"), false, () => { mission.m_conditions.Add(new NoSecondaryUsed_MissionCondition()); });
                menu.AddItem(new GUIContent("No Equipment"), false, () => { mission.m_conditions.Add(new NoEquipmentUsed_MissionCondition()); });
                menu.AddItem(new GUIContent("No Reload"), false, () => { mission.m_conditions.Add(new NoReload_MissionCondition()); });
                menu.ShowAsContext();
            }
        }
    }
#endif
}