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

    private void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);

            Inventory[] invs = GetComponentsInChildren<Inventory>();
            foreach (Inventory inv in invs) {
                AddInventory(inv);
            }

            LoadInventories(SaveManager.currentSaveSlot);
        } else {
            Destroy(this);
            Destroy(gameObject);
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

    public void SaveInventories(int saveSlot) {
        foreach (Inventory inv in inventories) {
            inv.SaveInventory(saveSlot);
        }
    }

    public void TryMoveItem(Inventory source, Inventory target, int index) {
        Item item = source.slots[index].item;
        if (target.CanAddItemToInventory(item)) {
            source.RemoveItemFromInventory(index);
            Item leftover = target.TryAddItemToInventory(item);
            if (leftover != null) {
                source.TryAddItemToInventory(leftover);
            }
        }
    }

    public bool CanMoveItem(Inventory source, Inventory target, int index) {
        Item item = source.slots[index].item;
        return target.CanAddItemToInventory(item);
    }

    /// <summary>
    /// Loads inventories from file
    /// </summary>
    /// <param name="saveSlot"></param>
    public void LoadInventories(int saveSlot) {
        foreach (Inventory inv in inventories) {
            inv.LoadInventory(saveSlot);
        }
    }

    // custom unity editor
    // #if UNITY_EDITOR
    // [CustomEditor(typeof(InventoryManager))]
    // public class InventoryManagerEditor : Editor
    // {
    //     public override void OnInspectorGUI()
    //     {
    //         DrawDefaultInspector();

    //         InventoryManager myScript = (InventoryManager)target;

    //         // list of prefabs
    //         UnityEditor.EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);
    //         foreach (GameObject obj in prefabInstances)
    //         {
    //             UnityEditor.EditorGUILayout.LabelField(obj.name);
    //         }

    //         // add prefabs to list
    //         if (GUILayout.Button("Add Prefabs to List"))
    //         {
    //             myScript.AddPrefabsToListList();
    //         }

    //         // clear prefab list
    //         if (GUILayout.Button("Clear Prefab List"))
    //         {
    //             InventoryManager.prefabInstances.Clear();
    //         }

    //         // on change, set dirty
    //         if (GUI.changed)
    //         {
    //             EditorUtility.SetDirty(target);
    //         }
    //     }
    // }
    // #endif
}
