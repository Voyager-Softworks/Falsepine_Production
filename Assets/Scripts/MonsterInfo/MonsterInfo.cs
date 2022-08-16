using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monster info scriptable object.
/// </summary>
[CreateAssetMenu(fileName = "MonsterInfo", menuName = "MonsterInfo/MonsterInfo")]
public class MonsterInfo : ScriptableObject
{
    public enum MonsterType
    {
        Boss,
        Minion
    }

    public string m_name;
    public MonsterType m_type;

    //@todo decide if zone is needed here
    // zone?
}
