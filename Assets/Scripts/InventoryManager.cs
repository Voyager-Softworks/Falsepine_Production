using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Singleton donotdestroy script that handles the mission system
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    // list of inventories on this object
    [SerializeField] public List<Inventory> inventories = new List<Inventory>();

    private void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            LoadInventories();
        } else {
            Destroy(this);
            Destroy(gameObject);
        }

        Inventory[] invs = GetComponentsInChildren<Inventory>();
        foreach (Inventory inv in invs) {
            AddInventory(inv);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddInventory(Inventory inv) {
        // if inventory is already on this object, remove it first
        if (inventories.Contains(inv)) {
            inventories.Remove(inv);
        }
        
        inventories.Add(inv);
    }

    public void RemoveInventory(Inventory inv) {
        if (inventories.Contains(inv)) {
            inventories.Remove(inv);
        }
    }

    public Inventory GetInventory(string _id) {
        foreach (Inventory inv in inventories) {
            if (inv.id == _id) {
                return inv;
            }
        }
        return null;
    }

    public void SaveInventories() {
        foreach (Inventory inv in inventories) {
            inv.SaveInventory();
        }
    }

    public void LoadInventories() {
        foreach (Inventory inv in inventories) {
            inv.LoadInventory();
        }
    }
}
