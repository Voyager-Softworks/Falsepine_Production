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

    // stat class with be used to build subclasses.
    // subclasses will be used to keep track of specific things (up to dev), for example ( Damage, Accuracy, Rate of Fire, etc. )

    // stat groups will 

    // need to make an interface for different types of common stats, such as damage, accuracy, rate of fire, etc.


    // EXAMPLE:
    // StatType: Damage
    // StatType: Accuracy
    // etc.

    // StatGroup: WeaponStats
    //  WeaponStats: Damage, etc
    //  StatGroup: ShotgunStats
    // ShotgunStats: Damage, Accuracy, etc.

    /// <summary>
    /// Class for a single basic statistic. <br/>
    /// IMPORTANT: Stats are NOT meant to be instantiated at runtime. <br/>
    /// </summary>
    [Serializable]
    public class StatGroup
    {
    }

    public interface StatType
    {
    }

    public interface Damage : StatType
    {
        float GetFlatDamage();
        float GetMultiplierDamage();
    }
    /// <summary>
    /// Calculates the damage of a weapon from a list of stat groups.
    /// </summary>
    /// <param name="statGroups"></param>
    /// <param name="baseDamage"></param>
    /// <returns></returns>
    static public float CalculateDamage(List<StatGroup> statGroups, float baseDamage = 0)
    {
        float damage = baseDamage;
        float flatDamage = 0;
        float multiplierDamage = 0;
        foreach (StatGroup statGroup in statGroups)
        {
            if (statGroup is Damage)
            {
                Damage damageStat = (Damage)statGroup;
                flatDamage += damageStat.GetFlatDamage();
                multiplierDamage += damageStat.GetMultiplierDamage();
            }
        }
        damage += flatDamage;
        damage *= multiplierDamage;
        return damage;
    }

    public class RangedWeapon : StatGroup, Damage
    {
        public float flatDamage;
        public float multiplierDamage;

        public float GetFlatDamage()
        {
            return flatDamage;
        }
        public float GetMultiplierDamage()
        {
            return multiplierDamage;
        }
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
