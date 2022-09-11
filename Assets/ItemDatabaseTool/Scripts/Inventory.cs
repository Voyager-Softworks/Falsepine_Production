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

/// <summary>
/// Manages storing and loading of items.
/// </summary>
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
                    //@todo add this check back in, remove to try create instances in editor
                    //if (!Application.isPlaying) return;

                    m_item = m_item.CreateInstance();
                }
            }
        }
        [SerializeField] public Inventory ownerInventory;

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
                //Debug.LogWarning("Item type does not match inventory type filter.");

                return _item;
            }

            // set item
            this.item = _item;

            return null;
        }

        public bool CanAddItemToSlot(Item _item)
        {
            // if item is null, return
            if (_item == null) return false;

            // if slot is not empty, try to stack
            if (this.item != null)
            {
                return this.item.CanAddItemToStack(_item);
            }

            // check if item type is in filter
            if (typeFilter.Count() > 0 && !typeFilter.Contains(_item.GetType().Name))
            {
                //Debug.LogWarning("Item type does not match inventory type filter.");

                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Gets all of the items in this inventory.
    /// </summary>
    /// <returns></returns>
    public List<Item> GetItems()
    {
        List<Item> items = new List<Item>();

        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null)
            {
                items.Add(slot.item);
            }
        }

        return items;
    }

    [SerializeField] private string m_id = "";

    public int GetItemIndex(Item item)
    {
        if (item == null) return -1;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// The unique ID of the inventory.
    /// </summary>
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

    [SerializeField] public bool m_infiniteSlots = false;

    public void SetSlotsOwner()
    {
        foreach (InventorySlot slot in slots)
        {
            slot.ownerInventory = this;
        }
    }

    /// <summary>
    /// Add an item to the inventory by reference.
    /// </summary>
    public Item TryAddItemToInventory(Item _item){
        // if item is null, return
        if (_item == null) return null;
        int startAmount = _item.currentStackSize;

        // loop through slots and try to add item to existing stack
        for (int i = 0; i < slots.Count; i++)
        {
            Item inSlot = slots[i].item;
            if (inSlot && _item && inSlot.id == _item.id)
            {
                _item = slots[i].TryAddItemToSlot(_item);
                if (_item == null)
                {
                    return null;
                }
            }
        }

        // loop through all slots and try to add item while not null
        for (int i = 0; i < m_slots.Count; i++)
        {
            InventorySlot slot = m_slots[i];
            
            _item = slot.TryAddItemToSlot(_item);
        }

        // if m_infiniteSlots, add a new slot
        if (m_infiniteSlots)
        {
            while (_item != null)
            {
                AddSlot();
                _item = m_slots[m_slots.Count - 1].TryAddItemToSlot(_item);
            }
        }

        if (_item != null)
        {
            int endAmount = _item.currentStackSize;

            //Debug.Log("Could not add all: Added " + (endAmount - startAmount) + "/" + startAmount + " of " + _item.name + " to inventory.");
        }

        return _item;
    }

    /// <summary>
    /// Checks if it is possible to add a given item to the inventory.
    /// </summary>
    /// <param name="_item"></param>
    /// <returns></returns>
    public bool CanAddItemToInventory(Item _item)
    {
        // if item is null, return
        if (_item == null) return false;

        // if infinite slots, return true
        if (m_infiniteSlots) return true;

        // loop through slots and try to add item to existing stack
        for (int i = 0; i < slots.Count; i++)
        {
            Item inSlot = slots[i].item;
            if (inSlot && _item && inSlot.id == _item.id)
            {
                if (inSlot.CanAddItemToStack(_item)){
                    return true;
                }
            }
        }

        // loop through all slots and try to add item while not null
        for (int i = 0; i < m_slots.Count; i++)
        {
            InventorySlot slot = m_slots[i];

            if (slot.CanAddItemToSlot(_item))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to add an item to the inventory by ID.
    /// </summary>
    public bool TryAddItemToInventory(string _id, int _amount = 1){
        Item item = ItemDatabase.GetItem(_id);
        if (item == null) return false;

        item = item.CreateInstance();
        item.currentStackSize = _amount;

        item = TryAddItemToInventory(item);
        if (item == null) return true;

        return false;
    }

    /// <summary>
    /// Removes an item from the inventory by index.
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public Item RemoveItemFromInventory(int _index){
        if (_index < 0 || _index >= m_slots.Count) return null;

        InventorySlot slot = m_slots[_index];
        Item item = slot.item;

        slot.item = null;

        return item;
    }

    /// <summary>
    /// Fill the ammo of every ranged weapon to max.
    /// </summary>
    public void FillAmmo(){
        foreach (Inventory.InventorySlot slot in slots)
        {
            Item item = slot.item;
            // if item is ranged weapon
            if (item != null && item as RangedWeapon != null)
            {
                RangedWeapon weapon = item as RangedWeapon;
                weapon.m_clipAmmo = weapon.m_clipSize;
                weapon.m_spareAmmo = weapon.m_maxSpareAmmo;
            }
        }
    }

    /// <summary>
    /// Adds a slot with the given filter to the inventory.
    /// </summary>
    /// <param name="_filter"></param>
    public void AddSlot(List<System.Type> _filter = null)
    {
        if (_filter == null)
        {
            _filter = new List<System.Type>();
        }
        // remove any types that do not derive from Item, or are Item
        List<System.Type> filteredTypes = new List<System.Type>();
        foreach (System.Type type in _filter)
        {
            if (type.IsSubclassOf(typeof(Item)) && type != typeof(Item))
            {
                filteredTypes.Add(type);
            }
        }

        // remove any duplicates
        filteredTypes = filteredTypes.Distinct().ToList();

        // convert to string list
        List<string> filteredStrings = new List<string>();
        foreach (System.Type type in filteredTypes)
        {
            filteredStrings.Add(type.ToString());
        }

        AddSlot(filteredStrings);
    }

    private void AddSlot(List<string> _filters)
    {
        InventorySlot slot = new InventorySlot();

        // convert Item.AllTypes to a list of strings
        List<string> allTypes = new List<string>();
        foreach (System.Type type in Item.AllTypes)
        {
            allTypes.Add(type.ToString());
        }
        // if any filters are NOT present in allTypes, then remove them
        for (int i = _filters.Count - 1; i >= 0; i--)
        {
            if (allTypes.Contains(_filters[i]))
            {
                continue;
            }
            _filters.RemoveAt(i);
        }

        slot.typeFilter = new List<String>(_filters);
        slot.ownerInventory = this;
        slots.Add(slot);
    }

    /// <summary>
    /// Gets the amount of slots in this inventory.
    /// </summary>
    /// <returns></returns>
    internal int GetSlotCount()
    {
        return m_slots.Count();
    }

    /// <summary>
    /// Gets a slot by index
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public InventorySlot GetSlot(int _index){
        if (_index < 0 || _index >= m_slots.Count) return null;
        return m_slots[_index];
    }

    /// <summary>
    /// Fill the stacks of every item to max.
    /// </summary>
    public void FillStacks(){
        foreach (Inventory.InventorySlot slot in slots)
        {
            Item item = slot.item;

            // set item stack size to max
            if (item != null)
            {
                item.currentStackSize = item.maxStackSize;
            }
        }
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

    public static string GetInventoryFolder(int saveSlot){
        return SaveManager.GetSaveFolderPath(saveSlot) + "/inventories/";
    }

    /// <summary>
    /// Gets the save FOLDER path for this inventory.
    /// </summary>
    public string GetSaveFolderPath(int saveSlot)
    {
        return GetInventoryFolder(saveSlot) + this.GetType().Name + id.ToString() + "/";
    }

    /// <summary>
    /// Gets the save FILE path for this inventory.
    /// </summary>
    public string GetSaveFilePath(int saveSlot)
    {
        return GetSaveFolderPath(saveSlot) + "/inventory.json";
    }

    /// <summary>
    /// Saves this inventory to file.
    /// </summary>
    public void SaveInventory(int saveSlot)
    {
        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }

        List<string> slotsInfo = new List<string>();

        // save each item to a new line
        foreach (InventorySlot slot in m_slots)
        {
            string itemFileName = "";

            if (slot.item != null)
            {
                itemFileName = slot.item.GetInstanceFileName(saveSlot);
                Item.Save(Item.GetInstanceSavePath(saveSlot), itemFileName, slot.item);
            }

            // get the filter of this slot
            List<string> filters = new List<string>(slot.typeFilter);
            string filter = string.Join(",", filters.ToArray());

            slotsInfo.Add(filter + ":" + itemFileName);
        }

        FileStream file = File.Create(GetSaveFilePath(saveSlot));

        //write to file
        StreamWriter writer = new StreamWriter(file);
        for (int i = 0; i < slotsInfo.Count; i++)
        {
            //if not last item, write new line
            if (i < slotsInfo.Count - 1)
            {
                writer.WriteLine(slotsInfo[i]);
            }
            else
            {
                writer.Write(slotsInfo[i]);
            }
        }

        writer.Close();
        file.Close();
    }

    /// <summary>
    /// Loads this inventory from file.
    /// </summary>
    public void LoadInventory(int saveSlot)
    {
        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(saveSlot));
        }
        // if save file doesn't exist, return
        if (!File.Exists(GetSaveFilePath(saveSlot)))
        {
            Debug.Log("Save file does not exist.");
            return;
        }

        // get file name
        string file = System.IO.File.ReadAllText(GetSaveFilePath(saveSlot));

        string[] slotsInfo = file.Split('\n');

        // clear inventory
        ClearInventory();
        slots = new List<InventorySlot>();

        for (int i = 0; i < slotsInfo.Length; i++)
        {
            // split file name into filters and file name
            string[] split = slotsInfo[i].Split(':');
            if (split.Length < 2) continue;
            string filters = split[0];
            string fileName = split[1];

            // remove newline
            fileName = fileName.Replace("\n", "");
            // remove whitespace
            fileName = fileName.Trim();

            // convert filters to list of strings
            List<string> filterList = new List<string>(filters.Split(','));

            // add slot to inventory
            AddSlot(filterList);

            if (fileName == "") continue;

            Item item = Item.Load(Item.GetInstanceSavePath(saveSlot), fileName);

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
    public void DeleteSaveFile(int saveSlot)
    {
        if (File.Exists(GetSaveFilePath(saveSlot)))
        {
            File.Delete(GetSaveFilePath(saveSlot));
        }
    }

    /// <summary>
    /// Clears all items from the inventory, but leaves slots.
    /// </summary>
    public void ClearInventory()
    {
        foreach (InventorySlot slot in m_slots)
        {
            slot.item = null;
        }
    }

    /// <summary>
    /// Resets the inventory to empty, including removing all slots.
    /// </summary>
    public void ResetInventory(){
        ClearInventory();
        slots = new List<InventorySlot>();
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

        //remove null inventories
        for (int i = allInventories.Count - 1; i >= 0; i--)
        {
            if (allInventories[i] == null)
            {
                allInventories.RemoveAt(i);
            }
        }

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

        SetSlotsOwner();
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

            // infinite slots
            inventory.m_infiniteSlots = EditorGUILayout.Toggle(new GUIContent("Infinite Slots", "If true, the inventory will make new slots when adding items"), inventory.m_infiniteSlots);

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

                // if slot owner does not exist
                if (!slot.ownerInventory)
                {
                    inventory.SetSlotsOwner();
                    EditorUtility.SetDirty(inventory);
                }

                EditorGUILayout.EndVertical();  
            }
            if (GUILayout.Button("Add Slot"))
            {
                inventory.m_slots.Add(new InventorySlot());
                inventory.SetSlotsOwner();
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