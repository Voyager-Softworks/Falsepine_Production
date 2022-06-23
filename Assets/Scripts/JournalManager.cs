using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Singleton donotdestroy script that handles the journal system
/// </summary>
[Serializable]
public class JournalManager : MonoBehaviour
{
    public enum InfoType
    {
        Lore,
        Clue
    }

    public static JournalManager instance;

    [Header("Keys")]
    public InputAction openJournalAction;
    public InputAction closeAction;

    [Header("Sounds")]
    public AudioClip openJournalSound;
    public AudioClip closeJournalSound;
    public AudioClip addInfoSound;

    [Header("References")]
    public GameObject journalPanel;
    
    private AudioSource _audioSource;

    [Serializable]
    public class MonsterClues
    {
        public string monsterName = "";
        public UI_MonsterClues monsterCluesUI;
        public int loreFound;
        public int cluesFound;

        [Serializable]
        public class Data
        {
            [SerializeField] public string monsterName = "";
            [SerializeField] public int loreFound;
            [SerializeField] public int cluesFound;
        }
    }
    [SerializeField] public List<MonsterClues> monsterCluesList = new List<MonsterClues>();

    public static string GetSaveFolderPath(int saveSlot = 0)
    {
        return Application.dataPath + "/saves/save" + saveSlot + "/journal/";
    }

    public static string GetSaveFilePath(int saveSlot = 0)
    {
        return GetSaveFolderPath(saveSlot) + "clues.json";
    }

    private void Awake()
    {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadJournal();
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        openJournalAction.performed += ctx => ToggleJournal();
        closeAction.performed += ctx => CloseJournal();
    }

    void OnEnable()
    {
        openJournalAction.Enable();
        closeAction.Enable();
    }

    void OnDisable()
    {
        openJournalAction.Disable();
        closeAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // update the monster clues
        foreach (MonsterClues clue in monsterCluesList) {
            if (clue.monsterCluesUI != null) {
                for (int i = 0; i < clue.monsterCluesUI.lore.Count; i++) {
                    if (i < clue.loreFound) {
                        clue.monsterCluesUI.lore[i].transform.GetChild(0).gameObject.SetActive(true);
                        clue.monsterCluesUI.lore[i].transform.GetChild(1).gameObject.SetActive(false);
                    } else {
                        clue.monsterCluesUI.lore[i].transform.GetChild(0).gameObject.SetActive(false);
                        clue.monsterCluesUI.lore[i].transform.GetChild(1).gameObject.SetActive(true);
                    }
                }

                for (int i = 0; i < clue.monsterCluesUI.clue.Count; i++) {
                    if (i < clue.cluesFound) {
                        clue.monsterCluesUI.clue[i].transform.GetChild(0).gameObject.SetActive(true);
                        clue.monsterCluesUI.clue[i].transform.GetChild(1).gameObject.SetActive(false);
                    } else {
                        clue.monsterCluesUI.clue[i].transform.GetChild(0).gameObject.SetActive(false);
                        clue.monsterCluesUI.clue[i].transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void AddInfo(JournalManager.InfoType infoType, string monsterName, string infoName)
    {
        // find the monster clues
        MonsterClues clues = monsterCluesList.Find(x => x.monsterName == monsterName);

        if (clues != null) {
            // add the info to the monster clues
            if (infoType == InfoType.Lore) {
                clues.loreFound++;
            } else if (infoType == InfoType.Clue) {
                clues.cluesFound++;
            }

            // play the add info sound
            if (addInfoSound && _audioSource) _audioSource.PlayOneShot(addInfoSound);
        }
    }


    /// <summary>
    /// Save the journal
    /// </summary>
    public void SaveJournal()
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath()))
        {
            Directory.CreateDirectory(GetSaveFolderPath());
        }

        FileStream file = File.Create(GetSaveFilePath());

        // create list of data
        List<MonsterClues.Data> data = new List<MonsterClues.Data>();
        foreach (MonsterClues clues in monsterCluesList)
        {
            MonsterClues.Data newData = new MonsterClues.Data();
            newData.monsterName = clues.monsterName;
            newData.loreFound = clues.loreFound;
            newData.cluesFound = clues.cluesFound;
            data.Add(newData);
        }

        StreamWriter writer = new StreamWriter(file);
        // for each data, write it to the file new line
        foreach (MonsterClues.Data d in data)
        {
            writer.WriteLine(JsonUtility.ToJson(d));
        }
        writer.Close();

        file.Close();
    }

    /// <summary>
    /// Load the journal
    /// </summary>
    public void LoadJournal()
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath()))
        {
            Directory.CreateDirectory(GetSaveFolderPath());
        }

        // if the file doesn't exist, create it
        if (!File.Exists(GetSaveFilePath()))
        {
            File.Create(GetSaveFilePath());
            return;
        }

        // read the file
        FileStream file = File.Open(GetSaveFilePath(), FileMode.Open);

        StreamReader reader = new StreamReader(file);
        string json = reader.ReadToEnd();
        
        // parse the json
        List<MonsterClues.Data> data = new List<MonsterClues.Data>();
        foreach (string line in json.Split('\n'))
        {
            if (line.Length > 0)
            {
                data.Add(JsonUtility.FromJson<MonsterClues.Data>(line));
            }
        }

        // update the monsterClues with cluesData
        foreach (MonsterClues mc in monsterCluesList)
        {
            //find the data for this monster
            MonsterClues.Data d = data.Find(x => x.monsterName == mc.monsterName);
            if (d != null)
            {
                mc.loreFound = d.loreFound;
                mc.cluesFound = d.cluesFound;
            }
        }


        reader.Close();
        file.Close();
    }

    public void UpdateJournalUI(){
        //update mission cards
        MissionManager missionManager = FindObjectOfType<MissionManager>();

        if (missionManager == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }
        else{
            missionManager.UpdateAllMissionCards();
        }
    }

    public void ToggleJournal()
    {
        if (journalPanel.activeSelf)
        {
            CloseJournal();
        }
        else
        {
            OpenJournal();
        }
    }

    public void OpenJournal()
    {
        if (journalPanel.activeSelf )return;

        journalPanel.SetActive(true);
        if (_audioSource && openJournalSound) _audioSource.PlayOneShot(openJournalSound);

        UpdateJournalUI();
    }

    public void CloseJournal()
    {
        if (!journalPanel.activeSelf) return;

        journalPanel.SetActive(false);
        if (_audioSource && closeJournalSound) _audioSource.PlayOneShot(closeJournalSound);
    }
}
