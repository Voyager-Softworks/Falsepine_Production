using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A class that will replace the "GridItem" and "InventoryCell" classes, which will display the items in the inventory.
/// @todo finish this class
/// </summary>
public class ItemDisplay : MonoBehaviour
{
    public string m_linkedInventoryID = "";
    public int m_slotNumber = 0;

    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    public Inventory m_linkedInventory;

    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    public Item m_linkedItem = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Updates the linked values (inventory and item) from the ID and slot number.
    /// </summary>
    public void UpdateLinkedValues(){
        m_linkedInventory = InventoryManager.instance.GetInventory(m_linkedInventoryID);
        if (m_linkedInventory == null) return;

        Inventory.InventorySlot slot = m_linkedInventory.GetSlot(m_slotNumber);
        if (slot == null) return;

        m_linkedItem = slot.item;
    }

    // custom editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(ItemDisplay))]
    public class ItemDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ItemDisplay script = (ItemDisplay)target;
            if (GUILayout.Button("Try Link Slot"))
            {
                script.UpdateLinkedValues();
            }
        }
    }
    #endif
}
