using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : Interactable  /// @todo Comment
{
    [Header("Pickup Settings")]
    public string itemToAdd;
    public int amountToAdd;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();

        OnInteract.AddListener(() => AddToInventory(itemToAdd, amountToAdd));
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    public void AddToInventory(string _itemName, int _amount)
    {
        old_InventoryManager inventoryManager = FindObjectOfType<old_InventoryManager>();

        if (inventoryManager == null) return;

        //inventoryManager.AddItem(_itemName, _amount);
    }
}
