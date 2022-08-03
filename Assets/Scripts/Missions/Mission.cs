using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Mission class stores data about a specific mission. <br/>
/// E.g. Snow Zone, size, type, title, description, and completion status.
/// </summary>
[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/New Mission")]
[Serializable]
public class Mission : ScriptableObject /// @todo Comment
{
    [Serializable]
    public enum MissionSize {
        LESSER,
        GREATER
    };

    [Serializable]
    public enum MissionType {
        COLLECTION,
        EXTERMINATION,
        INVESTIGATION,
        BOSS
    }

    [SerializeField] public MissionSize m_size;

    [SerializeField] public MissionType m_type;

    [SerializeField] public string m_title;

    [TextArea(4, 10)]
    [SerializeField] public string m_description;

    [SerializeField] public bool m_isCompleted;

    public void SetCompleted(bool _val){
        m_isCompleted = _val;
    }

    public void Reset(){
        SetCompleted(false);
    }

    // equality check
    public bool Equals(Mission other)
    {
        if (other == null) return false;
        return this.m_size == other.m_size && this.m_type == other.m_type && this.m_title == other.m_title && this.m_description == other.m_description;
    }

    /// <summary>
    /// Serializable class equivalent for the Mission ScriptableObject
    /// </summary>
    [Serializable]
    public class Serializable_Mission
    {
        [SerializeField] public Mission.MissionSize m_size;
        [SerializeField] public Mission.MissionType m_type;
        [SerializeField] public string m_title;
        [SerializeField] public string m_description;
        [SerializeField] public bool m_isCompleted;

        public Serializable_Mission(Mission _mission)
        {
            m_size = _mission.m_size;
            m_type = _mission.m_type;
            m_title = _mission.m_title;
            m_description = _mission.m_description;
            m_isCompleted = _mission.m_isCompleted;
        }

        public Mission ToMission()
        {
            Mission m = new Mission();
            m.m_size = m_size;
            m.m_type = m_type;
            m.m_title = m_title;
            m.m_description = m_description;
            m.m_isCompleted = m_isCompleted;
            return m;
        }
    }
}