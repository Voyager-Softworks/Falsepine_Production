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


    ////////// STATS //////////
    #region Stats

    /// <summary>
    /// An ENUM replacement for the types
    /// </summary>
    [Serializable]
    public class StatType
    {
        private StatType(string _value)
        {
            value = _value;
        }

        public string value;

        // Damage + Items
        public static StatType PlayerDamage { get { return new StatType("PlayerDamage"); } }
        public static StatType RangedDamage { get { return new StatType("RangedDamage"); } }
        public static StatType RangedInaccuracy { get { return new StatType("RangedInaccuracy"); } }
        public static StatType RangedRange { get { return new StatType("RangedRange"); } }
        public static StatType RangedAimSpeed { get { return new StatType("RangedAimSpeed"); } }
        public static StatType ShotgunDamage { get { return new StatType("ShotgunDamage"); } }
        public static StatType PistolDamage { get { return new StatType("PistolDamage"); } }
        public static StatType RifleDamage { get { return new StatType("RifleDamage"); } }
        public static StatType ExplosiveDamage { get { return new StatType("ExplosiveDamage"); } }
        public static StatType TrapDamage { get { return new StatType("TrapDamage"); } }
        public static StatType EnemyDamage { get { return new StatType("EnemyDamage"); } }
        public static StatType MeleeDamage { get { return new StatType("MeleeDamage"); } }

        // Economy
        public static StatType ItemCost { get { return new StatType("ItemCost"); } }

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

        public static StatType StringToStatType(string _value)
        {
            MethodInfo[] methods = GetAllStatTypeMethods();

            // loop through all methods
            foreach (MethodInfo method in methods)
            {
                // get the value of the method
                StatType type = (StatType)method.Invoke(null, null);
                // if the value matches the one we are looking for, return it
                if (type.value == _value)
                {
                    return type;
                }
            }
            // if we get here, the type was not found
            return null;
        }

        public static MethodInfo[] GetAllStatTypeMethods()
        {
            // get all static methods of the StatType class
            MethodInfo[] methods = typeof(StatType).GetMethods(BindingFlags.Public | BindingFlags.Static);
            // remove any which arent of type StatType
            methods = methods.Where(f => f.ReturnType == typeof(StatType)).ToArray();
            // remove any which take parameters
            methods = methods.Where(f => f.GetParameters().Length == 0).ToArray();
            // sort alphabetically
            methods = methods.OrderBy(f => ((StatType)f.Invoke(null, null)).value).ToArray();
            return methods;
        }

        // equality == override
        public static bool operator ==(StatType a, StatType b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.value == b.value;
        }
        // inequality != override
        public static bool operator !=(StatType a, StatType b)
        {
            return !(a == b);
        }
        // Equals override
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            StatType p = obj as StatType;
            if ((System.Object)p == null)
            {
                return false;
            }
            return (value == p.value);
        }
        // GetHashCode override
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }

    /// <summary>
    /// Interface for a class that makes use of stats
    /// </summary>
    public interface UsesStats
    {
        List<StatType> GetStatTypes();
        void AddStatType(StatType type);
        void RemoveStatType(StatType type);
    }
    #endregion

    ////////// MODS //////////
    #region Mods

    [Serializable]
    public enum ModType
    {
        Additive,
        Multiplier
    }

    /// <summary>
    /// Used to modify a stat
    /// </summary>
    [Serializable]
    public class StatMod
    {
        [SerializeField] public StatType statType = null;
        [SerializeField] public ModType modType = ModType.Additive;
        [SerializeField] public float value = 0;

        public string ToText(int _decimalPlaces = 2){
            string text = StatsManager.StatType.DisplayName(statType) + " " + (modType == StatsManager.ModType.Multiplier ? "x" : (value < 0 ? "" : "+")) + value.ToString("F" + _decimalPlaces);
            return text;
        }
    }

    /// <summary>
    /// Interface for a class that modifies stats
    /// </summary>
    public interface HasStatMods
    {
        List<StatMod> GetStatMods();
    }

    /// <summary>
    /// A talisman modifies stats, and is kept for the whole run. <br/>
    /// Only mods ONE stat type.
    /// </summary>
    [Serializable]
    public class Talisman : StatsManager.HasStatMods
    {
        // HasStatMods interface implementation
        public StatsManager.StatMod m_statMod = new StatsManager.StatMod();
        public List<StatsManager.StatMod> GetStatMods()
        {
            return new List<StatsManager.StatMod>() { m_statMod };
        }

        public Sprite m_icon = null;
    }
    #endregion

    ////////// TALISMANS //////////
    #region Talisman
    
    /// <summary>
    /// Stat mod range stores data to be used to create a random stat mod
    /// </summary>    
    [Serializable]
    public class StatModRange
    {
        /// <summary>
        /// Do not edit this value! Automatically set by the editor.
        /// </summary>
        [SerializeField][HideInInspector] public string m_name = "name";
        [SerializeField] public StatType m_statType = null;
        [SerializeField] public ModType m_modType = ModType.Additive;
        [SerializeField] public float m_min = 0;
        [SerializeField] public float m_max = 0;
        [SerializeField] public Sprite m_icon = null;
    }
    [SerializeField] private List<StatModRange> m_possibleTalismanMods = new List<StatModRange>();
    public List<Talisman> m_activeTalismans = new List<Talisman>();

    public Talisman GetRandomTalisman()
    {
        StatModRange modRange = m_possibleTalismanMods[UnityEngine.Random.Range(0, m_possibleTalismanMods.Count)];
        Talisman talisman = new Talisman();
        talisman.m_statMod.statType = modRange.m_statType;
        talisman.m_statMod.modType = modRange.m_modType;
        talisman.m_statMod.value = UnityEngine.Random.Range(modRange.m_min, modRange.m_max);
        talisman.m_icon = modRange.m_icon;
        return talisman;
    }
    #endregion

    ////////// Utility //////////
    #region utility functions

    /// <summary>
    /// Converts a float into a signed string with specific DP.
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_decimalPlaces"></param>
    /// <returns></returns>
    public static string SignedFloatString(float _value, int _decimalPlaces = 2)
    {
        string text = "";

        // add sign
        if (_value < 0)
        {
            text += "-";
        }
        else if (_value > 0)
        {
            text += "+";
        }
        // add value
        text += Mathf.Abs(_value).ToString("F" + _decimalPlaces);

        return text;
    }

    #endregion

    ////////// Enemy Stats //////////
    #region Enemy Stats
    
    [Serializable]
    public class MonsterStat{
        public MonsterInfo m_monster;
        public List<Health_Base.DamageStat> m_kills = new List<Health_Base.DamageStat>();
    }
    [SerializeField] public List<MonsterStat> m_monsterStats = new List<MonsterStat>();

    public void AddKill(MonsterInfo _monster, Health_Base.DamageStat _damageStat)
    {
        MonsterStat stats = m_monsterStats.Find(x => x.m_monster == _monster);
        if (stats == null)
        {
            stats = new MonsterStat();
            stats.m_monster = _monster;
            m_monsterStats.Add(stats);
        }

        // if damage stat is null, make a new one
        if (_damageStat == null)
        {
            _damageStat = new Health_Base.DamageStat(0, this.gameObject, Vector3.zero, Vector3.zero, null);
        }

        stats.m_kills.Add(_damageStat);
    }

    public int GetKills(MonsterInfo _monster)
    {
        MonsterStat stats = m_monsterStats.Find(x => x.m_monster == _monster);
        if (stats == null)
        {
            return -1;
        }
        return stats.m_kills.Count;
    }

    #endregion

    [SerializeField]
    static public List<StatMod> globalStatMods = new List<StatMod>()
    {
    };

    /// <summary>
    /// Gets the stat mods from the player inventory
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Gets all active stat mods
    /// </summary>
    /// <returns></returns>
    public static List<StatMod> GetAllStatMods()
    {
        List<StatMod> allStatMods = new List<StatMod>();
        allStatMods.AddRange(globalStatMods);
        allStatMods.AddRange(GetPlayerInvetoryStatMods());
        // get talisman mods
        foreach (Talisman talisman in instance.m_activeTalismans)
        {
            allStatMods.AddRange(talisman.GetStatMods());
        }
        return allStatMods;
    }

    /// <summary>
    /// Uses damage stats to calculate damage
    /// </summary>
    /// <param name="_statUser"></param>
    /// <param name="_baseDamage"></param>
    /// <returns></returns>
    static public float CalculateDamage(UsesStats _statUser, float _baseDamage = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.PlayerDamage,
            StatType.RangedDamage,
            StatType.ShotgunDamage,
            StatType.PistolDamage,
            StatType.RifleDamage,
            StatType.ExplosiveDamage,
            StatType.TrapDamage,
            StatType.EnemyDamage,
            StatType.MeleeDamage,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return GenericStatCalc(_statUser, _baseDamage, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public float CalculateRange(UsesStats _statUser, float _baseRange = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.RangedRange,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return GenericStatCalc(_statUser, _baseRange, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    /// <summary>
    /// Uses economy stats to calculate cost
    /// </summary>
    /// <param name="_statUser"></param>
    /// <param name="_basePrice"></param>
    /// <returns></returns>
    static public int CalculatePrice(UsesStats _statUser, int _basePrice = 1)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.ItemCost,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return (int)GenericStatCalc(_statUser, _basePrice, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    /// <summary>
    /// The foundational stat calculation function
    /// </summary>
    /// <param name="_statUser"></param>
    /// <param name="_baseVal"></param>
    /// <param name="_usedStatTypes"></param>
    /// <param name="_additiveVal"></param>
    /// <param name="_multiplierVal"></param>
    /// <param name="_minVal"></param>
    /// <param name="_maxVal"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Calculates the effect of all stat mods on a stat
    /// </summary>
    /// <param name="_statUser"></param>
    /// <param name="additiveVal"></param>
    /// <param name="dmultiplierVal"></param>
    /// <param name="_usedStatTypes"></param>
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

    public static string GetRecentDeathFolderPath(int saveSlot)
    {
        return SaveManager.GetRecentDeathFolderPath(saveSlot) + "/stats/";
    }

    public static string GetSaveFilePath(int saveSlot)
    {
        return GetSaveFolderPath(saveSlot) + "stats.json";
    }

    public static string GetRecentDeathFilePath(int saveSlot)
    {
        return GetRecentDeathFolderPath(saveSlot) + "stats.json";
    }

    /// <summary>
    /// Data calss for saving stats to file
    /// </summary>
    [Serializable]
    public class SaveData{
        public List<StatMod> globalStatMods = new List<StatMod>();
        public List<StatModRange> possibleTalismanMods = new List<StatModRange>();
        public List<Talisman> activeTalismans = new List<Talisman>();
        public List<MonsterStat> monsterStats = new List<MonsterStat>();
    }

    public void SaveStats(int saveSlot)
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }

        FileStream file = File.Create(GetSaveFilePath(saveSlot));

        // make data
        SaveData data = new SaveData();
        
        // add global stat mods
        data.globalStatMods = new List<StatMod>(globalStatMods);
        // add possible talisman mods
        data.possibleTalismanMods = new List<StatModRange>(m_possibleTalismanMods);
        // add active talismans
        data.activeTalismans = new List<Talisman>(m_activeTalismans);
        // add monster stats
        data.monsterStats = new List<MonsterStat>(m_monsterStats);

        // convert data to json
        string json = JsonUtility.ToJson(data);

        // write json to file
        StreamWriter writer = new StreamWriter(file);

        writer.Write(json);

        writer.Close();

        file.Close();
    }


    public void LoadStats(int saveSlot)
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

        // read the file
        FileStream file = File.Open(GetSaveFilePath(saveSlot), FileMode.Open);

        StreamReader reader = new StreamReader(file);

        string json = reader.ReadToEnd();

        reader.Close();

        file.Close();

        // convert json to data
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // load global stat mods
        globalStatMods = new List<StatMod>(data.globalStatMods);
        // load possible talisman mods
        m_possibleTalismanMods = new List<StatModRange>(data.possibleTalismanMods);
        // load active talismans
        m_activeTalismans = new List<Talisman>(data.activeTalismans);
        // load monster stats
        m_monsterStats = new List<MonsterStat>(data.monsterStats);
    }

    /// <summary>
    /// Resotres some stats from the last death (only restores possible talisman mods and monster stats)
    /// </summary>
    /// <param name="_saveSlot"></param>
    public void RestoreFromLastDeath(int _saveSlot)
    {
        // if recent death path doesnt exist, return
        if (!Directory.Exists(GetRecentDeathFolderPath(_saveSlot)))
        {
            Debug.Log("Recent death folder does not exist.");
            return;
        }

        // if recent death file doesnt exist, return
        if (!File.Exists(GetRecentDeathFilePath(_saveSlot)))
        {
            Debug.Log("Recent death file does not exist.");
            return;
        }

        // read the file
        FileStream file = File.Open(GetRecentDeathFilePath(_saveSlot), FileMode.Open);

        StreamReader reader = new StreamReader(file);

        string json = reader.ReadToEnd();

        reader.Close();

        file.Close();

        // convert json to data
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // load possible talisman mods
        m_possibleTalismanMods = new List<StatModRange>(data.possibleTalismanMods);
        // load monster stats
        m_monsterStats = new List<MonsterStat>(data.monsterStats);

        // save
        SaveStats(_saveSlot);
    }

    /// <summary>
    /// Deletes the save at index
    /// </summary>
    /// <param name="saveSlot"></param>
    public void DeleteStatsSave(int saveSlot)
    {
        if (File.Exists(GetSaveFilePath(saveSlot)))
        {
            File.Delete(GetSaveFilePath(saveSlot));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    // custom editor for this class
    [CustomEditor(typeof(StatsManager))]
    public class StatsManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            StatsManager statsManager = (StatsManager)target;

            // add missing monsters button
            if (GUILayout.Button("Add Missing Monsters"))
            {
                statsManager.AddAllMonsterInfo();
            }
        }
    }

    /// <summary>
    /// Adds all missing monster info to the list of monster stats
    /// </summary>
    public void AddAllMonsterInfo(){
        // get all monster info scriptable objects in the resources
        MonsterInfo[] monsterInfos = Resources.LoadAll<MonsterInfo>("");
        // loop through all monster info
        foreach (MonsterInfo monsterInfo in monsterInfos)
        {
            // if the monster info is not in the list of monster stats
            if (!m_monsterStats.Any(x => x.m_monster == monsterInfo))
            {
                // make new monster stat
                MonsterStat monsterStat = new MonsterStat();
                monsterStat.m_monster = monsterInfo;

                // add the monster info to the list
                m_monsterStats.Add((monsterStat));
            }
            
        }
    } 

    private void OnValidate() {
        foreach(StatModRange statModRange in m_possibleTalismanMods)
        {
            statModRange.m_name = StatType.DisplayName(statModRange.m_statType) + " - " + statModRange.m_modType.ToString();
        }
    }

    /// <summary>
    /// A property drawer for the StatType class which acts like an enum! Very cool :)
    /// </summary>
    [CustomPropertyDrawer(typeof(StatType))]
    public class StatTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //EditorGUI.BeginProperty(position, GUIContent.none, property);

            // get all static methods of the StatType class
            MethodInfo[] methods = StatType.GetAllStatTypeMethods();
            // create a list of strings to display in the dropdown
            List<GUIContent> options = new List<GUIContent>();
            foreach (MethodInfo method in methods)
            {
                options.Add(new GUIContent(((StatType)method.Invoke(null, null)).value));
            }

            // get the property value
            SerializedProperty value = property.FindPropertyRelative("value").Copy();
            EditorGUI.BeginProperty(position, GUIContent.none, value);

            // get the index of the selected option
            int selectedIndex = -1;
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].text == value.stringValue)
                {
                    selectedIndex = i;
                    break;
                }
            }
            //check if index is within bounds
            if (selectedIndex < 0 || selectedIndex >= options.Count)
            {
                // set the selected option
                selectedIndex = 0;
            }
            // draw the dropdown
            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, options.ToArray());
            string name = ((StatType)methods[selectedIndex].Invoke(null, null)).value;

            value.stringValue = name;

            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// Used to draw a list of StatMods in the inspector. <br/>
    /// Primarily used by the Item class.
    /// </summary>
    /// <param name="statMods"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Used specifically for the DrawStatModList function!
    /// </summary>
    /// <param name="statMod"></param>
    /// <param name="doHoriz"></param>
    /// <returns></returns>
    static private bool DrawStatMod(StatMod statMod, bool doHoriz = true)
    {
        if (statMod == null)
        {
            return false;
        }

        if (doHoriz) EditorGUILayout.BeginHorizontal();

        // get all static fields of the StatType class
        MethodInfo[] methods = StatType.GetAllStatTypeMethods();

        // create a list of strings to display in the dropdown
        List<string> options = new List<string>();
        foreach (MethodInfo method in methods)
        {
            options.Add(((StatType)method.Invoke(null, null)).value);
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
        // get the selected option
        statMod.statType = (StatType)methods[selectedIndex].Invoke(null, null);

        statMod.modType = (ModType)EditorGUILayout.EnumPopup(statMod.modType);
        statMod.value = EditorGUILayout.FloatField(statMod.value);

        if (doHoriz) EditorGUILayout.EndHorizontal();

        return false;
    }

    /// <summary>
    /// A custom function that draws a custom popup and display for a list of statTypes. <br/>
    /// Primarily used by the Item class.
    /// </summary>
    /// <param name="_target"></param>
    public static void StatTypeListDropdown(StatsManager.UsesStats _target, out bool needToSave)
    {
        // target
        StatsManager.UsesStats target = _target;

        needToSave = false;

        // used stat types, multi select dropdown (generic menu)
        GUILayout.BeginHorizontal();
        string usedStatTypes = "Used Stat Types: ";
        for (int i = 0; i < _target.GetStatTypes().Count; i++)
        {
            usedStatTypes += _target.GetStatTypes()[i].value;
            // if not last item, add comma and/or new line
            if (i != _target.GetStatTypes().Count - 1)
            {
                usedStatTypes += ", ";

                // every third item, add a new line
                if (i % 3 == 2)
                {
                    usedStatTypes += "\n";
                }
            }
        }
        if (_target.GetStatTypes().Count <= 0)
        {
            usedStatTypes += "None";
        }
        //GUILayout.Label(new GUIContent(usedStatTypes, "The stat types used by this item"));
        if (GUILayout.Button(new GUIContent(usedStatTypes, "Stat Types this item should use")))
        {
            GenericMenu menu = new GenericMenu();

            needToSave = true;

            // get all static methods of the StatType class
            MethodInfo[] methods = StatType.GetAllStatTypeMethods();
            foreach (MethodInfo method in methods)
            {
                StatsManager.StatType type = null;
                // if field is not static, return
                if (!method.IsStatic) continue;
                type = ((StatType)method.Invoke(null, null));
                if (_target.GetStatTypes().Count() > 0)
                {
                    StatsManager.StatType firstTpe = _target.GetStatTypes()[0];
                }

                // check if it is used, by comparing the value of the field to the used stat types
                bool isUsed = _target.GetStatTypes().Any(x => x.value == type.value);

                menu.AddItem(new GUIContent(type.value), isUsed, () =>
                {
                    if (!isUsed)
                    {
                        _target.AddStatType(type);

                        //save
                        // if not in play mode, save (set dirty)
                        if (!Application.isPlaying)
                        {
                            // save param is set earlier, this is only here if code needs to be changed back
                        }
                    }
                    else
                    {
                        _target.RemoveStatType(type);

                        //save
                        // if not in play mode, save (set dirty)
                        if (!Application.isPlaying)
                        {
                            // save param is set earlier, this is only here if code needs to be changed back
                        }
                    }
                });
            }

            // show menu
            menu.ShowAsContext();
        }
        GUILayout.EndHorizontal();
    }
#endif
}
