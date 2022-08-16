using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Scriptable object that holds the content of a journal entry.
/// </summary>
[CreateAssetMenu(fileName = "JournalEntry", menuName = "Journal/Entry")]
public class JounralEntry : ScriptableObject  /// @todo comment
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
}
