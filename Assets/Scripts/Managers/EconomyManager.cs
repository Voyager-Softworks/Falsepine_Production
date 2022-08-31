using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
// scene manage
using UnityEngine.SceneManagement;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public Item m_item = null;

        public int m_minAmount = 1;
        public int m_maxAmount = 1;

        public bool m_unlocked = false;

        public bool m_removeIfExists = false;

        // serializable version of the PurchasableItem class.
        [Serializable]
        public class PurchasableItem_Serializable
        {
            public string itemJSON = "";
            public string itemType = "";
            public int minAmount = 1;
            public int maxAmount = 1;
            public bool unlocked = false;
            public bool removeIfExists = false;

            public PurchasableItem_Serializable(PurchasableItem _purchasable)
            {
                //serialize item
                itemJSON = JsonUtility.ToJson(_purchasable.m_item, true);
                itemType = _purchasable.m_item.GetType().Name;
                this.minAmount = _purchasable.m_minAmount;
                this.maxAmount = _purchasable.m_maxAmount;
                this.unlocked = _purchasable.m_unlocked;
                this.removeIfExists = _purchasable.m_removeIfExists;
            }

            public PurchasableItem ToPurchasableItem()
            {
                // make scritableobject of same type
                ScriptableObject item = ScriptableObject.CreateInstance(itemType);

                // load data into item
                JsonUtility.FromJsonOverwrite(itemJSON, item);

                // create purchasable item
                PurchasableItem purchasableItem = new PurchasableItem();
                purchasableItem.m_item = (Item)item; // ensure to cast item to Item
                purchasableItem.m_minAmount = minAmount;
                purchasableItem.m_maxAmount = maxAmount;
                purchasableItem.m_unlocked = unlocked;
                purchasableItem.m_removeIfExists = removeIfExists;
                return purchasableItem;
            }
        }
    }

    public string storeInventoryName = "store";
    public string playerInventoryName = "player";
    public string homeInventoryName = "home";

    public List<PurchasableItem> m_purchasableItems = new List<PurchasableItem>();

    public int m_playerSilver = 0;

    public int m_maxStoreItems = 10;

    private class SaveData {
        public List<PurchasableItem.PurchasableItem_Serializable> purchasableItems = new List<PurchasableItem.PurchasableItem_Serializable>();
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

            // on scene load
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //RefillStoreInventory();
    }

    public void RefillStoreInventory()
    {
        // get store inv
        Inventory storeInventory = InventoryManager.instance.GetInventory(storeInventoryName);
        if (storeInventory == null){
            Debug.LogError("Store inventory not found");
            return;
        }

        // reset store inv
        storeInventory.ResetInventory();

        // refill store inv from purchasable items:

        // randomize purchasable items in new list
        List<PurchasableItem> tempPurchasableItems = new List<PurchasableItem>(m_purchasableItems);
        tempPurchasableItems = tempPurchasableItems.OrderBy(x => Guid.NewGuid()).ToList();

        // get up to maxStoreItems items from purchasable items
        int itemsAdded = 0;
        foreach (PurchasableItem purchasableItem in tempPurchasableItems)
        {
            // check if max items reached
            if (itemsAdded >= m_maxStoreItems)
            {
                break;
            }
            //@todo make this section work lmao
            // check if item exists in player, home, or bank
            // if (InventoryManager.instance.GetInventory(playerInventoryName).ContainsItem(purchasableItem.m_item) ||
            //     InventoryManager.instance.GetInventory(homeInventoryName).ContainsItem(purchasableItem.m_item) ||
            //     InventoryManager.instance.GetInventory(storeInventoryName).ContainsItem(purchasableItem.m_item))
            // {
            //     continue;
            // }
            if (purchasableItem.m_unlocked)
            {
                // make new instance
                int randAmount = UnityEngine.Random.Range(purchasableItem.m_minAmount, purchasableItem.m_maxAmount);
                Item item = purchasableItem.m_item.CreateInstance();
                item.currentStackSize = randAmount;

                // add item to store inventory
                storeInventory.TryAddItemToInventory(item);
                itemsAdded++;
            }
        }
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
        
        data.purchasableItems = new List<PurchasableItem.PurchasableItem_Serializable>();

        foreach (PurchasableItem item in m_purchasableItems)
        {
            data.purchasableItems.Add(new PurchasableItem.PurchasableItem_Serializable(item));
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

        foreach (PurchasableItem.PurchasableItem_Serializable item in data.purchasableItems)
        {
            m_purchasableItems.Add(item.ToPurchasableItem());
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

    #if UNITY_EDITOR
    private void OnValidate() {
        foreach (PurchasableItem purchasable in m_purchasableItems)
        {
            if (purchasable.m_item?.instanceID == "")
            {
                // if game is not running, return
                //@todo add this check back in, remove to try create instances in editor
                //if (!Application.isPlaying) return;

                purchasable.m_item = purchasable.m_item.CreateInstance();

                // set dirty
                EditorUtility.SetDirty(this);
            }
        }
    }
    #endif
}