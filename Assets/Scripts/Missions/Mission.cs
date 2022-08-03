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

    [SerializeField] public MissionZone m_parentZone;

    [SerializeField] public MissionType m_type;

    [SerializeField] public string m_title;

    [TextArea(4, 10)]
    [SerializeField] public string m_description;

    [SerializeField] public bool m_isCompleted;

    public void SetCompleted(bool _val){
        m_isCompleted = _val;
    }

    // equality check
    public bool Equals(Mission other)
    {
        if (other == null) return false;
        return this.m_size == other.m_size && this.m_parentZone == other.m_parentZone && this.m_type == other.m_type && this.m_title == other.m_title && this.m_description == other.m_description;
    }
}