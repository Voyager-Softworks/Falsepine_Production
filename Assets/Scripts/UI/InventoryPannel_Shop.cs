using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Inventory pannel for the shop specifically.
/// </summary>
public class InventoryPannel_Shop : InventoryPannel
{

    public Item itemToBuy = null;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    protected override void UpdateItemDisplays()
    {
        base.UpdateItemDisplays();
    }

    public override void ItemClicked(ItemDisplay gridItem)
    {
        base.ItemClicked(gridItem);
    }

    /// <summary>
    /// Tries to buy the item.
    /// @todo Make this work again with new UI.
    /// </summary>
    /// <param name="gridItem"></param>
    /// <param name="sourceInventory"></param>
    /// <param name="targetInventory"></param>
    /// <param name="sourceIndex"></param>
    protected override void PerformClickAction(ItemDisplay _itemDisplay, Inventory _sourceInventory, Inventory _targetInventory, int _sourceIndex)
    {
        Item item = _itemDisplay.m_linkedItem;
        if (item == null) return;

        itemToBuy = item;

        EconomyManager economyManager = EconomyManager.instance;
        if (economyManager == null) return;

        int price = item.GetPrice();
        if (economyManager.CanAfford(price)){
            //transfer
            if (InventoryManager.instance.CanMoveItem(_sourceInventory, _targetInventory, _sourceIndex)){
                economyManager.Spend(price);
                InventoryManager.instance.TryMoveItem(_sourceInventory, _targetInventory, _sourceIndex);
            }
        }
    }
}
