using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Singleton donotdestroy script that handles the journal system
/// </summary>
[Serializable]
public class JournalManager : ToggleableWindow
{
    /// <summary>
    /// Used to link a button to a specific page of content
    /// </summary>
    [Serializable]
    public class ContentsLink
    {
        public Button button;
        public GameObject contents;
    }

    public static JournalManager instance;

    [Header("Keys")]
    public InputAction openJournalAction;
    public InputAction closeAction;

    [Header("Sounds")]
    public AudioClip openJournalSound;
    public AudioClip addInfoSound;

    [Header("References")]
    public GameObject journalPanel;

    public List<ContentsLink> contentsLinks;

    private AudioSource _audioSource;

    [Header("Entries")]
    public List<JounralEntry> m_undiscoveredEntries = new List<JounralEntry>();
    public List<JounralEntry> m_discoveredEntries = new List<JounralEntry>();

    [Header("UI variables")]
    public bool m_showHUDConditions = true;
    public Button m_HUDButton;
    public bool m_showTutorial = true;

    /// <summary>
    /// Class used to save the journal entries
    /// </summary> 
    private class SaveData{
        public List<JounralEntry.SerializableJournalEntry> undiscoveredEntries = new List<JounralEntry.SerializableJournalEntry>();
        public List<JounralEntry.SerializableJournalEntry> discoveredEntries = new List<JounralEntry.SerializableJournalEntry>();

        public bool showHUDConditions = false;
        public bool showTutorial = false;
    }

    public static string GetSaveFolderPath(int saveSlot)
    {
        return SaveManager.GetSaveFolderPath(saveSlot) + "/journal/";
    }

    public static string GetSaveFilePath(int saveSlot)
    {
        return GetSaveFolderPath(saveSlot) + "clues.json";
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadJournal(SaveManager.currentSaveSlot);

            //add listeners to the buttons
            foreach (ContentsLink link in contentsLinks)
            {
                link.button?.onClick.AddListener(() =>
                {
                    OpenContents(link.contents);
                    link.button.interactable = false;
                });
            }

            // HUD button
            if (m_HUDButton != null)
            {
                m_HUDButton.onClick.AddListener(() =>
                {
                    m_showHUDConditions = !m_showHUDConditions;

                    // set color
                    //m_HUDButton.image.color = m_showHUDConditions ? Color.green : Color.white;

                    // update Tracking button
                    UpdateTrackingButton();
                });

                // update tracking button
                UpdateTrackingButton();
            }

            // update all content
            UpdateAllContent();
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    private void UpdateAllContent()
    {
        foreach (ContentsLink link in contentsLinks)
        {
            link.contents.GetComponent<JournalContentUpdater>()?.UpdateContent();
        }
    }

    private void UpdateTrackingButton()
    {
        if (m_showHUDConditions)
        {
            m_HUDButton.GetComponentInChildren<TextMeshProUGUI>().text = "Tracking";
        }
        else
        {
            m_HUDButton.GetComponentInChildren<TextMeshProUGUI>().text = "Track";
        }
    }

    /// <summary>
    /// Opens a specific contents panel
    /// </summary>
    /// <param name="contents"></param>
    public void OpenContents(GameObject contents)
    {
        DisableAllContents();

        //enable the contents
        contents.SetActive(true);

        //get the scroll rect
        ScrollRect scrollRect = contents.GetComponent<ScrollRect>();
        if (scrollRect == null) return;
        //scroll to the top
        scrollRect.verticalNormalizedPosition = 1;

        
        // sound
        UIAudioManager.instance?.buttonSound.Play();
    }

    public void OpenToEntry(JounralEntry _entry)
    {   
        // find the monster in the journal
        JournalUpdater_Monster[] updaters = GetComponentsInChildren<JournalUpdater_Monster>(true);
        foreach (JournalUpdater_Monster updater in updaters)
        {
            if (updater.m_monster == _entry.m_linkedMonster)
            {
                // find the content link JournalManager.instance.contentsLinks
                foreach (JournalManager.ContentsLink link in JournalManager.instance.contentsLinks)
                {
                    if (link.contents == updater.gameObject)
                    {
                        // open the journal
                        JournalManager.instance.OpenWindow();

                        // open the contentupdater
                        updater.m_entryType = _entry.m_entryType;
                        JournalManager.instance.OpenContents(link.contents);
                        break;
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    /// Closes all contents panels
    /// </summary>
    private void DisableAllContents()
    {
        //disable all other contents
        foreach (ContentsLink link in contentsLinks)
        {
            link.contents.SetActive(false);
        }

        // enable all buttons
        foreach (ContentsLink link in contentsLinks)
        {
            if (link.button) link.button.interactable = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    

    protected override void OnEnable() {
        base.OnEnable();

        openJournalAction.Enable();
    }

    protected override void OnDisable() {
        base.OnDisable();
        
        openJournalAction.Disable();
    }

    // Update is called once per frame
    public override void Update()
    {   
        base.Update();

        if (openJournalAction.WasPressedThisFrame())
        {
            // get console window
            Console consoleWindow = FindObjectOfType<Console>();
            // if console is open, don't open journal
            if (consoleWindow != null && consoleWindow.IsOpen())
            {
                return;
            }
            ToggleWindow();
        }

        // hide hud button if no current mission
        if (MissionManager.instance.GetCurrentMission() == null){
            m_HUDButton.gameObject.SetActive(false);
        }
        else{
            m_HUDButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Adds specific entry to the journal, and removes it from the undiscovered list (if it is there)
    /// @todo make some sort of UI notification that a new entry has been added
    /// </summary>
    /// <param name="entry"></param>
    public bool DiscoverEntry(JounralEntry entry)
    {
        if (entry == null) return false;

        if (m_undiscoveredEntries.Contains(entry))
        {
            m_undiscoveredEntries.Remove(entry);
        }

        if (!m_discoveredEntries.Contains(entry))
        {
            m_discoveredEntries.Add(entry);
            // send message
            MessageManager.instance?.AddMessage("New Journal Entry\n" + entry.m_linkedMonster.m_name, "monster", _linkedJournalEntry: entry);
            // notify
            NotificationManager.instance?.AddIconAtPlayer("monster");
            // sound
            UIAudioManager.instance?.entryDiscoveredSound.Play();

            // if all entries for this monster are discovered, unlock achievement
            bool allFound = true;
            foreach (JounralEntry _entry in m_undiscoveredEntries)
            {
                if (_entry.m_linkedMonster == entry.m_linkedMonster)
                {
                    allFound = false;
                    break;
                }
            }
            if (allFound)
            {
                Achievements.AchievementsManager.instance?.UnlockAchievement(entry.m_linkedMonster.m_entryAchievement);
            }

            // if all entries are discovered, unlock achievement
            if (m_undiscoveredEntries.Count == 0)
            {
                Achievements.AchievementsManager.instance?.UnlockAchievement(Achievements.AchievementsManager.Achievement.AllLoreCompleted);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Discovers a random entry that matches the filters passed in
    /// </summary>
    /// <param name="_monster"></param>
    /// <param name="_entryType"></param>
    public bool DiscoverRandomEntry(MonsterInfo _monster = null, JounralEntry.EntryType? _entryType = null)
    {
        JounralEntry toDiscover = GetRandomUndiscoveredEntry(_monster: _monster, _entryType: _entryType);
        return DiscoverEntry(toDiscover);
    }

    /// <summary>
    /// Gets a random undisovered entry that matches the filters passed in
    /// </summary>
    /// <param name="_monster"></param>
    /// <param name="_monsterType"></param>
    /// <param name="_entryType"></param>
    /// <returns></returns>
    public JounralEntry GetRandomUndiscoveredEntry(MonsterInfo _monster = null, MonsterInfo.MonsterType? _monsterType = null, JounralEntry.EntryType? _entryType = null)
    {
        // get valid entries
        List<JounralEntry> undisLore = GetUndiscoveredEntries(_monster, _monsterType, _entryType);

        // if there is any, pick one at random and discover it
        if (undisLore.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, undisLore.Count);
            return undisLore[index];
        }

        return null;
    }

    /// <summary>
    /// Gets a list of undiscovered entries matching the filters passed in
    /// </summary>
    /// <param name="_monster"></param>
    /// <param name="_monsterType"></param>
    /// <param name="_entryType"></param>
    /// <returns></returns>
    public List<JounralEntry> GetUndiscoveredEntries(MonsterInfo _monster = null, MonsterInfo.MonsterType? _monsterType = null, JounralEntry.EntryType? _entryType = null)
    {
        List<JounralEntry> undisLore = new List<JounralEntry>();
        foreach (JounralEntry entry in m_undiscoveredEntries)
        {
            if (_monster != null && entry.m_linkedMonster != _monster) continue;
            if (_monsterType != null && entry.m_linkedMonster.m_type != _monsterType) continue;
            if (_entryType != null && entry.m_entryType != _entryType) continue;
            undisLore.Add(entry);
        }

        return undisLore;
    }

    /// <summary>
    /// Save the journal
    /// </summary>
    public void SaveJournal(int saveSlot)
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }

        FileStream file = File.Create(GetSaveFilePath(saveSlot));

        // make data
        SaveData data = new SaveData();
        foreach (JounralEntry entry in m_undiscoveredEntries)
        {
            data.undiscoveredEntries.Add(new JounralEntry.SerializableJournalEntry(entry));
        }
        foreach (JounralEntry entry in m_discoveredEntries)
        {
            data.discoveredEntries.Add(new JounralEntry.SerializableJournalEntry(entry));
        }
        data.showHUDConditions = m_showHUDConditions;
        data.showTutorial = m_showTutorial;
        

        StreamWriter writer = new StreamWriter(file);
        
        // save the data
        writer.Write(JsonUtility.ToJson(data, true));
        
        writer.Close();

        file.Close();
    }

    /// <summary>
    /// Load the journal
    /// </summary>
    public void LoadJournal(int saveSlot)
    {
        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }
        // if save file doesn't exist, return
        if (!File.Exists(GetSaveFilePath(saveSlot)))
        {
            Debug.Log("Save file does not exist.");
            return;
        }

        // read the file
        FileStream file = File.Open(GetSaveFilePath(saveSlot), FileMode.Open);

        StreamReader reader = new StreamReader(file);

        // load the data
        SaveData data = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());
        // if data is null, return
        if (data == null) return;

        // clear the lists
        m_undiscoveredEntries.Clear();
        m_discoveredEntries.Clear();

        // add the data to the lists
        foreach (JounralEntry.SerializableJournalEntry entry in data.undiscoveredEntries)
        {
            m_undiscoveredEntries.Add(entry.ToEntry());
        }
        foreach (JounralEntry.SerializableJournalEntry entry in data.discoveredEntries)
        {
            m_discoveredEntries.Add(entry.ToEntry());
        }
        m_showHUDConditions = data.showHUDConditions;
        m_showTutorial = data.showTutorial;

        reader.Close();
        file.Close();
    }

    /// <summary>
    /// Deletes the save file
    /// </summary>
    public void DeleteJournalSave(int saveSlot)
    {
        if (File.Exists(GetSaveFilePath(saveSlot)))
        {
            File.Delete(GetSaveFilePath(saveSlot));
        }
    }

    /// <summary>
    /// Updates the journal UI
    /// </summary>
    public void UpdateJournalUI(){
        //update mission cards
        MissionManager missionManager = FindObjectOfType<MissionManager>();

        if (missionManager == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }
        else
        {
            missionManager.UpdateAllMissionCards();
        }
    }

    // Toggleable Window overrides
    public override bool IsOpen()
    {
        if (journalPanel == null) return false;

        return journalPanel.activeSelf;
    }
    public override void OpenWindow()
    {
        // if tutorial popup is open, return
        if (FindObjectOfType<TutorialPopup>() && FindObjectOfType<TutorialPopup>().gameObject.activeSelf) {
            return;
        }
        // if toggleable windows are open, return
        // if (ToggleableTownWindow.AnyWindowOpen()){
        //     return;
        // }

        base.OpenWindow();
        journalPanel.SetActive(true);

        // update all content
        UpdateAllContent();
    }
    public override void CloseWindow()
    {
        base.CloseWindow();
        journalPanel.SetActive(false);
    }

    // custom editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(JournalManager))]
    public class JournalManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            JournalManager myScript = (JournalManager)target;
            
            // add all un-added entries
            if (GUILayout.Button("Add Missing Entries"))
            {
                AddAllEntries();
            }
        }

        /// <summary>
        /// Adds all entries from assets that are not in the journal list
        /// </summary>
        private void AddAllEntries()
        {
            JournalManager myScript = (JournalManager)target;

            // get all entries from the asset database
            List<JounralEntry> entries = new List<JounralEntry>();
            foreach (JounralEntry entry in AssetDatabase.FindAssets("t:JounralEntry").Select(guid => AssetDatabase.LoadAssetAtPath<JounralEntry>(AssetDatabase.GUIDToAssetPath(guid))))
            {
                entries.Add(entry);
            }

            // if an entry is not in discovered or undiscovered, add it to undiscovered
            foreach (JounralEntry entry in entries)
            {
                if (!myScript.m_discoveredEntries.Contains(entry) && !myScript.m_undiscoveredEntries.Contains(entry))
                {
                    myScript.m_undiscoveredEntries.Add(entry);
                }
            }

            // set dirty
            EditorUtility.SetDirty(myScript);
        }

        // binds the intro text to the intro text field
        // public void SelectIntros()
        // {
        //     JournalManager myScript = (JournalManager)target;

        //     JournalUpdater_Monster[] updaters = myScript.GetComponentsInChildren<JournalUpdater_Monster>(true);
        //     foreach (JournalUpdater_Monster updater in updaters)
        //     {
        //         // find child with name "Intro"
        //         TextMeshProUGUI[] texts = updater.GetComponentsInChildren<TextMeshProUGUI>(true);
        //         foreach (TextMeshProUGUI text in texts)
        //         {
        //             if (text.name == "Intro")
        //             {
        //                 updater.m_introText = text;
        //                 continue;
        //             }
        //         }

        //         // set dirty
        //         EditorUtility.SetDirty(updater);
        //     }

        //     // set dirty
        //     EditorUtility.SetDirty(myScript);
        // }
    }
    #endif
}
