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

    public List<JounralEntry> undiscoveredEntries = new List<JounralEntry>();
    public List<JounralEntry> discoveredEntries = new List<JounralEntry>();

    /// <summary>
    /// Class used to save the journal entries
    /// </summary> 
    private class SaveData{
        public List<JounralEntry.SerializableJournalEntry> undiscoveredEntries = new List<JounralEntry.SerializableJournalEntry>();
        public List<JounralEntry.SerializableJournalEntry> discoveredEntries = new List<JounralEntry.SerializableJournalEntry>();
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
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Opens a specific contents panel
    /// </summary>
    /// <param name="contents"></param>
    private void OpenContents(GameObject contents)
    {
        DisableAllContents();

        //enable the contents
        contents.SetActive(true);

        //get the scroll rect
        ScrollRect scrollRect = contents.GetComponent<ScrollRect>();
        if (scrollRect == null) return;
        //scroll to the top
        scrollRect.verticalNormalizedPosition = 1;
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

    void OnEnable()
    {
        openJournalAction.Enable();
    }

    void OnDisable()
    {
        openJournalAction.Disable();
    }

    // Update is called once per frame
    public override void Update()
    {   
        base.Update();

        if (openJournalAction.WasPressedThisFrame())
        {
            ToggleWindow();
        }
    }

    /// <summary>
    /// Adds specific entry to the journal, and removes it from the undiscovered list (if it is there)
    /// </summary>
    /// <param name="entry"></param>
    public void DiscoverEntry(JounralEntry entry)
    {
        if (entry == null) return;

        if (undiscoveredEntries.Contains(entry))
        {
            undiscoveredEntries.Remove(entry);
        }

        discoveredEntries.Add(entry);
    }

    /// <summary>
    /// Discovers a random entry that matches the MonsterType? and EntryType? if given
    /// </summary>
    public void DiscoverRandomEntry(MonsterInfo _monster = null, JounralEntry.EntryType? _entryType = null)
    {
        // get valid entries
        List<JounralEntry> undisLore = new List<JounralEntry>();
        foreach (JounralEntry entry in undiscoveredEntries) {
            if (_monster != null && entry.m_linkedMonster != _monster) continue;
            if (_entryType != null && entry.m_entryType != _entryType) continue;
            undisLore.Add(entry);
        }

        // if there is any, pick one at random and discover it
        if (undisLore.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, undisLore.Count);
            DiscoverEntry(undisLore[index]);
        }
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
        foreach (JounralEntry entry in undiscoveredEntries)
        {
            data.undiscoveredEntries.Add(new JounralEntry.SerializableJournalEntry(entry));
        }
        foreach (JounralEntry entry in discoveredEntries)
        {
            data.discoveredEntries.Add(new JounralEntry.SerializableJournalEntry(entry));
        }
        

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
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }

        // if the file doesn't exist, create it
        if (!File.Exists(GetSaveFilePath(saveSlot)))
        {
            File.Create(GetSaveFilePath(saveSlot));
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
        undiscoveredEntries.Clear();
        discoveredEntries.Clear();

        // add the data to the lists
        foreach (JounralEntry.SerializableJournalEntry entry in data.undiscoveredEntries)
        {
            undiscoveredEntries.Add(entry.ToEntry());
        }
        foreach (JounralEntry.SerializableJournalEntry entry in data.discoveredEntries)
        {
            discoveredEntries.Add(entry.ToEntry());
        }

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
        return journalPanel.activeSelf;
    }
    public override void OpenWindow()
    {
        base.OpenWindow();
        journalPanel.SetActive(true);
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
                if (!myScript.discoveredEntries.Contains(entry) && !myScript.undiscoveredEntries.Contains(entry))
                {
                    myScript.undiscoveredEntries.Add(entry);
                }
            }

            // set dirty
            EditorUtility.SetDirty(myScript);
        }
    }
    #endif
}
