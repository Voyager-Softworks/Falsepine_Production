using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that lets the player pikcup up an item.
/// @todo rework this to use the new inventory system.
/// </summary>
public class PickupScript : Interactable
{
    [Header("Pickup Settings")]
    public Item m_itemToAdd = null;
    public int amountToAdd = 1;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    override public void DoInteract()
    {
        base.DoInteract();

        // add item to inventory
        Inventory playerInv = InventoryManager.instance.GetInventory("player");
        playerInv.TryAddItemToInventory(m_itemToAdd.id, m_itemToAdd.instanceID, amountToAdd);
    }

    /// <summary>
    /// Tries to add an item to the inventory.
    /// </summary>
    /// <param name="_itemName"></param>
    /// <param name="_amount"></param>
    public void AddToInventory(string _itemName, int _amount)
    {
        old_InventoryManager inventoryManager = FindObjectOfType<old_InventoryManager>();

        if (inventoryManager == null) return;

        //inventoryManager.AddItem(_itemName, _amount);
    }
}
