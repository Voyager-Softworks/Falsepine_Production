using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Responsible for updating a journal page with the latest information.
/// </summary>
public class JournalUpdater_Monster : JournalContentUpdater
{
    public MonsterInfo m_monster = null;

    public Image m_monsterImage;
    public TextMeshProUGUI m_monsterName;
    public TextMeshProUGUI m_killCountText;

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
        List<JounralEntry> discoveredEntries = journalManager.m_discoveredEntries;

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

        // update image
        m_monsterImage.sprite = m_monster.m_monsterImage;
        // get all children of the image and set sprite to the same
        Image[] children = m_monsterImage.GetComponentsInChildren<Image>();
        foreach (Image child in children)
        {
            child.sprite = m_monster.m_monsterImage;
        }

        // update name
        m_monsterName.text = m_monster.m_name;

        // update kill count text
        if (m_killCountText != null)
        {
            m_killCountText.text = "Kills: " + StatsManager.instance.GetKills(m_monster);
        }
    }
}
