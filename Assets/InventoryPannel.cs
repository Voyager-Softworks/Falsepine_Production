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
}
