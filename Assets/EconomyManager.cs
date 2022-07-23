using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages the in game economy, things like player and shop money, unlocked items, item prices, etc.
/// </summary>
[Serializable]
public class EconomyManager : MonoBehaviour, StatsManager.UsesStats
{
    public static EconomyManager instance;

    // StatsManager.UsesStats implementation
    public List<StatsManager.StatType> m_usedStatTypes = new List<StatsManager.StatType>()
    {

    };
    public List<StatsManager.StatType> GetStatTypes()
    {
        return m_usedStatTypes;
    }

    public interface Purchasable
    {
        int GetPrice();
        bool AllowDiscount();
    }

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
}
