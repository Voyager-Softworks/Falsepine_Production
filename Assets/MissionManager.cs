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

    [SerializeField] public List<Mission> missionList;
    [SerializeField] public int currentMissionIndex;

    /// <summary>
    /// Serializable version of the mission class from Mission.cs
    /// </summary>
    [Serializable]
    public class SerializableMission{

        [SerializeField] public string title;

        [SerializeField] public string description;

        [SerializeField] public bool isCompleted;
    }

    /// <summary>
    /// Serializable class that holds the mission data to be saved
    /// </summary>
    [Serializable]
    public class MissionData{
        [SerializeField] public List<SerializableMission> missionList;
        [SerializeField] public int currentMissionIndex;
    }

    void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ReinstantiateMissions();
    }

    // Update is called once per frame
    void Update()
    {
        //if pressed 0,1,2, go to that scene
        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(0);
        }
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(1);
        }
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(2);
        }

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
    }

    /// <summary>
    /// Serialize the missions and save them to file
    /// </summary>
    public void SaveMissions(){
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/missions.dat");

        MissionData data = new MissionData();

        //copy mission list to serializable mission list
        data.missionList = new List<SerializableMission>();
        foreach (Mission m in missionList)
        {
            SerializableMission sm = new SerializableMission();
            sm.title = m.title;
            sm.description = m.description;
            sm.isCompleted = m.isCompleted;
            data.missionList.Add(sm);
        }

        data.currentMissionIndex = currentMissionIndex;

        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Deserialize the missions from file and load them
    /// </summary>
    public void LoadMissions(){
        if (File.Exists(Application.persistentDataPath + "/missions.dat")){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/missions.dat", FileMode.Open);

            MissionData data = (MissionData)bf.Deserialize(file);
            file.Close();

            //copy serializable mission list to mission list
            missionList = new List<Mission>();
            foreach (SerializableMission sm in data.missionList)
            {
                Mission m = ScriptableObject.CreateInstance<Mission>();
                m.title = sm.title;
                m.description = sm.description;
                m.isCompleted = sm.isCompleted;
                missionList.Add(m);
            }

            currentMissionIndex = data.currentMissionIndex;
        }

        UpdateAllMissionCards();
    }

    /// <summary>
    /// Replace missions in list with new copies of the missions
    /// </summary>
    public void ReinstantiateMissions(){
        for (int i = 0; i < missionList.Count; i++)
        {
            missionList[i] = Instantiate(missionList[i]);
        }
    }

    /// <summary>
    /// Try to accept and start the given mission
    /// </summary>
    public void TryStartMission(Mission mission){
        if (mission == null) return;

        int missionIndex = missionList.IndexOf(mission);

        if (missionIndex == -1) return;

        currentMissionIndex = missionIndex;

        UpdateAllMissionCards();
    }

    public void UpdateAllMissionCards(){
        MissionCardUI[] missionCardUIList = FindObjectsOfType<MissionCardUI>(true);

        if (missionCardUIList == null) return;

        for (int i = 0; i < missionCardUIList.Length; i++)
        {
            MissionCardUI missionCardUI = missionCardUIList[i];
            if (missionCardUI == null) continue;

            missionCardUI.UpdateUI();
        }
    }
}