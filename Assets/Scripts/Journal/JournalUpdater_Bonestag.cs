using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalUpdater_Bonestag : JournalContentUpdater
{
    private JounralEntry.BossType bossType = JounralEntry.BossType.Bonestag;

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
            if (entry.bossType == bossType)
            {
                contentList.Add(entry.entryContent);
            }
        }

        base.UpdateContent();
    }
}
