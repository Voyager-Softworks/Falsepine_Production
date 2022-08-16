using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalUpdater_Monster : JournalContentUpdater  /// @todo comment
{
    public JounralEntry.MonsterType bossType = JounralEntry.MonsterType.Bonestag;

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
            if (entry.m_monsterType == bossType)
            {
                contentList.Add(entry.entryContent);
            }
        }

        base.UpdateContent();
    }
}
