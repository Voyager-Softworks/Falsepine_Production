using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// This class is used to manage the inventory UI.<br/>
/// Used on inventory pannels to display items, and allow the player to interact with them.
/// </summary>
public class InventoryPannel : MonoBehaviour
{
    public string inventoryID;
    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    [SerializeField] public Inventory linkedInventory;
    [SerializeField] public InventoryPannel otherPannel;

    [Header("Transferring")]
    [SerializeField] public bool sendingAllowed = true;
    [SerializeField] public bool receivingAllowed = true;
    [SerializeField] public bool removeSpaces = false;

    [Header("UI")]
    public List<InventoryCell> cells = new List<InventoryCell>();

    // Start is called before the first frame update
    public virtual void Start()
    {
        // get inventory
        linkedInventory = InventoryManager.instance?.GetInventory(inventoryID);

        // if cells empty, get them
        if (cells.Count == 0)
        {
            cells = GetComponentsInChildren<InventoryCell>().ToList();
        }

        LinkGridItems();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // every 5 frames, update the UI
        if (Time.frameCount % 5 == 0)
        {
            UpdateUI();
        }

        // every 20 frames, try remove empty spaces
        if (Time.frameCount % 20 == 0)
        {
            if (removeSpaces){
                TryRemoveSpaces(linkedInventory);
            }
        }
    }

    public virtual void UpdateUI()
    {
        // update each cell
        foreach (InventoryCell cell in cells)
        {
            cell.UpdateUI();
        }
    }

    protected virtual void LinkGridItems(){
        // get inventory, and a list of slots in it
        Inventory inventory = InventoryManager.instance?.GetInventory(inventoryID);
        List<Inventory.InventorySlot> slots = new List<Inventory.InventorySlot>();
        if (inventory != null)
        {
            slots = new List<Inventory.InventorySlot>(inventory.slots);
        }

        // link grid items to cells
        foreach (InventoryCell cell in cells)
        {
            InventoryGridItem gridItem = cell.gridItem;
            if (gridItem != null)
            {
                gridItem.linkedSlot = slots.FirstOrDefault();
                // if slot is not null, remove it from the list
                if (gridItem.linkedSlot != null)
                {
                    slots.Remove(gridItem.linkedSlot);
                }
            }
        }
    }

    public virtual void ItemClicked(InventoryGridItem gridItem)
    {
        if (gridItem == null) return;

        if (!sendingAllowed) return;

        Inventory sourceInventory = gridItem.linkedSlot.ownerInventory;
        if (sourceInventory == null) return;

        //get index of slot in source inventory
        int sourceIndex = sourceInventory.GetItemIndex(gridItem.itemInSlot);
        if (sourceIndex == -1) return;

        Inventory targetInventory = otherPannel?.linkedInventory;
        if (targetInventory == null) return;

        if (!otherPannel.receivingAllowed) return;

        PerformClickAction(gridItem, sourceInventory, targetInventory, sourceIndex);

        if (removeSpaces){
            TryRemoveSpaces(sourceInventory);
        }
    }

    protected virtual void PerformClickAction(InventoryGridItem gridItem, Inventory sourceInventory, Inventory targetInventory, int sourceIndex){
        InventoryManager.instance.TryMoveItem(sourceInventory, targetInventory, sourceIndex);
    }

    protected virtual void TryRemoveSpaces(Inventory sourceInventory){
        // get inventory, and a list of slots in it
        Inventory inventory = InventoryManager.instance?.GetInventory(inventoryID);
        List<Inventory.InventorySlot> slots = new List<Inventory.InventorySlot>();
        if (inventory != null)
        {
            slots = new List<Inventory.InventorySlot>(inventory.slots);
        }

        // loop through each slot in the inventory, try move the item to an earlier slot
        foreach (Inventory.InventorySlot slot in slots)
        {
            Item item = slot.item;
            if (item == null) continue;

            //get index of slot in source inventory
            int sourceIndex = sourceInventory.GetItemIndex(item);
            if (sourceIndex == -1) return;

            sourceInventory.RemoveItemFromInventory(sourceIndex);

            Item leftover = sourceInventory.TryAddItemToInventory(item);
        }
    }
}
