using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Inventory : MonoBehaviour
{
    static public List<Inventory> allInventories = new List<Inventory>();

    private void Awake() {
        AddInventory(this);

        // if game is not running, convert items to instances.
        if (Application.isPlaying){
            ConvertItemsToInstances();
        }
    }

    private void OnDestroy() {
        RemoveInventory(this);
    }

    private void Start() {
        AddInventory(this);
    }

    [Serializable]
    public class InventorySlot
    {
        // type filter
        [SerializeField] public List<string> typeFilter = new List<string>();

        [SerializeField] private Item m_item = null;
        [SerializeField] public Item item
        {
            get { return m_item; }
            set
            {
                // if same, return
                if (m_item == value) return;
                
                m_item = value;

                // if null, return
                if (m_item == null) return;

                // if item type is not in filter, clear item
                if (typeFilter.Count() > 0 && !typeFilter.Contains(m_item.GetType().Name)) {
                    m_item = null;

                    Debug.LogWarning("Item type does not match inventory type filter.");

                    return;
                }

                if (m_item.instanceID == "")
                {
                    // if game is not running, return
                    if (!Application.isPlaying) return;

                    m_item = m_item.CreateInstance();
                }
            }
        }

        /// <summary>
        /// Attempt to add an item to this slot (matching filter), tries to stack if possible.
        /// </summary>
        public Item TryAddItemToSlot(Item _item)
        {
            // if item is null, return
            if (_item == null) return null;

            // if slot is not empty, try to stack
            if (this.item != null)
            {
                _item = this.item.TryAddToStack(_item);

                return _item;
            }

            // check if item type is in filter
            if (typeFilter.Count() > 0 && !typeFilter.Contains(_item.GetType().Name))
            {
                Debug.LogWarning("Item type does not match inventory type filter.");

                return _item;
            }

            // set item
            this.item = _item;

            return null;
        }
    }

    [SerializeField] private string m_id = "";
    [SerializeField] public string id
    {
        get { return m_id; }
        set
        {
            m_id = value;
            ValidateID();
        }
    }

    [SerializeField] private List<InventorySlot> m_slots = new List<InventorySlot>();
    [SerializeField] public List<InventorySlot> slots
    {
        get { return m_slots; }
        set { m_slots = value; }
    }

    /// <summary>
    /// Add an item to the inventory by reference.
    /// </summary>
    public Item AddItemToInventory(Item _item){
        // if item is null, return
        if (_item == null) return null;
        int startAmount = _item.currentStackSize;

        // loop through all slots and try to add item while not null
        for (int i = 0; i < m_slots.Count; i++)
        {
            InventorySlot slot = m_slots[i];
            
            _item = slot.TryAddItemToSlot(_item);
        }

        if (_item != null)
        {
            int endAmount = _item.currentStackSize;

            Debug.Log("Could not add all: Added " + (endAmount - startAmount) + "/" + startAmount + " of " + _item.name + " to inventory.");
        }

        return _item;
    }

    /// <summary>
    /// Tries to add an item to the inventory by ID.
    /// </summary>
    public bool AddItemToInventory(string _id, int _amount = 1){
        Item item = ItemDatabase.GetItem(_id);
        if (item == null) return false;

        item = item.CreateInstance();
        item.currentStackSize = _amount;

        item = AddItemToInventory(item);
        if (item == null) return true;

        return false;
    }

    /// <summary>
    /// Convert all items to instances.
    /// </summary>
    public void ConvertItemsToInstances()
    {
        for (int i = 0; i < m_slots.Count; i++)
        {
            InventorySlot slot = m_slots[i];

            if (slot.item != null)
            {
                slot.item = slot.item.CreateInstance();
            }
        }
    }

    public static string GetInventoryFolder(int saveSlot = 0){
        return Application.dataPath + "/saves/save" + saveSlot + "/inventories/";
    }

    /// <summary>
    /// Gets the save FOLDER path for this inventory.
    /// </summary>
    public string GetSavePath(int saveSlot = 0)
    {
        return GetInventoryFolder(saveSlot) + this.GetType().Name + id.ToString() + "/";
    }

    /// <summary>
    /// Gets the save FILE path for this inventory.
    /// </summary>
    public string GetSaveFilePath()
    {
        return GetSavePath() + "/inventory.json";
    }

    /// <summary>
    /// Saves this inventory to file.
    /// </summary>
    public void SaveInventory()
    {
        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSavePath()))
        {
            Directory.CreateDirectory(GetSavePath());
        }

        List<string> fileNames = new List<string>();

        foreach (InventorySlot slot in m_slots)
        {
            if (slot.item != null)
            {
                string fileName = slot.item.GetInstanceFileName();
                Item.Save(Item.GetInstanceSavePath(), fileName, slot.item);

                fileNames.Add(fileName);
            }
        }

        FileStream file = File.Create(GetSaveFilePath());

        //write to file
        StreamWriter writer = new StreamWriter(file);
        foreach (string fileName in fileNames)
        {
            writer.WriteLine(fileName);
        }
        writer.Close();
        file.Close();
    }

    /// <summary>
    /// Loads this inventory from file.
    /// </summary>
    public void LoadInventory()
    {
        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSavePath()))
        {
            Directory.CreateDirectory(GetSavePath());
        }
        // if save file doesn't exist, return
        if (!File.Exists(GetSaveFilePath()))
        {
            Debug.Log("Save file does not exist.");
            return;
        }

        // get file name
        string file = System.IO.File.ReadAllText(GetSaveFilePath());

        string[] fileNames = file.Split('\n');

        for (int i = 0; i < fileNames.Length; i++)
        {
            string fileName = fileNames[i];

            // remove newline
            fileName = fileName.Replace("\n", "");

            // remove whitespace
            fileName = fileName.Trim();

            if (fileName == "") continue;

            Item item = Item.Load(Item.GetInstanceSavePath(), fileName);

            if (item == null)
            {
                Debug.LogWarning("Item " + fileName + " could not be loaded. NULL");
                continue;
            }

            // for loading, we need to directly set the item
            if (m_slots.Count() > i)
            {
                m_slots[i].item = item;
            }
            else
            {
                Debug.LogWarning("Item " + fileName + " could not be loaded. NO SPACE");
            }
        }
    }

    /// <summary>
    /// Delete save file
    /// </summary>
    public void DeleteSaveFile()
    {
        if (File.Exists(GetSaveFilePath()))
        {
            File.Delete(GetSaveFilePath());
        }
    }

    /// <summary>
    /// Clears all items from the inventory.
    /// </summary>
    public void ClearInventory()
    {
        foreach (InventorySlot slot in m_slots)
        {
            slot.item = null;
        }
    }

    /// <summary>
    /// Validates the ID of the inventory.
    /// </summary>
    public void ValidateID(){
        string copy = id;

        //correct id
        //to lower
        m_id = m_id.ToLower();
        //replace non letters and numbers with ""
        m_id = Regex.Replace(m_id, @"[^a-zA-Z0-9_]", "");
        //replace space with _
        m_id = m_id.Replace(" ", "_");

        if (copy != m_id)
        {
            #if UNITY_EDITOR
            //set dirty
            EditorUtility.SetDirty(this);
            #endif
        }
    }

    /// <summary>
    /// Adds this inventory to the inventory list.
    /// </summary>
    private static void AddInventory(Inventory inventory)
    {
        if (allInventories.Contains(inventory))
        {
            // replace existing inventory
            allInventories[allInventories.IndexOf(inventory)] = inventory;
            return;
        }
        allInventories.Add(inventory);
    }

    /// <summary>
    /// Removes this inventory from the inventory list.
    /// </summary>
    private static void RemoveInventory(Inventory inventory)
    {
        if (!allInventories.Contains(inventory))
        {
            return;
        }
        allInventories.Remove(inventory);
    }

    /// <summary>
    /// Validates all inventory IDs.
    /// </summary>
    static public void ValidateIDs(Inventory _selectedInventory = null) {

        #if UNITY_EDITOR
        // if unity is building, don't validate
        if (EditorApplication.isCompiling)
        {
            return;
        }

        // put selected id at the top (Reason: so that when we check for issues, it will be the first to change, in an attempt to not modify other items )
        if (_selectedInventory != null)
        {
            var selectedIndex = allInventories.IndexOf(_selectedInventory);
            if (selectedIndex > 0 && selectedIndex < allInventories.Count)
            {
                var selectedItem = allInventories[selectedIndex];
                allInventories.RemoveAt(selectedIndex);
                allInventories.Insert(0, selectedItem);
            }
        }
        #endif

        //correct ID values
        for (int i = 0; i < allInventories.Count; i++) {
            allInventories[i].ValidateID();
        }

        //correct duplicate IDs
        for (int i = 0; i < allInventories.Count; i++) {
            Inventory inventory = allInventories[i];
            if (!inventory) continue;
            
            string copyID = inventory.id;

            for (int j = 0; j < allInventories.Count; j++) {
                if (allInventories[i].id == allInventories[j].id && i != j) {
                    //check if last char is a number
                    if (allInventories[i] && allInventories[i].id != "" && char.IsNumber(allInventories[i].id[allInventories[i].id.Length - 1])) {
                        //go backwards and count numbers
                        int count = 0;
                        for (int k = allInventories[i].id.Length - 1; k >= 0; k--) {
                            if (char.IsNumber(allInventories[i].id[k])) {
                                count++;
                            } else {
                                break;
                            }
                        }
                        //add one to the number
                        int number = int.Parse(allInventories[i].id.Substring(allInventories[i].id.Length - count, count)) + 1;
                        //remove last numbers
                        allInventories[i].id = allInventories[i].id.Substring(0, allInventories[i].id.Length - count);
                        //add new number
                        allInventories[i].id += number;

                        ValidateIDs();
                    }
                    else {
                        //add _1 to the number
                        allInventories[i].id = allInventories[i].id + "_1";
                    }
                }
            }

            if (copyID != inventory.id)
            {
                #if UNITY_EDITOR
                //set dirty
                EditorUtility.SetDirty(inventory);
                #endif
            }
        }
    }


    #if UNITY_EDITOR
    private void OnValidate() {
        // if not in scene, return
        if (!this.gameObject.scene.IsValid()) return;

        AddInventory(this);

        if (m_id == "") {
            m_id = System.Guid.NewGuid().ToString();

            ValidateID();

            // if not in play mode, save (set dirty)
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
            }
        }
    }

    // custom editor
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            Inventory inventory = (Inventory)target;

            //id
            inventory.id = EditorGUILayout.TextField(new GUIContent("ID","A unique identifier for this inventory"), inventory.id);

            // space
            EditorGUILayout.Space();

            // horiz
            EditorGUILayout.BeginHorizontal();
            //slots 
            EditorGUILayout.LabelField("Slots [" + inventory.m_slots.Count + "]", CustomEditorStuff.bold_label);
            
            EditorGUILayout.LabelField("Filter");
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < inventory.m_slots.Count; i++)
            {
                InventorySlot slot = inventory.m_slots[i];

                EditorGUILayout.BeginVertical("box");

                // horiz
                EditorGUILayout.BeginHorizontal();
                // labal for index
                EditorGUILayout.LabelField("Slot " + i, GUILayout.Width(50));

                //item
                slot.item = (Item)EditorGUILayout.ObjectField(slot.item, typeof(Item), false);

                //type filter
                // dropdown for item type
                // array of possible types
                string[] names = Item.AllTypes.Select(t => t.Name).ToArray();
                names = names.Concat(new string[] { typeof(Item).Name }).ToArray();
                // remove "item" from names
                names = names.Where(n => n != typeof(Item).Name).ToArray();
                // remove "item" from type filter
                slot.typeFilter = slot.typeFilter.Where(t => t != typeof(Item).Name).ToList();

                // display the GenericMenu when pressing a button
                int filterCount = slot.typeFilter.Count();
                if (GUILayout.Button(new GUIContent(filterCount == 1 ? slot.typeFilter[0] : filterCount > 1 ? "[Multiple]" : "Item", "Ticked types are allowed in this slot"), GUILayout.MinWidth(120)))
                {
                    // draw the dropdown
                    GenericMenu menu = new GenericMenu();
                    for (int j = 0; j < names.Length; j++)
                    {
                        string name = names[j];
                        menu.AddItem(new GUIContent(name), slot.typeFilter.Contains(name), () => { 
                            if (slot.typeFilter.Contains(name)) {
                                slot.typeFilter.Remove(name);
                            } else {
                                slot.typeFilter.Add(name);
                            }
                        });
                    }

                    // draw the dropdown
                    menu.ShowAsContext();
                }

                // remove button
                if (GUILayout.Button(new GUIContent("X", "Remove this slot"), GUILayout.Width(20)))
                {
                    inventory.m_slots.RemoveAt(i);
                    EditorUtility.SetDirty(inventory);
                    break;
                }
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();  
            }
            if (GUILayout.Button("Add Slot"))
            {
                inventory.m_slots.Add(new InventorySlot());
                EditorUtility.SetDirty(inventory);
            }

            // show total inventory count
            EditorGUILayout.LabelField("Total Inventory Count: " + allInventories.Count());

            // save on change
            if (GUI.changed)
            {
                // validate id
                Inventory.ValidateIDs();

                EditorUtility.SetDirty(inventory);
            }
        }
    }
    #endif
}