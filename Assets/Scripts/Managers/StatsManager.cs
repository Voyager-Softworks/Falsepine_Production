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
using UnityEngine.Serialization;
using Achievements;

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
        private StatType(string _value, string _displayName = "")
        {
            this.value = _value;
            this.displayName = _displayName;
            if (this.displayName == "")
            {
                string newDisplayName = GenerateDisplayName();

                this.displayName = newDisplayName;
            }
        }

        private string GenerateDisplayName()
        {
            //add a space before each capital letter
            string newDisplayName = "";
            int i = 0;
            foreach (char c in this.value)
            {
                if (char.IsUpper(c) && i != 0)
                {
                    newDisplayName += " ";
                }
                newDisplayName += c;

                i++;
            }

            return newDisplayName;
        }

        public string value;

        // Display name was NOT serialized, so it needs to be private and retrieved from this static class
        private string displayName;
        public String DisplayName()
        {
            // get the same statType from the static class, and return its display name
            StatType newInstance = StringToStatType(this.value);
            if (newInstance != null) {
                return newInstance.displayName;
            }
            else {
                Debug.LogError("StatType " + this.value + " not found in the static class!");

                // generate a new display name
                this.displayName = GenerateDisplayName();
                return this.displayName;
            }
        }

        // Damage + Items
        public static StatType PlayerDamage { get { return new StatType("PlayerDamage", "Hunter Damage"); } }
        public static StatType RangedDamage { get { return new StatType("RangedDamage"); } }
        public static StatType RangedInaccuracy { get { return new StatType("RangedInaccuracy", "Aim Inaccuracy"); } }
        public static StatType RangedRange { get { return new StatType("RangedRange", "Range"); } }
        public static StatType RangedAimTime { get { return new StatType("RangedAimTime", "Aim Time"); } }
        public static StatType ShotgunDamage { get { return new StatType("ShotgunDamage"); } }
        public static StatType PistolDamage { get { return new StatType("PistolDamage"); } }
        public static StatType RifleDamage { get { return new StatType("RifleDamage"); } }
        public static StatType SpecialDamage { get { return new StatType("SpecialDamage"); } }
        public static StatType ExplosiveDamage { get { return new StatType("ExplosiveDamage"); } }
        public static StatType TrapDamage { get { return new StatType("TrapDamage"); } }
        public static StatType EnemyDamage { get { return new StatType("EnemyDamage"); } }
        public static StatType MeleeDamage { get { return new StatType("MeleeDamage"); } }
        public static StatType PoisonDamage { get { return new StatType("PoisonDamage"); } }
        public static StatType FireDamage { get { return new StatType("FireDamage"); } }
        public static StatType SilverDamage { get { return new StatType("SilverDamage"); } }
        public static StatType SpareAmmo { get { return new StatType("SpareAmmo"); } }

        // Health
        public static StatType PlayerMaxHealth { get { return new StatType("PlayerMaxHealth", "Max Health"); } }
        public static StatType PlayerDamageTaken { get { return new StatType("PlayerDamageTaken", "Damage Taken"); } }
        public static StatType PlayerHealthSteal { get { return new StatType("PlayerHealthSteal", "Health Steal"); } }
        public static StatType PlayerHealAmount { get { return new StatType("PlayerHealAmount", "Heal Amount"); } }
        public static StatType EnemyMaxHealth { get { return new StatType("EnemyMaxHealth", "Creature Health"); } }
        public static StatType EnemyDamageTaken { get { return new StatType("EnemyDamageTaken", "Creature Health"); } }
        public static StatType BossMaxHealth { get { return new StatType("BossMaxHealth", "Boss Health"); } }
        public static StatType BossDamageTaken { get { return new StatType("BossDamageTaken"); } }

        // Economy
        public static StatType ItemCost { get { return new StatType("ItemCost"); } }
        public static StatType MoneyGain { get { return new StatType("MoneyGain"); } }

        public static StatType StringToStatType(string _value, bool _caseSensitive = false)
        {
            MethodInfo[] methods = GetAllStatTypeMethods();

            // loop through all methods
            foreach (MethodInfo method in methods)
            {
                // get the value of the method
                StatType type = (StatType)method.Invoke(null, null);
                // if the value matches the one we are looking for, return it
                if (_caseSensitive)
                {
                    if (type.value == _value)
                    {
                        return type;
                    }
                }
                else
                {
                    if (type.value.ToLower() == _value.ToLower())
                    {
                        return type;
                    }
                }
            }
            // if we get here, the type was not found
            return null;
        }

        // CompareTo method required for sorting
        public int CompareTo(StatType other)
        {
            if (other == null)
            {
                return 1;
            }
            return value.CompareTo(other.value);
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

    // a simple class that has a constructor from a list of stats to a UsesStats class
    private class TempStatUser : UsesStats
    {
        public List<StatType> stats = new List<StatType>();
        public List<StatType> GetStatTypes()
        {
            return stats;
        }
        public void AddStatType(StatType type)
        {
            stats.Add(type);
        }
        public void RemoveStatType(StatType type)
        {
            stats.Remove(type);
        }

        public TempStatUser(List<StatType> _stats)
        {
            stats = _stats;
        }
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
            string text = statType.DisplayName() + " " + (modType == StatsManager.ModType.Multiplier ? "x" : (value < 0 ? "" : "+")) + value.ToString("F" + _decimalPlaces);
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

    public List<Talisman> GetRandomTalismans(int _num = 1)
    {
        // create temp list of possible talismans
        List<StatModRange> tempPossible = new List<StatModRange>();
        // for each possible talisman mod
        foreach (StatModRange t in m_possibleTalismanMods){
            // if the stat type is not already in the list
            if (tempPossible.Find(f => f.m_statType == t.m_statType) == null){
                // add it
                tempPossible.Add(t);
            }
        }

        // create list of talismans to return
        List<Talisman> talismans = new List<Talisman>();

        // for each talisman to create
        for (int i = 0; i < _num; i++){
            // if there are no more possible talismans
            if (tempPossible.Count == 0){
                // break
                break;
            }
            // get a random talisman
            StatModRange mod = tempPossible[UnityEngine.Random.Range(0, tempPossible.Count)];
            // create a new talisman
            Talisman talisman = new Talisman();
            // set the stat mod
            talisman.m_statMod.statType = mod.m_statType;
            talisman.m_statMod.modType = mod.m_modType;
            talisman.m_statMod.value = UnityEngine.Random.Range(mod.m_min, mod.m_max);
            // set the icon
            talisman.m_icon = mod.m_icon;
            // add the talisman to the list
            talismans.Add(talisman);
            // remove the talisman from the list of possible talismans
            tempPossible.Remove(mod);
        }

        // return the list of talismans
        return talismans;
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
        [ReadOnly] public int m_previousKills = 0;

        [Serializable]
        public class SerializableMonsterStat{
            public MonsterInfo.SerializableMonsterInfo m_monster;
            public List<Health_Base.DamageStat> m_kills = new List<Health_Base.DamageStat>();
            [ReadOnly] public int m_previousKills = 0;

            public SerializableMonsterStat(MonsterStat _monsterStat){
                m_monster = new MonsterInfo.SerializableMonsterInfo(_monsterStat.m_monster);
                m_kills = _monsterStat.m_kills;
                m_previousKills = _monsterStat.m_previousKills;
            }

            public MonsterStat ToMonsterStat(){
                MonsterStat monsterStat = new MonsterStat();
                monsterStat.m_monster = m_monster.ToMonsterInfo();
                monsterStat.m_kills = m_kills;
                monsterStat.m_previousKills = m_previousKills;
                return monsterStat;
            }
        }
    }
    [SerializeField] public List<MonsterStat> m_monsterStats = new List<MonsterStat>();

    public int m_killsForClue = 20;
    public int m_maxCluesFromKills = 5;

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

        // every 20 kills, add a random clue for this enemy (up to 100 kills, must be lesser enemy)
        if (_monster != null && _monster.m_type == MonsterInfo.MonsterType.Minion && stats.m_kills.Count % m_killsForClue == 0 && stats.m_kills.Count <= m_killsForClue * m_maxCluesFromKills)
        {
            JournalManager.instance.DiscoverRandomEntry(_monster, JounralEntry.EntryType.Clue);
        }
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

    public int GetCurrentKills(MonsterInfo _monster)
    {
        MonsterStat stats = m_monsterStats.Find(x => x.m_monster == _monster);
        if (stats == null)
        {
            return -1;
        }
        return stats.m_kills.Count - stats.m_previousKills;
    }

    #endregion

    [SerializeField]
    static public List<StatMod> globalStatMods = new List<StatMod>()
    {
    };

    [SerializeField] static public List<Drink> activeDrinks = new List<Drink>();

    public float m_playerCurrentHealth = 100;
    public float m_playerMaxHealth = 100;
    public float m_calcedPlayerMaxHealth {
        get {
            // make a temp stat user
            TempStatUser tempUser = new TempStatUser(new List<StatType>() {
                StatType.PlayerMaxHealth
            });
            return StatsManager.CalculateMaxHealth(tempUser, m_playerMaxHealth);
        }
        set { 
            m_playerMaxHealth = value; 
        }
    }

    public List<Item> m_normalAchievementItems = new List<Item>();
    public List<Item> m_legendaryAchievementItems = new List<Item>(); 

    /// <summary>
    /// Marks an item as used for the achievement system.
    /// </summary>
    /// <param name="_item">The item to store in player prefs</param>
    public void AchievementItemOwned(Item _item)
    {
        if (_item == null) {
            return;
        }

        string key = "AchievementOwnedItem_" + _item.m_displayName;

        // if the item is already used, return
        if (PlayerPrefs.GetInt(key, 0) == 1)
        {
            return;
        }

        // otherwise, save the item as used
        if (PlayerPrefs.GetInt(key, 0) == 0)
        {
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }

        // check if all normal items have been used
        bool allNormalUsed = true;
        foreach (Item item in m_normalAchievementItems)
        {
            string normalKey = "AchievementOwnedItem_" + item.m_displayName;
            if (PlayerPrefs.GetInt(normalKey, 0) == 0)
            {
                allNormalUsed = false;
                break;
            }
        }
        if (allNormalUsed)
        {
            // unlock the achievement
            AchievementsManager.instance?.UnlockAchievement(AchievementsManager.Achievement.BuyAllWeapons);
        }

        // check if all legendary items have been used
        bool allLegendaryUsed = true;
        foreach (Item item in m_legendaryAchievementItems)
        {
            string legendaryKey = "AchievementOwnedItem_" + item.m_displayName;
            if (PlayerPrefs.GetInt(legendaryKey, 0) == 0)
            {
                allLegendaryUsed = false;
                break;
            }
        }
        if (allLegendaryUsed)
        {
            // unlock the achievement
            AchievementsManager.instance?.UnlockAchievement(AchievementsManager.Achievement.PurchaseAllWeapons);
        }
    }

    /// <summary>
    /// Deletes all of the player prefs relating to the item ownership achievements
    /// </summary> 
    public void ClearOwnedItems()
    {
        foreach (Item item in m_normalAchievementItems)
        {
            string key = "AchievementOwnedItem_" + item.m_displayName;
            PlayerPrefs.DeleteKey(key);
        }
        foreach (Item item in m_legendaryAchievementItems)
        {
            string key = "AchievementOwnedItem_" + item.m_displayName;
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Gets the stat mods from the player inventory
    /// </summary>
    /// <returns></returns>
    public static List<StatMod> GetPlayerInventoryStatMods()
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
        // add global
        allStatMods.AddRange(globalStatMods);
        // add drinks
        foreach (Drink drink in activeDrinks)
        {
            allStatMods.AddRange(drink.GetStatMods());
        }
        allStatMods.AddRange(GetPlayerInventoryStatMods());
        // get talisman mods
        foreach (Talisman talisman in instance.m_activeTalismans)
        {
            allStatMods.AddRange(talisman.GetStatMods());
        }
        return allStatMods;
    }

#region Calculations

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
            StatType.SpecialDamage,
            StatType.ExplosiveDamage,
            StatType.TrapDamage,
            StatType.EnemyDamage,
            StatType.MeleeDamage,
            StatType.PoisonDamage,
            StatType.FireDamage,
            StatType.SilverDamage,
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

    static public float CalculateInaccuracy(UsesStats _statUser, float _baseInaccuracy = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.RangedInaccuracy,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 1.0f;
        float maxVal = 40.0f;

        return GenericStatCalc(_statUser, _baseInaccuracy, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public float CalculateRangedAimTime(UsesStats _statUser, float _baseAimTime = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.RangedAimTime,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return GenericStatCalc(_statUser, _baseAimTime, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public int CalculateMaxSpareAmmo(UsesStats _statUser, float _baseSpareAmmo = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.SpareAmmo,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return (int)GenericStatCalc(_statUser, _baseSpareAmmo, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public float CalculateMaxHealth(UsesStats _statUser, float _baseHealth = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.PlayerMaxHealth,
            StatType.EnemyMaxHealth,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return GenericStatCalc(_statUser, _baseHealth, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public float CalculateHealAmount(UsesStats _statUser, float _baseHealAmount = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.PlayerHealAmount,
            //StatType.EnemyHealAmount,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return GenericStatCalc(_statUser, _baseHealAmount, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public float CalculateDamageTaken(UsesStats _statUser, float _baseDamage = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.PlayerDamageTaken,
            StatType.EnemyDamageTaken,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return GenericStatCalc(_statUser, _baseDamage, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public float CalculateHealthSteal(UsesStats _statUser, float _baseHealthSteal = 1.0f)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.PlayerHealthSteal,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return GenericStatCalc(_statUser, _baseHealthSteal, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
    }

    static public int CalculateCost(UsesStats _statUser, int _basePrice = 1)
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

    static public int CalculateMoneyGain(UsesStats _statUser, int _baseMoneyGain = 1)
    {
        // list of stats to use in this function
        List<StatType> usedStatTypes = new List<StatType>(){
            StatType.MoneyGain,
        };

        float additiveVal = 0.0f;
        float multiplierVal = 1.0f;

        float minVal = 0.0f;
        float maxVal = float.MaxValue;

        return (int)GenericStatCalc(_statUser, _baseMoneyGain, usedStatTypes, additiveVal, multiplierVal, minVal, maxVal);
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
    public static float GenericStatCalc(UsesStats _statUser, float _baseVal, List<StatType> _usedStatTypes, float _additiveVal = 0.0f, float _multiplierVal = 1.0f, float _minVal = 0.0f, float _maxVal = float.MaxValue)
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
    /// <param name="multiplierVal"></param>
    /// <param name="_usedStatTypes"></param>
    private static void CalcMods(UsesStats _statUser, ref float additiveVal, ref float multiplierVal, List<StatType> _usedStatTypes)
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
                            multiplierVal *= statMod.value;
                        }
                    }
                }
            }
        }
    }

#endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadStats(SaveManager.currentSaveSlot);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1){

    }

    static public void ClearDrinkMods()
    {
        activeDrinks.Clear();
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
        public List<Drink> activeDrinks = new List<Drink>();
        public List<StatModRange> possibleTalismanMods = new List<StatModRange>();
        public List<Talisman> activeTalismans = new List<Talisman>();
        public List<MonsterStat.SerializableMonsterStat> monsterStats = new List<MonsterStat.SerializableMonsterStat>();

        // health
        public float m_playerCurrentHealth = 100.0f;
        public float m_playerMaxHealth = 100.0f;
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
        // add drink mods
        data.activeDrinks = new List<Drink>(activeDrinks);
        // add possible talisman mods
        data.possibleTalismanMods = new List<StatModRange>(m_possibleTalismanMods);
        // add active talismans
        data.activeTalismans = new List<Talisman>(m_activeTalismans);
        // add monster stats
        data.monsterStats = new List<MonsterStat.SerializableMonsterStat>();
        foreach (MonsterStat monsterStat in m_monsterStats)
        {
            MonsterStat.SerializableMonsterStat serializableMonsterStat = new MonsterStat.SerializableMonsterStat(monsterStat);
            data.monsterStats.Add(serializableMonsterStat);
        }

        // add health
        data.m_playerCurrentHealth = m_playerCurrentHealth;
        data.m_playerMaxHealth = m_playerMaxHealth;

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
        // load drink mods
        activeDrinks = new List<Drink>(data.activeDrinks);
        // load possible talisman mods
        m_possibleTalismanMods = new List<StatModRange>(data.possibleTalismanMods);
        // load active talismans
        m_activeTalismans = new List<Talisman>(data.activeTalismans);
        // load monster stats
        m_monsterStats = new List<MonsterStat>();
        foreach (MonsterStat.SerializableMonsterStat serializableMonsterStat in data.monsterStats)
        {
            MonsterStat monsterStat = serializableMonsterStat.ToMonsterStat();
            m_monsterStats.Add(monsterStat);
        }

        // load health
        m_playerCurrentHealth = data.m_playerCurrentHealth;
        m_playerMaxHealth = data.m_playerMaxHealth;
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
        m_monsterStats = new List<MonsterStat>();
        foreach (MonsterStat.SerializableMonsterStat serializableMonsterStat in data.monsterStats)
        {
            MonsterStat monsterStat = serializableMonsterStat.ToMonsterStat();
            m_monsterStats.Add(monsterStat);
        }
        ResetPreviousKills();

        m_activeTalismans.Clear();

        // heal
        m_playerCurrentHealth = m_calcedPlayerMaxHealth;

        // save
        SaveStats(_saveSlot);
    }

    public void ResetPreviousKills()
    {
        foreach (MonsterStat stat in m_monsterStats)
        {
            stat.m_previousKills = stat.m_kills.Count;
        }
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
            statModRange.m_name = statModRange.m_statType.DisplayName() + " - " + statModRange.m_modType.ToString();
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
