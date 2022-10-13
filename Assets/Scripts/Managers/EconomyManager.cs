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

    public static string GetRecentDeathFolderPath(int saveSlot)
    {
        return SaveManager.GetRecentDeathFolderPath(saveSlot) + "/economy/";
    }

    public static string GetSaveFilePath(int saveSlot)
    {
        return GetSaveFolderPath(saveSlot) + "economy.json";
    }

    public static string GetRecentDeathFilePath(int saveSlot)
    {
        return GetRecentDeathFolderPath(saveSlot) + "economy.json";
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
    /// @todo make this actually save items in file somehwere? Seems to be neccecary if we want to be safe.
    /// </summary>
    [Serializable]
    public class PurchasableItem
    {
        [SerializeField] public Item m_item = null;

        [SerializeField] public int m_minAmount = 1;
        [SerializeField] public int m_maxAmount = 1;

        [SerializeField] public bool m_unlocked = false;

        [SerializeField] public bool m_removeIfExists = false;

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
                itemType = _purchasable.m_item != null ? _purchasable.m_item.GetType().Name : "";
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
    public string bankInventroyName = "bank";

    [SerializeField] public List<PurchasableItem> m_purchasableItems = new List<PurchasableItem>();
    [SerializeField] public List<Drink> m_purchasableDrinks = new List<Drink>();

    public int m_playerSilver = 0;

    public int m_maxStoreItems = 10;

    
    public int m_bankLevel = 0;

    [Header("Events")]
    public System.Action<int> OnPlayerSilverAdded;

    private class SaveData {
        public List<PurchasableItem.PurchasableItem_Serializable> purchasableItems = new List<PurchasableItem.PurchasableItem_Serializable>();
        public int m_playerSilver = 0;
        public int m_bankLevel = 0;
        public int m_maxStoreItems = 0;
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

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefillStoreInventory();

        TownBuilding_Bank bank = FindObjectOfType<TownBuilding_Bank>();
        if (bank != null)
        {
            // bind upgrade button to upgrade function (remove all listeners first?)
            bank.m_upgradeButton.onClick.RemoveAllListeners();
            bank.m_upgradeButton.onClick.AddListener(() => { UpgradeBank(); });
        }
    }

    /// <summary>
    /// Gets the amount of slots theat the bank has unlocked.
    /// @todo move this somewhere more responsible, perhaps a bank manager?
    /// </summary>
    /// <returns></returns>
    public int GetUnlockedBankSlotAmount()
    {
        int amount = 1;

        // at level 10, unlock 1 slot
        if (m_bankLevel >= 10)
        {
            amount += 1;
        }

        // for each unique boss kill, unlock 1 slot
        foreach (StatsManager.MonsterStat monster in StatsManager.instance.m_monsterStats)
        {
            if (monster != null && monster.m_monster != null && monster.m_monster.m_type == MonsterInfo.MonsterType.Boss && monster.m_kills.Count() >= 1)
            {
                amount += 1;
            }
        }

        return amount;
    }

    /// <summary>
    /// Increases the bank level by 1, and updates the slot amount.
    /// </summary>
    public void UpgradeBank()
    {
        m_bankLevel++;

        // if level over 10, set to 10
        if (m_bankLevel > 10)
        {
            m_bankLevel = 10;
        }

        // update slot amount
        UpdateBankSlotAmount();
    }

    /// <summary>
    /// Adds slots to the bank based on the current bank level.
    /// </summary>
    public void UpdateBankSlotAmount()
    {
        int unlockedSlots = GetUnlockedBankSlotAmount();
        // add slots to inventory until it reaches the unlocked amount
        Inventory bankInv = InventoryManager.instance.GetInventory(bankInventroyName);
        while (bankInv.GetSlotCount() < unlockedSlots)
        {
            bankInv.AddSlot();
        }
    }

    /// <summary>
    /// Returns the percentage of silver the player will retain on death, based on the bank level.
    /// </summary>
    /// <returns></returns>
    public float GetSilverRetainPercentage()
    {
        float maxPercent = 0.5f; // max percentage of silver retained
        int levelMaxReached = 10; // level at which max percentage is reached

        float percent = (float)m_bankLevel / (float)levelMaxReached;
        percent = Mathf.Clamp(percent, 0, maxPercent);

        return percent;
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
            if (purchasableItem != null && purchasableItem.m_unlocked && purchasableItem.m_item != null)
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
    public void SpendMoney(int _amount)
    {
        m_playerSilver -= _amount;

        if (MessageManager.instance) {
            MessageManager.instance.AddMessage("-" + _amount + " silver", "silver", true);
        }
    }

    /// <summary>
    /// Adds the specified amount of money.
    /// @todo make some sort of UI display when money is added
    /// </summary>
    /// <param name="_amount"></param>
    public void AddMoney(int _amount)
    {
        int calcedAmount = StatsManager.CalculateMoneyGain(this, _amount);

        m_playerSilver += calcedAmount;

        // message
        if (MessageManager.instance) {
            if (calcedAmount != 0){
                MessageManager.instance.AddMessage("+" + calcedAmount + " silver", "silver", true);
            }
        }

        // event
        OnPlayerSilverAdded?.Invoke(calcedAmount);
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
        data.m_bankLevel = m_bankLevel;
        data.m_maxStoreItems = m_maxStoreItems;

        StreamWriter writer = new StreamWriter(file);

        // save the data
        writer.Write(JsonUtility.ToJson(data, true));

        writer.Close();

        file.Close();
    }

    public void LoadEconomy(int _saveSlot)
    {
        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(_saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(_saveSlot));
        }
        // if save file doesn't exist, return
        if (!File.Exists(GetSaveFilePath(_saveSlot)))
        {
            Debug.Log("Save file does not exist.");
            //RestoreFromLastDeath(_saveSlot);
            return;
        }

        // read the file
        FileStream file = File.Open(GetSaveFilePath(_saveSlot), FileMode.Open);

        StreamReader reader = new StreamReader(file);

        // load the data 
        SaveData data = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());

        m_purchasableItems = new List<PurchasableItem>();
        foreach (PurchasableItem.PurchasableItem_Serializable item in data.purchasableItems)
        {
            m_purchasableItems.Add(item.ToPurchasableItem());
        }
        m_playerSilver = data.m_playerSilver;
        m_bankLevel = data.m_bankLevel;
        m_maxStoreItems = data.m_maxStoreItems;

        reader.Close();

        file.Close();
    }

    /// <summary>
    /// Loads the economy from the most recent death. <br/>
    /// Retians the bank level, purchasable items, and max store items, but only retains a portion of the player's silver.
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

        // load the data
        SaveData data = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());

        // retain bank level, purchasable items, and max store items
        m_bankLevel = data.m_bankLevel;
        m_maxStoreItems = data.m_maxStoreItems;
        m_purchasableItems = new List<PurchasableItem>();
        foreach (PurchasableItem.PurchasableItem_Serializable item in data.purchasableItems)
        {
            m_purchasableItems.Add(item.ToPurchasableItem());
        }

        // retain a portion of the player's silver
        float percent = GetSilverRetainPercentage();
        m_playerSilver = (int)(data.m_playerSilver * percent);

        // save
        SaveEconomy(_saveSlot);
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
            if (purchasable.m_item != null && purchasable.m_item.instanceID != "" && ItemDatabase.GetItemsByInstanceID(purchasable.m_item.instanceID).Count > 0)
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

    //custom editor
    [CustomEditor(typeof(EconomyManager))]
    public class EconomyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EconomyManager economy = (EconomyManager)target;

            // draw default inspector
            DrawDefaultInspector();

            // button to re-add all purchasable items items using their ID's
            if (GUILayout.Button("Re-add all items"))
            {
                foreach (PurchasableItem purchasable in economy.m_purchasableItems)
                {
                    Item item = ItemDatabase.GetItem(purchasable.m_item.id).CreateInstance();
                    if (item != null)
                    {
                        purchasable.m_item = item;
                    }

                    // set dirty
                    EditorUtility.SetDirty(economy);
                }
            }

            // add all drinks button
            if (GUILayout.Button("Add all drinks"))
            {
                // get all Drink assets
                List<Drink> drinks = AssetDatabase.FindAssets("t:Drink").Select(guid => AssetDatabase.LoadAssetAtPath<Drink>(AssetDatabase.GUIDToAssetPath(guid))).ToList();

                economy.m_purchasableDrinks = new List<Drink>();
                foreach (Drink drink in drinks)
                {
                    economy.m_purchasableDrinks.Add(drink);
                }

                // set dirty
                EditorUtility.SetDirty(economy);
            }
        }
    }
    #endif
}