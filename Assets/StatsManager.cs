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

    public List<Stat> allStats = new List<Stat>();
    public List<Modifier> allModifiers = new List<Modifier>();

    // stat class with be used to build subclasses.
    // subclasses will be used to keep track of specific things (up to dev), for example ( ShotgunStats: Damage, Accuracy, Rate of Fire, etc. )

    // modifier class with be used to build subclasses.
    // subclasses will be used to keep track of specific things (up to dev), for example ( ShotgunModifier: Damage, Accuracy, Rate of Fire, etc. )

    /// <summary>
    /// Class for a single basic statistic. <br/>
    /// IMPORTANT: Stats are NOT meant to be instantiated at runtime. <br/>
    /// </summary>
    [Serializable]
    public class Stat
    {
        public string name = "";

        public float baseValue = 0;
        // ? public float calculatedValue = 0;

        public float maxValue = Mathf.Infinity;
        public float minValue = Mathf.NegativeInfinity;
    }

    /// <summary>
    /// Class for a modifiable statistic. <br/>
    /// IMPORTANT: Stats are NOT meant to be instantiated at runtime. <br/>
    /// </summary>
    [Serializable]
    public class ModifiableStat : Stat
    {
        public List<Modifier> modifiers = new List<Modifier>();
    }

    /// <summary>
    /// Class for a single modifier. <br/>
    /// </summary>
    [Serializable]
    public class Modifier
    {
        public string name = "";
        public float value = 0;
        public float flatValue = 0;
        public float percentValue = 0;
    }



    [Serializable]
    public class Gold : Stat
    {
        
    }
    [SerializeField] Gold gold = new Gold();
    
    [Serializable]
    public class StatModifier
    {

    }

    unsafe private void Awake() {
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
