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
public class StatsManager : MonoBehaviour  /// @todo comment
{
    public static StatsManager instance;

    [Serializable]
    public class StatType
    {
        private StatType(string _value)
        {
            value = _value;
        }

        public string value;

        // Items
        public static StatType RangedDamage = new StatType("RangedDamage");
        public static StatType RangedInaccuracy = new StatType("RangedInaccuracy");
        public static StatType RangedRange = new StatType("RangedRange");
        public static StatType RangedAimSpeed = new StatType("RangedAimSpeed");
        public static StatType ShotgunDamage = new StatType("ShotgunDamage");
        public static StatType PistolDamage = new StatType("PistolDamage");
        public static StatType RifleDamage = new StatType("RifleDamage");

        // Economy
        public static StatType StoreCost = new StatType("StoreCost");
        public static StatType ItemCost = new StatType("ItemCost");

        public static String DisplayName(StatType type)
        {
            //add a space before each capital letter
            string displayName = "";
            foreach (char c in type.value)
            {
                if (char.IsUpper(c))
                {
                    displayName += " ";
                }
                displayName += c;
            }
            return displayName;
        }
    }

    public interface UsesStats
    {
        List<StatType> GetStatTypes();
    }

    public interface HasStatMods
    {
        List<StatMod> GetStatMods();
    }

    [Serializable]
    public enum ModType
    {
        Additive,
        Multiplier
    }

    [Serializable]
    public class StatMod
    {
        [SerializeField] public StatType statType = null;
        [SerializeField] public ModType modType = ModType.Additive;
        [SerializeField] public float value = 0;
    }
#if UNITY_EDITOR
    static public bool DrawStatMod(StatMod statMod, bool doHoriz = true)
    {
        if (statMod == null)
        {
            return false;
        }

        if (doHoriz) EditorGUILayout.BeginHorizontal();

        // get all static fields of the StatType class
        FieldInfo[] fields = typeof(StatType).GetFields(BindingFlags.Public | BindingFlags.Static);
        //remove any which arent of type StatType
        fields = fields.Where(f => f.FieldType == typeof(StatType)).ToArray();

        // create a list of strings to display in the dropdown
        List<string> options = new List<string>();
        foreach (FieldInfo field in fields)
        {
            options.Add(field.Name);
        }

        // get the index of the selected option
        int selectedIndex = 0;
        if (statMod.statType != null) selectedIndex = options.IndexOf(statMod.statType.value);
        //check if index is within bounds
        if (selectedIndex < 0 || selectedIndex >= options.Count)
        {
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
    static public bool DrawStatModList(List<StatMod> statMods)
    {
        foreach (StatMod statMod in statMods)
        {
            EditorGUILayout.BeginHorizontal();
            DrawStatMod(statMod, false);
            if (GUILayout.Button("X"))
            {
                statMods.Remove(statMod);
                return true;
            }
            EditorGUILayout.EndHorizontal();
        }
        // add a new stat mod
        if (GUILayout.Button("Add Stat Mod"))
        {
            statMods.Add(new StatMod());
            return true;
        }

        return false;
    }
#endif

    [SerializeField]
    static public List<StatMod> globalStatMods = new List<StatMod>()
    {

    };

    //default list of invetory IDS must be given as pararam
    public static List<StatMod> GetPlayerInvetoryStatMods()
    {
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

    public static List<StatMod> GetAllStatMods()
    {
        List<StatMod> allStatMods = new List<StatMod>();
        allStatMods.AddRange(globalStatMods);
        allStatMods.AddRange(GetPlayerInvetoryStatMods());
        return allStatMods;
    }

    static public float CalculateDamage(UsesStats _statUser, float _baseDamage = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.RangedDamage,
            StatType.ShotgunDamage,
            StatType.PistolDamage,
            StatType.RifleDamage,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return GenericStatCalc(_statUser, _baseDamage, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public int CalculatePrice(UsesStats _statUser, int _basePrice = 1)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.StoreCost,
            StatType.ItemCost,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return (int)GenericStatCalc(_statUser, _basePrice, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    private static float GenericStatCalc(UsesStats _statUser, float _baseVal, List<StatType> _usedStatTypes, float _additiveVal = 0.0f, float _multiplierVal = 1.0f, float _minVal = 0.0f, float _maxVal = float.MaxValue)
    {
        float baseVal = _baseVal;
        float additiveVal = _additiveVal;
        float multiplierVal = _multiplierVal;

        float minVal = _minVal;
        float maxVal = _maxVal;

        CalcMods(_statUser, ref additiveVal, ref multiplierVal, _usedStatTypes);

        // add additive damage
        baseVal += additiveVal;
        // multiply damage
        baseVal *= multiplierVal;

        // clamp damage
        baseVal = Mathf.Clamp(baseVal, minVal, maxVal);
        return baseVal;
    }

    private static void CalcMods(UsesStats _statUser, ref float additiveVal, ref float dmultiplierVal, List<StatType> _usedStatTypes)
    {
        // get all stat mods
        List<StatMod> statMods = GetAllStatMods();

        // loop through all statUser's stat types
        foreach (StatType statType in _statUser.GetStatTypes())
        {
            // if stat type is in the list of used stat types
            if (_usedStatTypes.Any(x => x.value == statType.value))
            {
                // loop through all stat mods
                foreach (StatMod statMod in statMods)
                {
                    // if stat mod is for this stat type
                    if (statMod.statType.value == statType.value)
                    {
                        // if mod is additive
                        if (statMod.modType == ModType.Additive)
                        {
                            additiveVal += statMod.value;
                        }
                        // if mod is multiplier
                        else if (statMod.modType == ModType.Multiplier)
                        {
                            dmultiplierVal *= statMod.value;
                        }
                    }
                }
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadStats(SaveManager.currentSaveSlot);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    public static string GetSaveFolderPath(int saveSlot)
    {
        return SaveManager.GetSaveFolderPath(saveSlot) + "/stats/";
    }

    public static string GetSaveFilePath(int saveSlot)
    {
        return GetSaveFolderPath(saveSlot) + "stats.json";
    }

    public void SaveStats(int saveSlot)
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }

        FileStream file = File.Create(GetSaveFilePath(saveSlot));

        // write the json to the file
        StreamWriter writer = new StreamWriter(file);
        writer.Write(JsonUtility.ToJson(this));
        writer.Close();
        file.Close();
    }


    public void LoadStats(int saveSlot)
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }

        // if the file doesn't exist, create it
        if (!File.Exists(GetSaveFilePath(saveSlot)))
        {
            File.Create(GetSaveFilePath(saveSlot));
            return;
        }

        // read the file
        FileStream file = File.Open(GetSaveFilePath(saveSlot), FileMode.Open);

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
