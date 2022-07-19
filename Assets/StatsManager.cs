using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Reflection;
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

    [Serializable]
    public enum StatType{
        RangedDamage,
        ShotgunDamage
    }

    [Serializable]
    public enum ModType{
        Additive,
        Multiplier
    }

    [Serializable]
    public class StatMod{
        [SerializeField] public StatType statType;
        [SerializeField] public ModType modType;
        [SerializeField] public float value;
    }

    public interface UsesStats{
        List<StatType> GetStatTypes();
    }

    public interface HasStatMods{
        List<StatMod> GetStatMods();
    }

    [SerializeField] static public List<StatMod> globalStatMods = new List<StatMod>()
    {

    };

    //default list of invetory IDS must be given as pararam
    public static List<StatMod> GetPlayerInvetoryStatMods(){
        //empty list
        List<StatMod> statMods = new List<StatMod>();

        // get inv manager
        InventoryManager invManager = InventoryManager.instance;
        if (!invManager) return statMods;

        // get player inventory
        Inventory playerInventory = invManager.GetInventory("player");
        if (!playerInventory) return statMods;

        // get all items in inventory, check if they have stat mods, if so add them to list
        List<Item> items = playerInventory.GetItems();
        foreach (Item item in items)
        {
            if (item is HasStatMods)
            {
                statMods.AddRange((item as HasStatMods).GetStatMods());
            }
        }

        return statMods;
    }

    public static List<StatMod> GetAllStatMods(){
        List<StatMod> allStatMods = new List<StatMod>();
        allStatMods.AddRange(globalStatMods);
        allStatMods.AddRange(GetPlayerInvetoryStatMods());
        return allStatMods;
    }

    static public float CalculateDamage(UsesStats statUser, float baseDamage = 1.0f){
        float damage = baseDamage;
        float damageAdditive = 0.0f;
        float damageMultiplier = 1.0f;

        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.RangedDamage,
            StatType.ShotgunDamage
        };

        // get all stat mods
        List<StatMod> statMods = GetAllStatMods();

        // loop through all stat mods and add them to the damage
        foreach (StatMod statMod in statMods)
        {
            // if stat type is in list of stat types to pay attention to
            if (usedStatTypes.Contains(statMod.statType))
            {
                // if stat mod is additive
                if (statMod.modType == ModType.Additive)
                {
                    damageAdditive += statMod.value;
                }
                // if stat mod is multiplier
                else if (statMod.modType == ModType.Multiplier)
                {
                    damageMultiplier *= statMod.value;
                }
            }
        }

        // add additive damage
        damage += damageAdditive;
        // multiply damage
        damage *= damageMultiplier;

        return damage;
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
