using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Manages the in game economy, things like player and store money, unlocked items, item prices, etc.
/// @todo make use of this more in the sotre. Perhaps rework it.
/// </summary>
[Serializable]
public class EconomyManager : MonoBehaviour, StatsManager.UsesStats
{
    public static EconomyManager instance;

    public static string GetSaveFolderPath(int saveSlot)
    {
        return SaveManager.GetSaveFolderPath(saveSlot) + "/economy/";
    }

    public static string GetSaveFilePath(int saveSlot)
    {
        return GetSaveFolderPath(saveSlot) + "economy.json";
    }

    // StatsManager.UsesStats implementation
    private List<StatsManager.StatType> m_usedStatTypes = new List<StatsManager.StatType>()
    {

    };
    public List<StatsManager.StatType> GetStatTypes()
    {
        return m_usedStatTypes;
    }
    public void AddStatType(StatsManager.StatType statType)
    {
        if (!m_usedStatTypes.Contains(statType))
        {
            m_usedStatTypes.Add(statType);
        }
    }
    public void RemoveStatType(StatsManager.StatType statType)
    {
        if (m_usedStatTypes.Contains(statType))
        {
            m_usedStatTypes.Remove(statType);
        }
    }

    /// <summary>
    /// Purchasable interface for things that can be purchased.
    /// </summary>
    public interface Purchasable
    {
        int GetPrice();
        bool GetAllowedDiscount();
    }

    /// <summary>
    /// Items which can be purchsed and info about them, such as unlock state, amount that can be bought, etc.
    /// @todo make this saveable.
    /// </summary>
    [Serializable]
    public class PurchasableItem
    {
        public Item item = null;

        public int minAmount = 1;
        public int maxAmount = 1;

        public bool unlocked = false;
    }

    public string storeInventoryName = "store";
    public string playerInventoryName = "player";
    public string homeInventoryName = "home";

    public List<PurchasableItem> purchasableItems = new List<PurchasableItem>();

    public int m_playerSilver = 0;

    private class SaveData {
        public List<PurchasableItem> purchasableItems = new List<PurchasableItem>();
        public int m_playerSilver = 0;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadEconomy(SaveManager.currentSaveSlot);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    public void RefillStoreInventory()
    {

    }

    /// <summary>
    /// Checks if the player has enough money to purchase the item.
    /// </summary>
    /// <param name="_amount"></param>
    /// <returns></returns>
    public bool CanAfford(int _amount)
    {
        return m_playerSilver >= _amount;
    }

    /// <summary>
    /// Spend the specified amount of money.
    /// </summary>
    /// <param name="_amount"></param>
    public void Spend(int _amount)
    {
        m_playerSilver -= _amount;
    }

    public void SaveEconomy(int saveSlot)
    {
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }

        FileStream file = File.Create(GetSaveFilePath(saveSlot));

        // make data
        SaveData data = new SaveData();
        foreach (PurchasableItem purchasableItem in purchasableItems)
        {
            //@todo make this actually saveable/serializable
            data.purchasableItems.Add(purchasableItem);
        }
        data.m_playerSilver = m_playerSilver;

        StreamWriter writer = new StreamWriter(file);

        // save the data
        writer.Write(JsonUtility.ToJson(data, true));

        writer.Close();

        file.Close();
    }

    public void LoadEconomy(int saveSlot)
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

        // load the data 
        SaveData data = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());

        foreach (PurchasableItem purchasableItem in data.purchasableItems)
        {
            //@todo make this actually saveable/serializable
            purchasableItems.Add(purchasableItem);
        }

        m_playerSilver = data.m_playerSilver;

        reader.Close();

        file.Close();
    }

    /// <summary>
    /// Deletes the save at index
    /// </summary>
    /// <param name="saveSlot"></param>
    public void DeleteEconomySave(int saveSlot)
    {
        if (File.Exists(GetSaveFilePath(saveSlot)))
        {
            File.Delete(GetSaveFilePath(saveSlot));
        }
    }
}
