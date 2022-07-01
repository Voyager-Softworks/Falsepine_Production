using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TownBuilding_Home : TownBuilding
{
    public GameObject playerPrimaryCell;
    public GameObject playerSecondaryCell;
    public GameObject playerEquipmentCell;

    public GameObject gridPanel;
    public GameObject itemPrefab;

    //4x3 grid
    public int gridWidth = 4;
    public int gridHeight = 3;

    public List<GameObject> gridItems = new List<GameObject>();

    //update
    protected override void Update()
    {
        base.Update();

        UpdateUI();
        
    }

    public void UpdateUI(){
        UpdateHomeItems();
        UpdatePlayerItems();

        if (!UI.activeSelf) return;
        // update info box
        InfoBox ib = FindObjectOfType<InfoBox>();
        if (ib != null)
        {
            List<GameObject> items = new List<GameObject>(gridItems);
            items.Add(playerPrimaryCell);
            items.Add(playerSecondaryCell);
            items.Add(playerEquipmentCell);

            // if mouse is over a grid item, display info box
            // if (items.Count > 0)
            // {
            //     foreach (GameObject gridItem in items)
            //     {
            //         Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            //         Vector3 gridItemWorldPos = gridItem.transform.position;
            //         Vector2 gridItemScreenPos = uiCamera.WorldToScreenPoint(gridItemWorldPos);
            //         float width = gridItem.GetComponent<RectTransform>().rect.width;
            //         float height = gridItem.GetComponent<RectTransform>().rect.height;

            //         // check if mouse is within the grid item's screen space
            //         if (mouseScreenPos.x > gridItemScreenPos.x &&
            //             mouseScreenPos.x < gridItemScreenPos.x + width &&
            //             mouseScreenPos.y < gridItemScreenPos.y &&
            //             mouseScreenPos.y > gridItemScreenPos.y - height)
            //         {
            //             // display info box
            //             ib.Display(gridItem.GetComponent<InventoryGridItem>().item, 0.1f, 0.1f);
            //             break;
            //         }
            //     }
            // }
        }
    }

    public void UpdateHomeItems(){
        if (itemPrefab == null) return;
        //get home inventory
        Inventory homeInventory = InventoryManager.instance.GetInventory("home");
        if (homeInventory == null) return;

        List<Item> items = new List<Item>();
        foreach (Inventory.InventorySlot slot in homeInventory.slots)
        {
            if (slot.item != null)
            {
                items.Add(slot.item);
            }
        }

        int gridCount = gridItems.Count;
        int itemCount = items.Count;

        //if there are more items than grid items, add more grid items
        if (itemCount > gridCount)
        {
            for (int i = gridCount; i < itemCount; i++)
            {
                GameObject newItem = Instantiate(itemPrefab);
                newItem.transform.SetParent(gridPanel.transform, false);
                gridItems.Add(newItem);
            }
        }
        //if there are more grid items than items, remove grid items
        else if (itemCount < gridCount)
        {
            for (int i = gridCount - 1; i >= itemCount; i--)
            {
                Destroy(gridItems[i]);
                gridItems.RemoveAt(i);
            }
        }

        // update grid items icons and texts
        // for (int i = 0; i < gridItems.Count; i++)
        // {
        //     if (i >= itemCount)
        //     {
        //         //gridItems[i].SetActive(false);
        //     }
        //     else
        //     {
        //         //gridItems[i].SetActive(true);
        //         Item item = items[i];
        //         if (item != null)
        //         {
        //             gridItems[i].GetComponent<InventoryGridItem>().item = item;

        //             gridItems[i].GetComponent<InventoryGridItem>().icon.sprite = item.m_icon;
        //             gridItems[i].GetComponent<InventoryGridItem>().text.text = item.m_displayName;

        //             //get index of item in home inventory
        //             int index = homeInventory.GetItemIndex(item);

        //             //button event
        //             gridItems[i].GetComponent<InventoryGridItem>().button.onClick.RemoveAllListeners();
        //             gridItems[i].GetComponent<InventoryGridItem>().button.onClick.AddListener(() => {
        //                 InventoryManager.instance.MoveItem(homeInventory, InventoryManager.instance.GetInventory("player"), index);
        //             });
        //         }
        //     }
        // }
    }

    public void UpdatePlayerItems(){
        Inventory playerInventory = InventoryManager.instance.GetInventory("player");
        if (playerInventory == null) return;

        Item primaryItem = playerInventory.slots[0].item;
        Item secondaryItem = playerInventory.slots[1].item;
        Item equipmentItem = playerInventory.slots[2].item;

        // if (primaryItem != null)
        // {
        //     playerPrimaryCell.SetActive(true);
            
        //     playerPrimaryCell.GetComponent<InventoryGridItem>().item = primaryItem;

        //     playerPrimaryCell.GetComponent<InventoryGridItem>().icon.sprite = primaryItem.m_icon;
        //     playerPrimaryCell.GetComponent<InventoryGridItem>().text.text = primaryItem.m_displayName;

        //     playerPrimaryCell.GetComponent<InventoryGridItem>().button.onClick.RemoveAllListeners();
        //     playerPrimaryCell.GetComponent<InventoryGridItem>().button.onClick.AddListener(() => {
        //         InventoryManager.instance.MoveItem(playerInventory, InventoryManager.instance.GetInventory("home"), 0);
        //     });
        // }
        // else
        // {
        //     playerPrimaryCell.SetActive(false);
        // }

        // if (secondaryItem != null)
        // {
        //     playerSecondaryCell.SetActive(true);

        //     playerSecondaryCell.GetComponent<InventoryGridItem>().item = secondaryItem;

        //     playerSecondaryCell.GetComponent<InventoryGridItem>().icon.sprite = secondaryItem.m_icon;
        //     playerSecondaryCell.GetComponent<InventoryGridItem>().text.text = secondaryItem.m_displayName;

        //     playerSecondaryCell.GetComponent<InventoryGridItem>().button.onClick.RemoveAllListeners();
        //     playerSecondaryCell.GetComponent<InventoryGridItem>().button.onClick.AddListener(() => {
        //         InventoryManager.instance.MoveItem(playerInventory, InventoryManager.instance.GetInventory("home"), 1);
        //     });
        // }
        // else
        // {
        //     playerSecondaryCell.SetActive(false);
        // }

        // if (equipmentItem != null)
        // {
        //     playerEquipmentCell.SetActive(true);

        //     playerEquipmentCell.GetComponent<InventoryGridItem>().item = equipmentItem;

        //     playerEquipmentCell.GetComponent<InventoryGridItem>().icon.sprite = equipmentItem.m_icon;
        //     playerEquipmentCell.GetComponent<InventoryGridItem>().text.text = equipmentItem.m_displayName;

        //     playerEquipmentCell.GetComponent<InventoryGridItem>().button.onClick.RemoveAllListeners();
        //     playerEquipmentCell.GetComponent<InventoryGridItem>().button.onClick.AddListener(() => {
        //         InventoryManager.instance.MoveItem(playerInventory, InventoryManager.instance.GetInventory("home"), 2);
        //     });
        // }
        // else
        // {
        //     playerEquipmentCell.SetActive(false);
        // }



    }
}
