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

            //LoadMissions();
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Inventory[] invs = GetComponentsInChildren<Inventory>();
        foreach (Inventory inv in invs) {
            inventories.Add(inv);
        }
    }
}
