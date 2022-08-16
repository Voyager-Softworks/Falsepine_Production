using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages the in game economy, things like player and store money, unlocked items, item prices, etc.
/// @todo make use of this more in the sotre. Perhaps rework it.
/// </summary>
[Serializable]
public class EconomyManager : MonoBehaviour, StatsManager.UsesStats
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

    /// <summary>
    /// Purchasable interface for things that can be purchased.
    /// </summary>
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
}
