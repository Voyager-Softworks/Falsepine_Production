using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/New Mission")]
public class Mission : ScriptableObject
{
    public enum MissionSize {
        LESSER,
        MAJOR
    }

    public enum MissionZone {
        SNOW,
        DESERT,
        REDWOOD,
        SWAMP,
        FOREST
    }

    public enum MissionType {
        COLLECTION,
        EXTERMINATION,
        INVESTIGATION
    }

    [SerializeField] public MissionSize size;

    [SerializeField] public MissionZone zone;

    [SerializeField] public MissionType type;

    [SerializeField] public string title;

    [SerializeField] public string description;

    [SerializeField] public bool isCompleted;
}