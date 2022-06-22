using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/New Mission")]
[Serializable]
public class Mission : ScriptableObject
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

    [SerializeField] public string description;

    [SerializeField] public bool isCompleted;

    // equality check
    public bool Equals(Mission other)
    {
        return this.size == other.size && this.zone == other.zone && this.type == other.type && this.title == other.title && this.description == other.description;
    }

}