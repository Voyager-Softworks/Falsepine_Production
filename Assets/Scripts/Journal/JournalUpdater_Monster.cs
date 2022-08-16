using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for updating a journal page with the latest information.
/// </summary>
public class JournalUpdater_Monster : JournalContentUpdater
{
    public MonsterInfo m_monster = null;

    public override void UpdateContent()
    {
        // refresh the content list:

        // get JournalManager instance
        JournalManager journalManager = JournalManager.instance;
        if (journalManager == null)
        {
            Debug.LogError("JournalManager instance not found!");
            return;
        }

        // get discovered entries
        List<JounralEntry> discoveredEntries = journalManager.discoveredEntries;

        // clear current content
        contentList.Clear();

        // add new content that has the same boss type
        foreach (JounralEntry entry in discoveredEntries)
        {
            if (entry.m_linkedMonster == m_monster)
            {
                contentList.Add(entry.entryContent);
            }
        }

        base.UpdateContent();
    }
}
