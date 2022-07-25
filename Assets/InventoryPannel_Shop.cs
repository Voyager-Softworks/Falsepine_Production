using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// This class is used to manage the inventory UI.<br/>
/// Used on inventory pannels to display items, and allow the player to interact with them.
/// </summary>
public class InventoryPannel_Shop : InventoryPannel
{

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

    protected override void LinkGridItems(){
        base.LinkGridItems();
    }

    public override void ItemClicked(InventoryGridItem gridItem)
    {
        base.ItemClicked(gridItem);
    }

    protected override void PerformClickAction(InventoryGridItem gridItem, Inventory sourceInventory, Inventory targetInventory, int sourceIndex)
    {
        Item item = gridItem.itemInSlot;
        if (gridItem.itemInSlot == null) return;

        EconomyManager economyManager = EconomyManager.instance;
        if (economyManager == null) return;

        int price = item.GetPrice();
        if (economyManager.CanAfford(price)){
            //transfer
            if (InventoryManager.instance.CanMoveItem(sourceInventory, targetInventory, sourceIndex)){
                economyManager.Spend(price);
                InventoryManager.instance.TryMoveItem(sourceInventory, targetInventory, sourceIndex);
            }
        }
    }
}
