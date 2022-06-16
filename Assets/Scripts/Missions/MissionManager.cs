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

    [SerializeField] public List<Mission> lesserMissionList;
    [SerializeField] public List<Mission> greaterMissionList;
    [SerializeField] public Mission currentMission;

    public static string GetSaveFolderPath(int saveSlot = 0)
    {
        return Application.dataPath + "/saves/save" + saveSlot + "/missions/";
    }

    public static string GetSaveFilePath(int saveSlot = 0)
    {
        return GetSaveFolderPath(saveSlot) + "missions.json";
    }

    /// <summary>
    /// Serializable version of the mission class from Mission.cs
    /// </summary>
    [Serializable]
    public class SerializableMission{
        [SerializeField] public Mission.MissionSize size;

        [SerializeField] public Mission.MissionZone zone;

        [SerializeField] public Mission.MissionType type;

        [SerializeField] public string title;

        [SerializeField] public string description;

        [SerializeField] public bool isCompleted;

        //constructor that converts the mission to a serializable mission
        public SerializableMission(Mission mission){
            size = mission.size;
            zone = mission.zone;
            type = mission.type;
            title = mission.title;
            description = mission.description;
            isCompleted = mission.isCompleted;
        }

        //converts the serializable mission to a mission
        public Mission ToMission(){
            Mission mission = ScriptableObject.CreateInstance<Mission>();
            mission.size = size;
            mission.zone = zone;
            mission.type = type;
            mission.title = title;
            mission.description = description;
            mission.isCompleted = isCompleted;
            return mission;
        }
    }

    /// <summary>
    /// Serializable class that holds the mission data to be saved
    /// </summary>
    [Serializable]
    public class MissionData{
        [SerializeField] public List<SerializableMission> lesserMissionList;
        [SerializeField] public List<SerializableMission> greaterMissionList;

        //current
        [SerializeField] public Mission.MissionSize currentMissionSize = Mission.MissionSize.LESSER;
        [SerializeField] public int currentMissionIndex = -1;
    }

    void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadMissions();
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ReinstantiateMissions();

        UpdateAllMissionCards();
    }

    // Update is called once per frame
    void Update()
    {
        //DEBUG
        //keypad + to save
        if (Keyboard.current.numpadPlusKey.wasPressedThisFrame)
        {
            SaveMissions();
        }
        //keypad - to load
        if (Keyboard.current.numpadMinusKey.wasPressedThisFrame)
        {
            LoadMissions();
        }
        //keypad 9 to delete save
        if (Keyboard.current.numpad9Key.wasPressedThisFrame)
        {
            DeleteMissionSave();
        }
    }

    /// <summary>
    /// Serialize the missions and save them to file
    /// </summary>
    public void SaveMissions(){
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath()))
        {
            Directory.CreateDirectory(GetSaveFolderPath());
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(GetSaveFilePath());

        MissionData data = new MissionData();

        //copy lesser mission list to serializable mission list
        data.lesserMissionList = new List<SerializableMission>();
        foreach (Mission m in lesserMissionList)
        {
            SerializableMission sm = new SerializableMission(m);
            data.lesserMissionList.Add(sm);
        }

        //copy greater mission list to serializable mission list
        data.greaterMissionList = new List<SerializableMission>();
        foreach (Mission m in greaterMissionList)
        {
            SerializableMission sm = new SerializableMission(m);
            data.greaterMissionList.Add(sm);
        }

        if (currentMission){
            data.currentMissionSize = currentMission.size;
            data.currentMissionIndex = GetMissionIndex(currentMission);
        }

        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Deserialize the missions from file and load them
    /// </summary>
    public void LoadMissions(){
        if (File.Exists(GetSaveFilePath())){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(GetSaveFilePath(), FileMode.Open);

            MissionData data = (MissionData)bf.Deserialize(file);
            file.Close();

            //copy lesser serializable mission list to mission list
            lesserMissionList = new List<Mission>();
            foreach (SerializableMission sm in data.lesserMissionList)
            {
                Mission m = sm.ToMission();
                lesserMissionList.Add(m);
            }

            //copy greater serializable mission list to mission list
            greaterMissionList = new List<Mission>();
            foreach (SerializableMission sm in data.greaterMissionList)
            {
                Mission m = sm.ToMission();
                greaterMissionList.Add(m);
            }

            currentMission = GetMission(data.currentMissionSize, data.currentMissionIndex);
        }

        UpdateAllMissionCards();
    }

    public void DeleteMissionSave(){
        if (File.Exists(GetSaveFilePath())){
            File.Delete(GetSaveFilePath());
        }
    }

    /// <summary>
    /// Replace missions in list with new copies of the missions
    /// </summary>
    public void ReinstantiateMissions(){
        //store current mission info
        Mission.MissionSize currentSize = Mission.MissionSize.LESSER;
        int currentIndex = -1;
        if (currentMission){
            currentSize = currentMission.size;
            currentIndex = GetMissionIndex(currentMission);
        }

        //lesser
        for (int i = 0; i < lesserMissionList.Count; i++)
        {
            lesserMissionList[i] = Instantiate(lesserMissionList[i]);
        }

        //greater
        for (int i = 0; i < greaterMissionList.Count; i++)
        {
            greaterMissionList[i] = Instantiate(greaterMissionList[i]);
        }

        //set current mission
        currentMission = GetMission(currentSize, currentIndex);
    }

    public void RestartMissions(){
        //loop through lesser missions and set them to not completed
        foreach (Mission m in lesserMissionList)
        {
            m.isCompleted = false;
        }

        //loop through greater missions and set them to not completed
        foreach (Mission m in greaterMissionList)
        {
            m.isCompleted = false;
        }

        //set current mission to null
        currentMission = null;

        UpdateAllMissionCards();
    }

    /// <summary>
    /// Try to accept and start the given mission
    /// </summary>
    public void TryStartMission(Mission mission){
        if (mission == null) return;

        //check if mission is in list
        int lesserIndex = lesserMissionList.IndexOf(mission);
        int greaterIndex = greaterMissionList.IndexOf(mission);
        if (lesserIndex == -1 && greaterIndex == -1) return;

        currentMission = mission;

        UpdateAllMissionCards();
    }

    /// <summary>
    /// Try to return the mission
    /// </summary>
    public void TryReturnMission(){
        currentMission = null;

        UpdateAllMissionCards();
    }

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

    public Mission GetMission(Mission.MissionSize _size, int _index){
        switch (_size)
        {
            case Mission.MissionSize.LESSER:
                //check if index is valid
                if (_index < 0 || _index >= lesserMissionList.Count) return null;
                return lesserMissionList[_index];
            case Mission.MissionSize.GREATER:
                //check if index is valid
                if (_index < 0 || _index >= greaterMissionList.Count) return null;
                return greaterMissionList[_index];
            default:
                return null;
        }
    }

    public int GetMissionIndex(Mission _mission){
        switch (_mission.size)
        {
            case Mission.MissionSize.LESSER:
                return lesserMissionList.IndexOf(_mission);
            case Mission.MissionSize.GREATER:
                return greaterMissionList.IndexOf(_mission);
            default:
                return -1;
        }
    }

    public bool WithinRange(int index){
        return index >= 0 && index < lesserMissionList.Count;
    }
}