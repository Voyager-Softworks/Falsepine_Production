using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages the in game economy, things like player and store money, unlocked items, item prices, etc.
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

    [Serializable]
    public enum PriceType{
        BUY_PRICE,
        SELL_PRICE,
    }

    public interface Purchasable
    {
        int GetPrice(PriceType _type = PriceType.BUY_PRICE);
        int GetBuyPrice();
        int GetSellPrice();
        bool GetAllowedDiscount();
    }

    public int GenericSellPrice(Purchasable _purchasable, PriceType _type = PriceType.BUY_PRICE)
    {
        int price = _purchasable.GetBuyPrice();
        int // bruh do we even want to be able to sell things, cuz with discounts this might be a large issue?
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

    void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            //LoadMissions();
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    public void RefillStoreInventory()
    {
        
    }

    public bool CanAfford(int _amount){
        return m_playerSilver >= _amount;
    }
}
