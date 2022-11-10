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
    public JounralEntry.EntryType m_entryType = JounralEntry.EntryType.Clue;
    public Button m_leftButton;
    public Button m_rightButton;
    public TextMeshProUGUI m_tabText;
    public GameObject m_clueTab;
    public TextMeshProUGUI m_clueIntroText;
    public GameObject m_loreTab;
    public TextMeshProUGUI m_loreIntroText;

    [Header("Refs")]
    public Sprite m_undiscoveredSprite;

    protected override void OnEnable() {
        base.OnEnable();

        m_leftButton.onClick.AddListener(OnLeftButtonPressed);
        m_rightButton.onClick.AddListener(OnRightButtonPressed);
    }

    protected override void OnDisable() {
        base.OnDisable();

        m_leftButton.onClick.RemoveListener(OnLeftButtonPressed);
        m_rightButton.onClick.RemoveListener(OnRightButtonPressed);
    }

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
        int count = 0;
        foreach (JounralEntry entry in discoveredEntries)
        {
            if (entry.m_linkedMonster == m_monster && entry.m_entryType == m_entryType)
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

                count++;
            }
        }

        // update tab text
        m_tabText.text = m_entryType.ToString();

        // Enable/disable tabs and intros
        m_clueTab.SetActive(m_entryType == JounralEntry.EntryType.Clue);
        m_loreTab.SetActive(m_entryType == JounralEntry.EntryType.Lore);
        m_clueIntroText.gameObject.SetActive(count <= 0 && m_entryType == JounralEntry.EntryType.Clue);
        m_loreIntroText.gameObject.SetActive(count <= 0 && m_entryType == JounralEntry.EntryType.Lore);

        base.UpdateContent();

        int monsterKills = StatsManager.instance.GetKills(m_monster);
        int monsterJournalEntries = JournalManager.instance?.m_discoveredEntries.FindAll(x => x.m_linkedMonster == m_monster).Count ?? 0;

        // update image:
        Sprite monsterSprite = m_undiscoveredSprite;
        // if kills or entry, show real image
        if (monsterKills > 0 || monsterJournalEntries > 0)
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

        // update name (if no kills "???"), also update the respective content link button
        string monsterName = "???";
        if (monsterKills > 0 || monsterJournalEntries > 0)
        {
            monsterName = m_monster.m_name;
        }
        m_monsterName.text = monsterName;
        JournalManager.ContentsLink link = new JournalManager.ContentsLink();
        // find the link that shares this gameobject
        foreach (JournalManager.ContentsLink l in journalManager.contentsLinks)
        {
            if (l.contents == gameObject)
            {
                link = l;
                break;
            }
        }
        link.button.GetComponentInChildren<TextMeshProUGUI>().text = monsterName;


        // update kill count text
        if (m_killCountText != null)
        {
            m_killCountText.text = "Kills: " + Mathf.Max(0,StatsManager.instance.GetKills(m_monster));

            int discoveredLore = 0;
            foreach (JounralEntry entry in JournalManager.instance.m_discoveredEntries){
                if (entry.m_entryType == JounralEntry.EntryType.Lore && entry.m_linkedMonster == m_monster){
                    discoveredLore++;
                }
            }
            m_killCountText.text += " - " + "Knowledge: +" + discoveredLore + "% dmg";
        }
    }


    // Note: 2 functions if we add more stuff

    /// <summary>
    /// Goes to the next tab
    /// </summary>
    public void OnRightButtonPressed(){
        // swap to next entry type
        if (m_entryType == JounralEntry.EntryType.Lore){
            m_entryType = JounralEntry.EntryType.Clue;
        }
        else{
            m_entryType = JounralEntry.EntryType.Lore;
        }
        UpdateContent();
    }

    /// <summary>
    /// Goes to prev tab
    /// </summary>
    public void OnLeftButtonPressed(){
        // swap to previous entry type
        if (m_entryType == JounralEntry.EntryType.Lore){
            m_entryType = JounralEntry.EntryType.Clue;
        }
        else{
            m_entryType = JounralEntry.EntryType.Lore;
        }
        UpdateContent();
    }

    #if UNITY_EDITOR
    private void OnValidate() {
        // set name of the object to the monster name
        if (m_monster != null){
            // remove spaces
            string name = m_monster.m_name.Replace(" ", "") + "_Content";
            if (gameObject.name != name){
                gameObject.name = name;
            }
        }
    }
    #endif
}