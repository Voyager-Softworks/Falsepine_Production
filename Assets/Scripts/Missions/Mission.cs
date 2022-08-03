using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public enum MissionZone {
        SNOW,
        DESERT,
        REDWOOD,
        SWAMP,
        FOREST
    }

    [Serializable]
    public enum MissionType {
        COLLECTION,
        EXTERMINATION,
        INVESTIGATION,
        BOSS
    }

    [SerializeField] public MissionSize size;

    [SerializeField] public MissionZone zone;

    [SerializeField] public MissionType type;

    [SerializeField] public string title;

    [TextArea(4, 10)]
    [SerializeField] public string description;

    [SerializeField] public bool isCompleted;

    public void SetCompleted(bool _val){
        isCompleted = _val;
    }

    // equality check
    public bool Equals(Mission other)
    {
        if (other == null) return false;
        return this.size == other.size && this.zone == other.zone && this.type == other.type && this.title == other.title && this.description == other.description;
    }

}