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

/// <summary>
/// Singleton donotdestroy script that handles the mission system
/// </summary>
public class MissionManager : MonoBehaviour 
{
    public static MissionManager instance;

    [SerializeField] public List<MissionZone> m_missionZones;
    [SerializeField] public MissionZone m_currentZone;

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
    public class MissionData{
        [SerializeField] public List<MissionZone.Serializable_MissionZone> missionZones;
        [SerializeField] public MissionZone.Serializable_MissionZone currentZone;
    }

    void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadMissions(SaveManager.currentSaveSlot);
        } else {
            Destroy(this);
            Destroy(gameObject);
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
        else {
            // if there are no lesser or greater missions in the current zone, reset it
            if (m_currentZone.m_lesserMissions.Count == 0 && m_currentZone.m_greaterMissions.Count == 0)
            {
                m_currentZone.Reset();
            }
        }

        UpdateAllMissionCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Saves the mission data to json file
    /// </summary>
    public void SaveMissions(int saveSlot){
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
    public void LoadMissions(int saveSlot){
        if (File.Exists(GetSaveFilePath(saveSlot))){
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
        }

        UpdateAllMissionCards();
    }

    public void DeleteMissionSave(int saveSlot){
        if (File.Exists(GetSaveFilePath(saveSlot))){
            File.Delete(GetSaveFilePath(saveSlot));
        }
    }

    /// <summary>
    /// Replace missions in list with new copies of the missions
    /// </summary>
    public void ReinstantiateZones(){
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
    public void ResetAllZones(){
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
    public void TryStartMission(Mission mission){
        if (mission == null) return;
        if (m_currentZone == null) return;

        if (m_currentZone.TryStartMission(mission)){
            UpdateAllMissionCards();
        }
    }

    public void TryEmbark(){
        //load level 1 if valid
        if (GetCurrentMission() != null && !GetCurrentMission().m_isCompleted){
            if (GetCurrentMission().m_size == Mission.MissionSize.LESSER){
                LoadFirstLesserScene();
            }
            else if (GetCurrentMission().m_size == Mission.MissionSize.GREATER){
                LoadFirstGreaterScene();
            }
        }
    }

    public void LoadFirstLesserScene(){
        if (m_currentZone == null) return;
        if (m_currentZone.m_startScene == null) return;

        LevelController.LoadScene(m_currentZone.m_startScene.scenePath);
    }

    public void LoadNextLesserScene(){
        if (m_currentZone == null) return;

        string nextPath = m_currentZone.GetNextLesserScenePath();
        if (nextPath == "") return;

        LevelController.LoadScene(nextPath);
    }

    public void LoadFirstGreaterScene(){
        if (m_currentZone == null) return;
        if (m_currentZone.m_startScene == null) return;

        LevelController.LoadScene(m_currentZone.m_bossScene.scenePath);
    }

    /// <summary>
    /// Try to return the mission
    /// </summary>
    public void TryReturnMission(){
        if (m_currentZone == null) return;

        if (m_currentZone.TryReturnMission()){
            UpdateAllMissionCards();
        }
    }

    /// <summary>
    /// Updates all mission cards.
    /// </summary>
    public void UpdateAllMissionCards(){
        MissionCardUI[] missionCardUIList = FindObjectsOfType<MissionCardUI>(true);

        if (missionCardUIList == null) return;

        for (int i = 0; i < missionCardUIList.Length; i++)
        {
            MissionCardUI missionCardUI = missionCardUIList[i];
            if (missionCardUI == null) continue;

            missionCardUI.UpdateCard();
        }
    }

    /// <summary>
    /// Gets the index of a zone in the list
    /// </summary>
    /// <param name="_zone"></param>
    /// <returns></returns>
    public int GetZoneIndex(MissionZone _zone){
        return m_missionZones.IndexOf(_zone);
    }

    /// <summary>
    /// Gets the zone of index
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public MissionZone GetZone(int _index){
        if (_index < 0 || _index >= m_missionZones.Count) return null;
        return m_missionZones[_index];
    }

    /// <summary>
    /// Gets the lesser missions of the current zone
    /// </summary>
    /// <returns></returns>
    public List<Mission> GetLesserMissions(){
        if (m_currentZone == null) return null;

        return m_currentZone.m_lesserMissions;
    }

    /// <summary>
    /// Gets the greater missions of the current zone
    /// </summary>
    /// <returns></returns>
    public List<Mission> GetGreaterMissions(){
        if (m_currentZone == null) return null;

        return m_currentZone.m_greaterMissions;
    }

    /// <summary>
    /// Gets the current mission of the current zone
    /// </summary>
    /// <returns></returns>
    public Mission GetCurrentMission(){
        if (m_currentZone == null) return null;

        return m_currentZone.currentMission;
    }

    /// <summary>
    /// Gets the mission from the current zone with the specified size and index)
    /// </summary>
    /// <param name="_size"></param>
    /// <param name="_index"></param>
    /// <returns></returns>
    public Mission GetMission(Mission.MissionSize _size, int _index){
        if (m_currentZone == null) return null;

        return m_currentZone.GetMission(_size, _index);
    }

    public int GetCurrentLesserSceneIndex(){
        if (m_currentZone == null) return -1;

        return m_currentZone.GetCurrentLesserSceneIndex();
    }

    // /// <summary>
    // /// Gets the path of the current scene
    // /// </summary>
    // /// <returns></returns>
    // public string GetCurrentScenePath(){
    //     if (m_currentZone == null) return "";
    //     return m_currentZone.GetCurrentScenePath();
    // }

    // /// <summary>
    // /// Gets the path of the next scene
    // /// </summary>
    // /// <returns></returns>
    // public string GetNextScenePath(){
    //     if (m_currentZone == null) return "";
    //     return m_currentZone.GetNextScenePath();
    // }
}