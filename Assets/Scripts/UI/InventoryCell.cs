using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of this cell and the item it contains.
/// </summary>
[Serializable]
public class InventoryCell : MonoBehaviour
{
    [SerializeField] public InventoryGridItem gridItem;

    private void Update()
    {
        if (gridItem && gridItem.itemInSlot)
        {
            if (!gridItem.gameObject.activeSelf)
            {
                gridItem.UpdateUI();
                gridItem.gameObject.SetActive(true);
            }
        }
        else
        {
            if (gridItem.gameObject.activeSelf)
            {
                gridItem.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateUI()
    {
        if (gridItem && gridItem)
        {
            gridItem.UpdateUI();
        }
    }
}