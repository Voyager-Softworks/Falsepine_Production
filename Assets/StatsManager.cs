using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Linq;
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

    // public enum StatType{
    //     RangedDamage = 0,
    //     RangedInaccuracy = 1,
    //     RangedRange = 2,
    //     RangedAimSpeed = 6,
    //     ShotgunDamage = 3,
    //     PistolDamage = 4,
    //     RifleDamage = 5,
    // }
    [Serializable]
    public class StatType
    {
        private StatType(string _value){
            value = _value;
        }

        public string value;

        public static StatType RangedDamage         = new StatType("RangedDamage");
        public static StatType RangedInaccuracy     = new StatType("RangedInaccuracy");
        public static StatType RangedRange          = new StatType("RangedRange");
        public static StatType RangedAimSpeed       = new StatType("RangedAimSpeed");
        public static StatType ShotgunDamage        = new StatType("ShotgunDamage");
        public static StatType PistolDamage         = new StatType("PistolDamage");
        public static StatType RifleDamage          = new StatType("RifleDamage");
    }

    [Serializable]
    public enum ModType{
        Additive,
        Multiplier
    }

    [Serializable]
    public class StatMod{
        [SerializeField] public StatType statType = null;
        [SerializeField] public ModType modType = ModType.Additive;
        [SerializeField] public float value = 0;
    }
    #if UNITY_EDITOR
    static public bool DrawStatMod(StatMod statMod, bool doHoriz = true){
        if (statMod == null) {
            return false;
        }

        if (doHoriz) EditorGUILayout.BeginHorizontal();

        // get all static fields of the StatType class
        FieldInfo[] fields = typeof(StatType).GetFields(BindingFlags.Public | BindingFlags.Static);
        //remove any which arent of type StatType
        fields = fields.Where(f => f.FieldType == typeof(StatType)).ToArray();

        // create a list of strings to display in the dropdown
        List<string> options = new List<string>();
        foreach (FieldInfo field in fields) {
            options.Add(field.Name);
        }

        // get the index of the selected option
        int selectedIndex = 0;
        if (statMod.statType != null) selectedIndex = options.IndexOf(statMod.statType.value);
        //check if index is within bounds
        if (selectedIndex < 0 || selectedIndex >= options.Count) {
            // set the selected option
            selectedIndex = 0;
        }
        // draw the dropdown
        selectedIndex = EditorGUILayout.Popup(selectedIndex, options.ToArray());
        string test = fields[selectedIndex].Name;
        // get the selected option
        statMod.statType = (StatType)fields[selectedIndex].GetValue(null);

        statMod.modType = (ModType)EditorGUILayout.EnumPopup(statMod.modType);
        statMod.value = EditorGUILayout.FloatField(statMod.value);
        
        if (doHoriz) EditorGUILayout.EndHorizontal();

        return false;
    }

    static public bool DrawStatModList(List<StatMod> statMods){
        foreach (StatMod statMod in statMods) {
            EditorGUILayout.BeginHorizontal();
            DrawStatMod(statMod, false);
            if (GUILayout.Button("X")) {
                statMods.Remove(statMod);
                return true;
            }
            EditorGUILayout.EndHorizontal();
        }
        // add a new stat mod
        if (GUILayout.Button("Add Stat Mod")) {
            statMods.Add(new StatMod());
            return true;
        }

        return false;
    }
    #endif

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

        float minDamage = 0.0f;
        float maxDamage = float.MaxValue;

        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.RangedDamage,
            StatType.ShotgunDamage,
            StatType.PistolDamage,
            StatType.RifleDamage,
        };

        // get all stat mods
        List<StatMod> statMods = GetAllStatMods();

        // loop through all stat mods and add them to the damage
        foreach (StatMod statMod in statMods)
        {
            // if stat type is in list of stat types to pay attention to
            if (usedStatTypes.Any(x => x.value == statMod.statType.value))
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

        // clamp damage
        damage = Mathf.Clamp(damage, minDamage, maxDamage);

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
