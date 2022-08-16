using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to keep track of each cell in the inventory grid. It is not funcitonal.
/// </summary>
[Serializable]
public class InventoryCell : MonoBehaviour  /// @todo comment
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

    internal void UpdateUI()
    {
        if (gridItem && gridItem)
        {
            gridItem.UpdateUI();
        }
    }
}