using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Monster info scriptable object.
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "MonsterInfo", menuName = "MonsterInfo/MonsterInfo")]
public class MonsterInfo : ScriptableObject
{
    [Serializable]
    public enum MonsterType
    {
        Boss,
        Minion
    }

    public string m_name;
    public MonsterType m_type;

    //@todo decide if zone is needed here
    // zone?

    //equality operator
    public static bool operator ==(MonsterInfo lhs, MonsterInfo rhs)
    {
        if (System.Object.ReferenceEquals(lhs, rhs))
        {
            return true;
        }

        if (((object)lhs == null) || ((object)rhs == null))
        {
            return false;
        }

        return lhs.m_name == rhs.m_name && lhs.m_type == rhs.m_type;
    }
    //inequality operator
    public static bool operator !=(MonsterInfo lhs, MonsterInfo rhs)
    {
        return !(lhs == rhs);
    }
    //override equals method
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        MonsterInfo other = obj as MonsterInfo;
        if (other == null)
        {
            return false;
        }
        return this == other;
    }

    [Serializable]
    public class SerializableMonsterInfo
    {
        public string m_name;
        public MonsterType m_type;

        public SerializableMonsterInfo(MonsterInfo monster)
        {
            m_name = monster.m_name;
            m_type = monster.m_type;
        }
        public MonsterInfo ToMonsterInfo()
        {
            MonsterInfo monster = ScriptableObject.CreateInstance<MonsterInfo>();
            monster.m_name = m_name;
            monster.m_type = m_type;
            return monster;
        }
    }
}
