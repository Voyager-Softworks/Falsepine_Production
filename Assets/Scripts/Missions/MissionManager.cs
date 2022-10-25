using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton donotdestroy script that handles the mission system
/// </summary>
public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

    [SerializeField] public List<MissionZone> m_missionZones;
    [SerializeField] public MissionZone m_currentZone;

    public Utilities.SceneField TownSceneReference;

    [Header("Loot Drops")]
    public LootPool m_easyLootPool;
    public LootPool m_mediumLootPool;
    public LootPool m_hardLootPool;

    public static string GetSaveFolderPath(int saveSlot)
    {
        return SaveManager.GetSaveFolderPath(saveSlot) + "/missions/";
    }

    public static string GetSaveFilePath(int saveSlot)
    {
        return GetSaveFolderPath(saveSlot) + "missions.json";
    }

    /// <summary>
    /// Serializable class that holds the mission data to be saved
    /// </summary>
    [Serializable]
    public class MissionData
    {
        [SerializeField] public List<MissionZone.Serializable_MissionZone> missionZones;
        [SerializeField] public MissionZone.Serializable_MissionZone currentZone;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadMissions(SaveManager.currentSaveSlot);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        // if instance is not this, return
        if (instance != this)
        {
            return;
        }

        SetUpMissionGroups();

        // call start function on all conditions
        if (GetCurrentMission() != null)
        {
            GetCurrentMission().OnSceneLoaded(arg0, arg1);
        }

        // if this scene is the town scene, end the mission
        if (arg0 == SceneManager.GetSceneByPath(TownSceneReference))
        {
            GetCurrentMission()?.EndMission();
        }

        // disable/enable journal pickups based on MissionZone pickup indexes
        CullJournalPickups();
    }

    private void CullJournalPickups()
    {
        // if already found a specific pickup, try replace with money
        foreach (JournalPickupInteract pickup in FindObjectsOfType<JournalPickupInteract>()){
            // if not specific, or if specific and not found, continue
            if (pickup.m_pickupType != JournalPickupInteract.PickupType.SpecificEntry || pickup.m_linkedEntry == null){
                continue;
            }

            // look for money replacement money
            Transform parent = pickup.transform.parent;
            MoneyPickup moneyPickup = null;
            if (parent != null)
            {
                moneyPickup = parent.GetComponentInChildren<MoneyPickup>();
            }

            if (pickup.m_linkedEntry && JournalManager.instance.m_discoveredEntries.Contains(pickup.m_linkedEntry)){
                SetPickupState(pickup, moneyPickup, false);
            }
        }

        if (m_currentZone != null)
        {
            // get the current middle scene from the mission zone
            string currentScenePath = SceneManager.GetActiveScene().path;
            Utilities.SceneField currentMiddleScene = m_currentZone.m_middleScenes.Find(x => currentScenePath.Contains(x.scenePath));
            if (currentMiddleScene != null)
            {
                // get the index of the current middle scene
                int currentMiddleSceneIndex = m_currentZone.m_middleScenes.IndexOf(currentMiddleScene);

                foreach (JournalPickupInteract pickup in FindObjectsOfType<JournalPickupInteract>())
                {
                    // look for money replacement money
                    Transform parent = pickup.transform.parent;
                    MoneyPickup moneyPickup = null;
                    if (parent != null)
                    {
                        moneyPickup = parent.GetComponentInChildren<MoneyPickup>();
                    }

                    // if current scene should have clue or lore, and the pickup is a clue or lore, enable it
                    if ((m_currentZone.m_clueSceneIndexes.Contains(currentMiddleSceneIndex) && pickup.GetEntryType() == JounralEntry.EntryType.Clue) ||
                        (m_currentZone.m_loreSceneIndexes.Contains(currentMiddleSceneIndex) && pickup.GetEntryType() == JounralEntry.EntryType.Lore))
                    {
                        SetPickupState(pickup, moneyPickup, true);
                    }
                    else
                    {
                        SetPickupState(pickup, moneyPickup, false);
                    }
                }
            }
        }
    }

    private static void SetPickupState(JournalPickupInteract pickup, MoneyPickup moneyPickup, bool _pickupEnabled)
    {
        pickup.gameObject.SetActive(_pickupEnabled);

        if (moneyPickup != null)
        {
            moneyPickup.gameObject.SetActive(!_pickupEnabled);
        }
    }

    /// <summary>
    /// Enables/Disables the mission groups based on the current mission
    /// </summary>
    private void SetUpMissionGroups()
    {
        // find all MissionGroup objects in the scene
        MissionGroup[] missionGroups = FindObjectsOfType<MissionGroup>(true);

        Mission currentM = GetCurrentMission();
        if (currentM == null) return;

        // enable/disable corrent ones
        foreach (MissionGroup missionGroup in missionGroups)
        {
            missionGroup.gameObject.SetActive(false);

            if (missionGroup.m_linkedMission == currentM)
            {
                missionGroup.gameObject.SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ReinstantiateZones();

        //if there is no current zone, set it to the first one and reset it
        if (m_currentZone == null)
        {
            m_currentZone = m_missionZones[0];
            m_currentZone.Reset();
        }
        else
        {
            // if there are no missions in the current zone, reset it
            if (m_currentZone.m_missions.Count == 0)
            {
                m_currentZone.Reset();
            }
        }

        UpdateAllMissionCards();
    }


    // Update is called once per frame
    void Update()
    {
        // call update function on all conditions
        if (GetCurrentMission() != null)
        {
            GetCurrentMission().Update();
        }
    }

    /// <summary>
    /// Saves the mission data to json file
    /// </summary>
    public void SaveMissions(int saveSlot)
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }

        FileStream file = File.Create(GetSaveFilePath(saveSlot));

        MissionData data = new MissionData();

        // Populate Data:
        data.missionZones = new List<MissionZone.Serializable_MissionZone>();
        foreach (MissionZone zone in m_missionZones)
        {
            data.missionZones.Add(new MissionZone.Serializable_MissionZone(zone));
        }
        data.currentZone = new MissionZone.Serializable_MissionZone(m_currentZone);

        //json serialize the data
        string json = JsonUtility.ToJson(data, true);

        StreamWriter writer = new StreamWriter(file);
        writer.Write(json);
        writer.Close();

        file.Close();
    }

    /// <summary>
    /// Deserialize the missions from file and load them
    /// </summary>
    public void LoadMissions(int saveSlot)
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

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(GetSaveFilePath(saveSlot), FileMode.Open);

        MissionData data = new MissionData();

        // unpack data
        string json = "";
        using (StreamReader reader = new StreamReader(file))
        {
            json = reader.ReadToEnd();
        }
        data = JsonUtility.FromJson<MissionData>(json);

        file.Close();

        // load data
        if (data == null)
        {
            return;
        }
        m_missionZones = new List<MissionZone>();
        foreach (MissionZone.Serializable_MissionZone zone in data.missionZones)
        {
            m_missionZones.Add(zone.ToMissionZone());
        }
        //make temp zone
        MissionZone tempZone = data.currentZone.ToMissionZone();

        //find the zone in the list
        foreach (MissionZone zone in m_missionZones)
        {
            if (zone == tempZone)
            {
                m_currentZone = zone;
                break;
            }
        }

        UpdateAllMissionCards();
    }

    /// <summary>
    /// Tries to set the current zone to the next one
    /// </summary>
    public void GoToNextZone()
    {
        // get current zone index
        int index = m_missionZones.IndexOf(m_currentZone);

        // if there is a next zone, set it to the current zone
        if (index + 1 < m_missionZones.Count)
        {
            TryReturnMission();
            m_currentZone = m_missionZones[index + 1];
            m_currentZone.Reset();
        }
        else
        {
            Debug.Log("No next zone");

            // set to first zone
            if (m_missionZones.Count > 0)
            {
                StatsManager.instance.ResetPreviousKills();
                m_currentZone = m_missionZones[0];
                m_currentZone.Reset();
            }
        }
    }

    /// <summary>
    /// Deletes the save file
    /// </summary>
    /// <param name="saveSlot"></param>
    public void DeleteMissionSave(int saveSlot)
    {
        if (File.Exists(GetSaveFilePath(saveSlot)))
        {
            File.Delete(GetSaveFilePath(saveSlot));
        }
    }

    /// <summary>
    /// Replace missions in list with new copies of the missions
    /// </summary>
    public void ReinstantiateZones()
    {
        // reinstatedate zones
        for (int i = 0; i < m_missionZones.Count; i++)
        {
            m_missionZones[i] = Instantiate(m_missionZones[i]);
        }

        // reinstantiate all missions in each zone first
        foreach (MissionZone zone in m_missionZones)
        {
            zone.ReinstantiateMissions();
        }

        // store current zone info
        int currentZoneIndex = GetZoneIndex(m_currentZone);

        m_missionZones = new List<MissionZone>(m_missionZones);

        m_currentZone = GetZone(currentZoneIndex);
    }

    /// <summary>
    /// Restarts and randomizes all zones.
    /// </summary>
    public void ResetAllZones()
    {
        // reset all zones
        foreach (MissionZone zone in m_missionZones)
        {
            zone.Reset();
        }

        // set current zone to first zone
        m_currentZone = GetZone(0);

        UpdateAllMissionCards();
    }

    /// <summary>
    /// Try to accept and start the given mission
    /// </summary>
    public void TryStartMission(Mission mission)
    {
        if (mission == null) return;
        if (m_currentZone == null) return;

        if (m_currentZone.TryStartMission(mission))
        {
            UpdateAllMissionCards();

            // sound
            UIAudioManager.instance?.acceptSound.Play();
        }
    }

    /// <summary>
    /// Tries to embark to new scene with current mission
    /// </summary>
    public void TryEmbark()
    {
        // heal
        if (StatsManager.instance){
            StatsManager.instance.m_playerCurrentHealth = StatsManager.instance.m_calcedPlayerMaxHealth;
        }

        // begin mission if it exists
        GetCurrentMission()?.BeginMission();

        LoadFirstScene();

        // sound
        UIAudioManager.instance?.embarkSound.Play();
    }

    /// <summary>
    /// Loads the first scene from list
    /// </summary>
    public void LoadFirstScene()
    {
        if (m_currentZone == null) return;
        if (m_currentZone.GetSceneList().Count <= 0) return;

        LevelController.LoadScene(m_currentZone.GetSceneList()[0]);
    }

    /// <summary>
    /// Loads the next scene in the list
    /// </summary>
    public void LoadNextScene()
    {
        if (m_currentZone == null) return;

        string nextPath = m_currentZone.GetNextScenePath();
        if (nextPath == "") return;

        LevelController.LoadScene(nextPath);
    }

    public void LoadCinematicScene()
    {
        if (m_currentZone == null) return;

        string cinematicPath = m_currentZone.m_cinematicScene?.scenePath;
        if (cinematicPath == "") return;

        LevelController.LoadScene(cinematicPath);
    }

    public void LoadBossScene()
    {
        if (m_currentZone == null) return;

        string bossPath = m_currentZone.m_bossScene?.scenePath;
        if (bossPath == "") return;

        LevelController.LoadScene(bossPath);
    }

    /// <summary>
    /// Try to return the current mission
    /// </summary>
    public void TryReturnMission()
    {
        if (m_currentZone == null) return;

        if (m_currentZone.TryReturnMission())
        {
            UpdateAllMissionCards();
        }
    }

    public LootPool GetLootPool(Mission.Difficulty _difficulty)
    {
        switch (_difficulty)
        {
            case Mission.Difficulty.Easy:
                return m_easyLootPool;
            case Mission.Difficulty.Medium:
                return m_mediumLootPool;
            case Mission.Difficulty.Hard:
                return m_hardLootPool;
            default:
                Debug.LogError("Invalid difficulty for loot pool", this);
                return null;
        }
    }

    /// <summary>
    /// Updates all mission cards.
    /// </summary>
    public void UpdateAllMissionCards()
    {
        MissionCardUI[] missionCardUIList = FindObjectsOfType<MissionCardUI>(true);

        if (missionCardUIList == null) return;

        for (int i = 0; i < missionCardUIList.Length; i++)
        {
            MissionCardUI missionCardUI = missionCardUIList[i];
            if (missionCardUI == null) continue;

            missionCardUI.UpdateCardUI();
        }
    }

    /// <summary>
    /// Checks if the current zone's boss has been killed at least once
    /// </summary>
    /// <returns></returns>
    public bool HasZoneBossDied(){
        foreach (MonsterInfo monster in GetCurrentZone()?.m_zoneMonsters){
            if (monster != null && monster.m_type == MonsterInfo.MonsterType.Boss && StatsManager.instance?.GetCurrentKills(monster) > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the current zone
    /// </summary>
    /// <returns></returns>
    public MissionZone GetCurrentZone(){
        return m_currentZone;
    }

    /// <summary>
    /// Gets the index of a zone in the list
    /// </summary>
    /// <param name="_zone"></param>
    /// <returns></returns>
    public int GetZoneIndex(MissionZone _zone)
    {
        return m_missionZones.IndexOf(_zone);
    }

    /// <summary>
    /// Gets the index of the current zone
    /// </summary>
    /// <returns></returns>
    public int GetCurrentZoneIndex()
    {
        return m_missionZones.IndexOf(GetCurrentZone());
    }

    /// <summary>
    /// Gets the zone of index
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public MissionZone GetZone(int _index)
    {
        if (_index < 0 || _index >= m_missionZones.Count) return null;
        return m_missionZones[_index];
    }

    public MissionZone GetZone(MissionZone.ZoneArea _area)
    {
        foreach (MissionZone zone in m_missionZones)
        {
            if (zone.m_area == _area) return zone;
        }
        return null;
    }

    /// <summary>
    /// Gets the missions of the current zone
    /// </summary>
    /// <returns></returns>
    public List<Mission> GetMissions()
    {
        if (m_currentZone == null) return new List<Mission>();

        return m_currentZone.m_missions;
    }

    /// <summary>
    /// Gets the current mission of the current zone
    /// </summary>
    /// <returns></returns>
    public Mission GetCurrentMission()
    {
        if (m_currentZone == null) return null;

        return m_currentZone.currentMission;
    }

    /// <summary>
    /// Gets the mission from the current zone with the specified size and index)
    /// </summary>
    /// <param name="_size"></param>
    /// <param name="_index"></param>
    /// <returns></returns>
    public Mission GetMission(int _index)
    {
        if (m_currentZone == null) return null;

        return m_currentZone.GetMission(_index);
    }

    public int GetCurrentSceneIndex()
    {
        if (m_currentZone == null) return -1;

        return m_currentZone.GetCurrentSceneIndex();
    }
}