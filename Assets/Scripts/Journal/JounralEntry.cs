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
        Entry,
        Clue
    }

    [Serializable]
    public enum BossType
    {
        Bonestag,
        Brightmaw
    }

    public EntryType entryType = EntryType.Entry;

    public BossType bossType = BossType.Bonestag;

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
