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
    [SerializeField] public bool transferCosts = false;
    [SerializeField] public EconomyManager.PriceType sendingPriceType = EconomyManager.PriceType.BUY_PRICE;
    [SerializeField] public EconomyManager.PriceType receivingPriceType = EconomyManager.PriceType.SELL_PRICE;

    [Header("UI")]
    public List<InventoryCell> cells = new List<InventoryCell>();

    // Start is called before the first frame update
    void Start()
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
    void Update()
    {
        // every 5 frames, update the UI
        if (Time.frameCount % 5 == 0)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // update each cell
        foreach (InventoryCell cell in cells)
        {
            cell.UpdateUI();
        }
    }

    public void LinkGridItems(){
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

    public void ItemClicked(InventoryGridItem gridItem)
    {
        if (gridItem == null) return;

        Item item = gridItem.itemInSlot;
        if (gridItem.itemInSlot == null) return;

        if (!sendingAllowed) return;

        Inventory sourceInventory = gridItem.linkedSlot.ownerInventory;
        if (sourceInventory == null) return;

        //get index of slot in source inventory
        int sourceIndex = sourceInventory.GetItemIndex(gridItem.itemInSlot);
        if (sourceIndex == -1) return;

        Inventory targetInventory = otherPannel?.linkedInventory;
        if (targetInventory == null) return;

        if (!otherPannel.receivingAllowed) return;

        EconomyManager economyManager = EconomyManager.instance;
        if (economyManager == null) return;

        if (!transferCosts){
            //transfer
            InventoryManager.instance.TryMoveItem(sourceInventory, targetInventory, sourceIndex);
        }
        else{
            //int cost = item.getp
        }
    }
}
