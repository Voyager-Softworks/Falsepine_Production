using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Responsible for updating a journal page with the latest information.
/// </summary>
public class JournalUpdater_Monster : JournalContentUpdater
{
    public MonsterInfo m_monster = null;

    public Image m_monsterImage;
    public TextMeshProUGUI m_monsterName;
    public TextMeshProUGUI m_killCountText;
    public TextMeshProUGUI m_introText;

    [Header("Refs")]
    public Sprite m_undiscoveredSprite;

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
                // make title content
                if (!string.IsNullOrEmpty(entry.m_title)){
                    JournalContent titleContent = new JournalContent();
                    titleContent.text = entry.m_title;
                    titleContent.bold = true;
                    titleContent.image = null;
                    contentList.Add(titleContent);
                }
                // add all other content
                contentList.Add(entry.entryContent);
            }
        }

        if (discoveredEntries.Count > 0)
        {
            // update UI
            m_introText.gameObject.SetActive(false);
        }

        base.UpdateContent();


        // update image:
        Sprite monsterSprite = m_undiscoveredSprite;
        // if no kills, disable image
        if (StatsManager.instance.GetKills(m_monster) > 0)
        {
            monsterSprite = m_monster.m_monsterImage;
        }

        m_monsterImage.sprite = monsterSprite;
        // get all children of the image and set sprite to the same
        Image[] children = m_monsterImage.GetComponentsInChildren<Image>();
        foreach (Image child in children)
        {
            child.sprite = monsterSprite;
        }

        // update name
        m_monsterName.text = m_monster.m_name;

        // update kill count text
        if (m_killCountText != null)
        {
            m_killCountText.text = "Kills: " + StatsManager.instance.GetKills(m_monster);

            int discoveredLore = 0;
            foreach (JounralEntry entry in JournalManager.instance.m_discoveredEntries){
                if (entry.m_entryType == JounralEntry.EntryType.Lore && entry.m_linkedMonster == m_monster){
                    discoveredLore++;
                }
            }
            m_killCountText.text += "\n" + "Knowledge: +" + discoveredLore + "% dmg";
        }
    }
}