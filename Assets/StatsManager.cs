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

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Singleton donotdestroy script that handles the stats system
/// </summary>
public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;

    /// <summary>
    /// Class for a single statistic. <br/>
    /// IMPORTANT: Stats are NOT meant to be instantiated at runtime. <br/>
    /// </summary>
    [Serializable]
    public class Stat
    {
        public string name = "";

        public float baseValue = 0;
        public float calculatedValue = 0;
        public float calculatedFlatMod = 0;
        public float calculatedPercentMod = 0;

        public float maxValue = Mathf.Infinity;
        public float minValue = Mathf.NegativeInfinity;
    }

    // perhaps 2 classes here, base stats, and modifiable stats. (base: e.g. gold, simple values that need to be saved, with custom functions, modifiable: e.g. damage, which can be modified by items, etc.)
    // then a class for modifiers that have flat and percent modifiers.
    // keep track of all modifier objects in a list.
    // keep track of modifiers that affect specific stats, within the stats?

    public class Gold : Stat
    {
        
    }
    Gold gold = new Gold();
    
    [Serializable]
    public class StatModifier
    {

    }

    private void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadStats();
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    public static string GetSaveFolderPath(int saveSlot = 0)
    {
        return Application.dataPath + "/saves/save" + saveSlot + "/stats/";
    }

    public static string GetSaveFilePath(int saveSlot = 0)
    {
        return GetSaveFolderPath(saveSlot) + "stats.json";
    }

    public void SaveStats()
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath()))
        {
            Directory.CreateDirectory(GetSaveFolderPath());
        }

        FileStream file = File.Create(GetSaveFilePath());

        // write the json to the file
        StreamWriter writer = new StreamWriter(file);
        writer.Write(JsonUtility.ToJson(this));
        writer.Close();
        file.Close();
    }


    public void LoadStats()
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
        reader.Close();
        file.Close();

        // parse the json
        JsonUtility.FromJsonOverwrite(json, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
