using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages the in game economy, things like player and store money, unlocked items, item prices, etc.
/// </summary>
[Serializable]
public class EconomyManager : MonoBehaviour, StatsManager.UsesStats  /// @todo comment
{
    public static EconomyManager instance;

    // StatsManager.UsesStats implementation
    private List<StatsManager.StatType> m_usedStatTypes = new List<StatsManager.StatType>()
    {

    };
    public List<StatsManager.StatType> GetStatTypes()
    {
        return m_usedStatTypes;
    }

    public interface Purchasable
    {
        int GetPrice();
        bool GetAllowedDiscount();
    }

    [Serializable]
    public class PurchasableItem
    {
        public Item item = null;

        public int minAmount = 1;
        public int maxAmount = 1;
    }

    public string storeInventoryName = "store";
    public string playerInventoryName = "player";
    public string homeInventoryName = "home";

    public List<PurchasableItem> purchasableItems = new List<PurchasableItem>();

    public int m_playerSilver = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            //LoadMissions();
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

    public bool CanAfford(int _amount)
    {
        return m_playerSilver >= _amount;
    }

    public void Spend(int _amount)
    {
        m_playerSilver -= _amount;
    }
}
