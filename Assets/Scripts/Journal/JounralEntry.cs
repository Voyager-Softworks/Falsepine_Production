using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Scriptable object that holds the content of a journal entry.
/// </summary>
[CreateAssetMenu(fileName = "JournalEntry", menuName = "Journal/Entry")]
public class JounralEntry : ScriptableObject
{
    [Serializable]
    public enum EntryType
    {
        Lore,
        Clue
    }

    public EntryType m_entryType = EntryType.Lore;

    public MonsterInfo m_linkedMonster;

    public JournalContent entryContent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // equality operator
    public static bool operator ==(JounralEntry lhs, JounralEntry rhs)
    {
        if (System.Object.ReferenceEquals(lhs, rhs))
        {
            return true;
        }

        if (((object)lhs== null) || ((object)rhs == null))
        {
            return false;
        }

        return lhs.m_entryType == rhs.m_entryType && lhs.m_linkedMonster == rhs.m_linkedMonster && lhs.entryContent == rhs.entryContent;
    }
    //inequality operator
    public static bool operator !=(JounralEntry lhs, JounralEntry rhs)
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
        JounralEntry other = obj as JounralEntry;
        if (other == null)
        {
            return false;
        }
        return this == other;
    }


    [Serializable]
    public class SerializableJournalEntry
    {
        public EntryType m_entryType;
        public MonsterInfo.SerializableMonsterInfo m_linkedMonster;
        public JournalContent m_entryContent;

        public SerializableJournalEntry(JounralEntry entry)
        {
            m_entryType = entry.m_entryType;
            m_linkedMonster = new MonsterInfo.SerializableMonsterInfo(entry.m_linkedMonster);
            m_entryContent = entry.entryContent;
        }

        public JounralEntry ToEntry()
        {
            JounralEntry entry = ScriptableObject.CreateInstance<JounralEntry>();
            entry.m_entryType = m_entryType;
            entry.m_linkedMonster = m_linkedMonster.ToMonsterInfo();
            entry.entryContent = m_entryContent;
            return entry;
        }
    }
}
