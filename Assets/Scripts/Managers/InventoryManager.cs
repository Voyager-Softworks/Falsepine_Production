using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// Singleton donotdestroy script that handles the mission system
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    // list of inventories on this object
    [SerializeField] public List<Inventory> inventories = new List<Inventory>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            Inventory[] invs = GetComponentsInChildren<Inventory>();
            foreach (Inventory inv in invs)
            {
                AddInventory(inv);
            }

            LoadInventories(SaveManager.currentSaveSlot);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Adds inventory to the list of inventories
    /// </summary>
    /// <param name="inv"></param>
    public void AddInventory(Inventory inv)
    {
        // if inventory is already on this object, remove it first
        if (inventories.Contains(inv))
        {
            inventories.Remove(inv);
        }

        inventories.Add(inv);
    }

    /// <summary>
    /// Removes inventory from the list of inventories
    /// </summary>
    /// <param name="inv"></param>
    public void RemoveInventory(Inventory inv)
    {
        if (inventories.Contains(inv))
        {
            inventories.Remove(inv);
        }
    }

    /// <summary>
    /// Gets an inventory by ID
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    public Inventory GetInventory(string _id)
    {
        foreach (Inventory inv in inventories)
        {
            if (inv.id == _id)
            {
                return inv;
            }
        }
        return null;
    }

    public void SaveInventories(int saveSlot)
    {
        foreach (Inventory inv in inventories)
        {
            inv.SaveInventory(saveSlot);
        }
    }

    /// <summary>
    /// Tries to move an item from one inventory to another
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="index"></param>
    public void TryMoveItem(Inventory source, Inventory target, int index)
    {
        Item item = source.slots[index].item;
        if (target.CanAddItemToInventory(item))
        {
            source.RemoveItemFromInventory(index);
            Item leftover = target.TryAddItemToInventory(item);
            if (leftover != null)
            {
                source.TryAddItemToInventory(leftover);
            }
        }
    }

    /// <summary>
    /// Checks if it is possible to move an item from one inventory to another
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CanMoveItem(Inventory source, Inventory target, int index)
    {
        Item item = source.slots[index].item;
        return target.CanAddItemToInventory(item);
    }

    /// <summary>
    /// Loads inventories from file
    /// </summary>
    /// <param name="saveSlot"></param>
    public void LoadInventories(int saveSlot)
    {
        foreach (Inventory inv in inventories)
        {
            inv.LoadInventory(saveSlot);
        }
    }

    /// <summary>
    /// Deletes all inventories at save index
    /// </summary>
    /// <param name="saveSlot"></param>
    public void DeleteInventories(int saveSlot)
    {
        foreach (Inventory inv in inventories)
        {
            inv.DeleteSaveFile(saveSlot);
        }
    }
}
