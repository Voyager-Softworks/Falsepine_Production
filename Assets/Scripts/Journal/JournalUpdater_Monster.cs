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

        if (discoveredEntries.Count > 0)
        {
            // update UI
            m_introText.enabled = false;
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

            int discoveredLore = 0;
            foreach (JounralEntry entry in JournalManager.instance.m_discoveredEntries){
                if (entry.m_entryType == JounralEntry.EntryType.Lore && entry.m_linkedMonster == m_monster){
                    discoveredLore++;
                }
            }
            m_killCountText.text += "\n" + "Knowledge: +" + discoveredLore + "% dmg";
        }
    }

    // custom button for editor
    #if UNITY_EDITOR
    [MenuItem("Dev/Select intro")]
    public static void SelectIntro()
    {
        JournalUpdater_Monster[] updaters = FindObjectsOfType<JournalUpdater_Monster>();
        foreach (JournalUpdater_Monster updater in updaters)
        {
            // find child with name "Intro"
            foreach (Transform child in updater.transform)
            {
                if (child.name == "Intro")
                {
                    Selection.activeGameObject = child.gameObject;
                    return;
                }
            }

            // set dirty
            EditorUtility.SetDirty(updater);
        }
    }
    #endif
}