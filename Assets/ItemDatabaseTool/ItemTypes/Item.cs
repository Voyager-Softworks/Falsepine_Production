using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using Achievements;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// The base class for all items. Lets dynamic items be created, and automatically generates adds and manages the item's data.
/// </summary>
[Serializable]
public class Item : ScriptableObject, StatsManager.UsesStats, StatsManager.HasStatMods, EconomyManager.Purchasable
{
    // List of all the item classes in the game
    [NonSerialized] public static readonly IEnumerable<System.Type> AllTypes;

    /// <summary>
    /// Gets the folder to save item instances to.
    /// </summary>
    public static string GetInstanceSavePath(int saveSlot){
        return SaveManager.GetSaveFolderPath(saveSlot) + "/instances/";
    }

    /// <summary>
    /// Gets the potential file name for this item instance.
    /// </summary>
    public string GetInstanceFileName(int saveSlot){
        return this.id + this.instanceID + ".json";
    }

    /// <summary>
    /// Constructor for Item class
    /// </summary>
    static Item()
    {
        // Keep track of all the item classes in the game
        System.Type type = typeof(Item);
        AllTypes = type.Assembly.GetTypes().Where(t => t.IsSubclassOf(type));
    }

    //simple constructor
    public Item()
    {
        //set default values
        id = this.GetType().Name + "_0";
        m_displayName = "New Item EE";
        m_description = "New Item Description";
        m_icon = null;
    }

    [NonSerialized] private bool drawDefaultInspector = false;

    //unique id of the item
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

    //unique instance id of the item
    [SerializeField] private string m_instanceID = "";
    [SerializeField] public string instanceID
    {
        get { return m_instanceID; }
        set
        {
            m_instanceID = value;
        }
    }
    
    public virtual string GetTypeDisplayName(){
        return "Item";
    }

    [SerializeField] public string m_displayName = "";
    [SerializeField] public string m_description = "";
    [SerializeField] public Sprite m_icon = null;

    [SerializeField] public bool mustMatchToStack = false;
    [SerializeField] private int m_currentStackSize = 1;
    public int currentStackSize
    {
        get { return m_currentStackSize; }
        set
        {
            m_currentStackSize = value;

            if (m_currentStackSize <= 0)
            {
                m_currentStackSize = 0;
            }

            if (m_currentStackSize > m_maxStackSize)
            {
                m_currentStackSize = m_maxStackSize;
            }
        }
    }
    [SerializeField] private int m_maxStackSize = 1;
    public int maxStackSize
    {
        get { return m_maxStackSize; }
        set
        {
            m_maxStackSize = value;

            if (m_maxStackSize < 1)
            {
                m_maxStackSize = 1;
            }

            if (m_maxStackSize < m_currentStackSize)
            {
                m_currentStackSize = m_maxStackSize;
            }
        }
    }

    [SerializeField] public List<TagManager.Tag> m_tags = new List<TagManager.Tag>();


    // StatsManager.UsesStats interface implementation
    [SerializeField] public List<StatsManager.StatType> m_usedStatTypes = new List<StatsManager.StatType>();
    public virtual List<StatsManager.StatType> GetStatTypes()
    {
        return m_usedStatTypes;
    }
    /// <summary>
    /// Adds a stat type to the list of used stat types if it isn't already in the list.
    /// </summary>
    /// <param name="statType"></param>
    public void AddStatType(StatsManager.StatType statType)
    {
        if (statType == null) return;

        if (!m_usedStatTypes.Contains(statType))
        {
            m_usedStatTypes.Add(statType);
        }
    }
    /// <summary>
    /// Removes a stat type from the list of used stat types if it is in the list.
    /// </summary>
    /// <param name="statType"></param>
    public void RemoveStatType(StatsManager.StatType statType)
    {
        if (m_usedStatTypes.Contains(statType))
        {
            m_usedStatTypes.Remove(statType);
        }
    }

    // StatsManager.HasStatMods interface implementation
    [SerializeField] public List<StatsManager.StatMod> m_statMods = new List<StatsManager.StatMod>();
    public List<StatsManager.StatMod> GetStatMods()
    {
        return m_statMods;
    }

    // EconomyManager.Purchasable interface implementation
    public int m_price = 0;
    public bool m_allowedDiscount = true;
    public int GetPrice()
    {
        return StatsManager.CalculateCost(this, m_price);
    }
    public bool GetAllowedDiscount()
    {
        return m_allowedDiscount;
    }

    public AchievementsManager.Achievement m_unlockAchievement = AchievementsManager.Achievement.None;


    [Serializable]
    public class FieldResourceLink{
        [SerializeField] public string fieldName = "";
        [SerializeField] public string resourcePath = "";
    }
    [SerializeField] public List<FieldResourceLink> m_resourceLinks = new List<FieldResourceLink>();

    /// <summary>
    /// Update function for the item
    /// </summary>
    public virtual void ManualUpdate(GameObject _owner)
    {
        // Update the item
    }

    /// <summary>
    /// Destroys the item instance, and deletes it from file.
    /// </summary>
    public void DestroyInstance(int saveSlot)
    {
        Debug.Log("Destroying instance of item: " + this.id);

        if (string.IsNullOrEmpty(instanceID))
        {
            return;
        }

        string path = GetInstanceSavePath(saveSlot) + GetInstanceFileName(saveSlot);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        Destroy(this);
    }

    /// <summary>
    /// Attempts to stack the given item with this one.
    /// </summary>
    /// <param name="_item">The item to stack with this one.</param>
    /// <returns>If fully stacked, returns null, otherwise returns back the item minus the amount that was stacked</returns>
    public Item TryAddToStack(Item _item){

        if (!CanAddItemToStack(_item))
        {
            return _item;
        }

        // add to stack as long as there is room
        int amountToAdd = _item.currentStackSize;
        int spareSpace = m_maxStackSize - m_currentStackSize;
        int amountToAddToStack = Mathf.Min(amountToAdd, spareSpace);

        if (amountToAddToStack > 0)
        {
            _item.currentStackSize -= amountToAddToStack;
            m_currentStackSize += amountToAddToStack;
        }

        // if the item is now empty, return null
        if (_item.currentStackSize <= 0)
        {
            return null;
        }

        // otherwise return the item minus the amount that was stacked
        return _item;
    }

    public bool CanAddItemToStack(Item _item)
    {
        // check invalid type
        if (this.GetType() != _item.GetType() || this.id != _item.id)
        {
            //Debug.LogWarning("Trying to add an item of a different type or id to a stack");
            return false;
        }

        // compare strings
        if (mustMatchToStack)
        {
            Item thisTemp = this.CreateInstance();
            thisTemp.instanceID = "";
            thisTemp.currentStackSize = 0;

            Item otherTemp = _item.CreateInstance();
            otherTemp.instanceID = "";
            otherTemp.currentStackSize = 0;

            // convert to json
            string thisJson = JsonUtility.ToJson(thisTemp);
            string otherJson = JsonUtility.ToJson(otherTemp);

            // compare json
            if (thisJson != otherJson)
            {
                //Debug.LogWarning("Trying to add an item with different data to a stack");
                return false;
            }
        }

        // add to stack as long as there is room
        int amountToAdd = _item.currentStackSize;
        int spareSpace = m_maxStackSize - m_currentStackSize;
        int amountToAddToStack = Mathf.Min(amountToAdd, spareSpace);

        if (amountToAddToStack > 0)
        {
            return true;
        }

        // if the item is now empty, return null
        return false;
    }

    /// <summary>
    /// Validates the id of the item.
    /// </summary>
    public void ValidateID(){
        //correct id
        //to lower
        m_id = m_id.ToLower();
        //replace non letters and numbers with ""
        m_id = Regex.Replace(m_id, @"[^a-zA-Z0-9_-]", "");
        //replace space with _
        m_id = m_id.Replace(" ", "_");
    }

    public void ValidateInstanceID()
    {
        //correct id
        //to lower
        m_instanceID = m_instanceID.ToLower();
        //replace non letters and numbers with ""
        m_instanceID = Regex.Replace(m_instanceID, @"[^a-zA-Z0-9_-]", "");
        //replace space with _
        m_instanceID = m_instanceID.Replace(" ", "_");
    }

    /// <summary>
    /// Make instance of this item.
    /// </summary>
    /// <returns>The instance.</returns>
    public virtual Item CreateInstance()
    {
        string typeName = this.GetType().Name;
        Item item = (Item)Item.CreateInstance(typeName);

        item.instanceID = Guid.NewGuid().ToString();
        item.m_id = this.id;
        item.m_displayName = this.m_displayName;
        item.m_description = this.m_description;
        item.m_icon = this.m_icon;
        // NOTE: Make sure to COPY lists like below, not pass by reference
        item.m_tags = new List<TagManager.Tag>(this.m_tags);

        item.m_usedStatTypes = new List<StatsManager.StatType>(this.m_usedStatTypes);
        item.m_statMods = new List<StatsManager.StatMod>(this.m_statMods);

        item.m_price = this.m_price;
        item.m_allowedDiscount = this.m_allowedDiscount;

        item.m_unlockAchievement = this.m_unlockAchievement;

        item.maxStackSize = this.m_maxStackSize;
        item.currentStackSize = this.m_currentStackSize;
        item.mustMatchToStack = this.mustMatchToStack;

        if (item.name == "")
        {
            item.name = item.id;
        }

        return item;
    }

    // list of valid types (GameObject, Sprite)
    List<Type> validResourceTypes = new List<Type>() {
        typeof(GameObject),
        typeof(Sprite)
    };
    #if UNITY_EDITOR

    public void PrefabsToResourceList(){
        m_resourceLinks.Clear();
        //m_resources.Clear();

        // get all GameObject varibles from item
        List<FieldInfo> fields = new List<FieldInfo>(this.GetType().GetFields());
        foreach (FieldInfo field in fields)
        {
            AddResourceToList(field);
        }
    }

    // generic method to add a field to the list without knowing the type
    public void AddResourceToList(FieldInfo field)
    {
        // TRY get obj from field
        UnityEngine.Object obj = null;
        try
        {
            obj = (UnityEngine.Object)field.GetValue(this);
        }
        catch (Exception e)
        {
            // this is expected if the field is not a UnityEngine.Object

            //Debug.LogWarning("Could not get value from field: " + field.Name + " in item: " + this.id + " - " + e.Message);
        }

        // if obj is null, skip
        if (obj == null)
        {
            return;
        }

        // check if type is valid
        if (!validResourceTypes.Contains(field.FieldType))
        {
            return;
        }

        // get path to texture (remove path before "Resources" and remove extension)
        string path = AssetDatabase.GetAssetPath(obj);
        // check if Resource folder is in path
        int index = path.IndexOf("Resources");
        if (index == -1)
        {
            Debug.LogError("Resource " + obj.name + " is not in a Resources folder. Check Item " + this.id);
            return;
        }
        path = path.Substring(index + 10);
        path = path.Substring(0, path.LastIndexOf("."));

        // add resource link
        FieldResourceLink link = new FieldResourceLink();
        link.fieldName = field.Name;
        link.resourcePath = path;
        m_resourceLinks.Add(link);
    }

    #endif

    public void ResourceListToPrefabs(){
        foreach (FieldResourceLink link in m_resourceLinks)
        {
            FieldInfo field = this.GetType().GetField(link.fieldName);
            if (field == null) continue;

            LoadResourceIntoField(field, link.resourcePath);
        }
    }

    public void LoadResourceIntoField(FieldInfo field, string resourcePath){
        // check if type is valid
        if (!validResourceTypes.Contains(field.FieldType))
        {
            return;
        }

        UnityEngine.Object value = Resources.Load(resourcePath, field.FieldType);

        if (value != null)
        {   
            if (field != null)
            {
                field.SetValue(this, value);
            }
        }
    }

    public void MovePrefabsToResourceFolder(){
        #if UNITY_EDITOR
        // get all GameObject varibles from item    
        List<FieldInfo> fields = new List<FieldInfo>(this.GetType().GetFields());
        foreach (FieldInfo field in fields)
        {
            string newRoot = "Assets/Resources/";

            // Prefabs:
            if (field.FieldType == typeof(GameObject))
            {
                GameObject prefab = (GameObject)field.GetValue(this);
                if (prefab != null)
                {
                    // AutoSound:
                    if (prefab.GetComponent<AutoSound>() != null)
                    {
                        newRoot += "AutoSounds/";
                    }

                    // check if root folder exists
                    if (!Directory.Exists(newRoot))
                    {
                        Directory.CreateDirectory(newRoot);
                    }

                    // move prefab to resources folder
                    UnityEditor.AssetDatabase.MoveAsset(UnityEditor.AssetDatabase.GetAssetPath(prefab), newRoot + prefab.name + ".prefab");

                    continue;
                }
            }

            // Sprites: 
            if (field.FieldType == typeof(Sprite))
            {
                Sprite sprite = (Sprite)field.GetValue(this);
                if (sprite != null)
                {
                    // Icon:
                    if (field.Name == "m_icon")
                    {
                        newRoot += "Icons/";
                    }

                    // check if root folder exists
                    if (!Directory.Exists(newRoot))
                    {
                        Directory.CreateDirectory(newRoot);
                    }

                    // move sprite to resources folder
                    UnityEditor.AssetDatabase.MoveAsset(UnityEditor.AssetDatabase.GetAssetPath(sprite), newRoot + sprite.name + ".png");

                    continue;
                }
            }
        }

        // get all sprite varibles from item
        fields = new List<FieldInfo>(this.GetType().GetFields());
        foreach (FieldInfo field in fields)
        {
            
        }
        #endif
    }

    /// <summary>
    /// Saves item to file
    /// </summary>
    /// <param name="_fileName">path to save to</param>
    public static void Save(string _path, string _fileName, Item _item)
    {
        // if save path doesn't exist, create it
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }

        FileStream file = File.Create(_path + _fileName);

        // only store the true values of the item if this is in engine, otherwise we will need to steal the values from the database
        #if UNITY_EDITOR
        _item.PrefabsToResourceList();
        #else
        // get item from database
        List<Item> possibleItems = ItemDatabase.GetItemsByName(_item.m_displayName);
        // find item that matches the display name exactly
        Item itemFromDatabase = possibleItems.Find(x => x.m_displayName == _item.m_displayName);
        if (itemFromDatabase != null)
        {
            // copy resource links from database item
            _item.m_resourceLinks = new List<FieldResourceLink>(itemFromDatabase.m_resourceLinks);
        }
        #endif
        

        //serialize item
        string json = JsonUtility.ToJson(_item, true);
        string itemType = _item.GetType().Name;

        //make writer
        StreamWriter writer = new StreamWriter(file);
        // write the item type
        writer.Write(itemType);
        // new line
        writer.Write("\n");
        // write to file
        writer.Write(json);
        writer.Close();

        file.Close();
    }

    /// <summary>
    /// Loads item from file
    /// </summary>
    /// <param name="_fileName">path to load from</param>
    /// <returns>Item loaded from file</returns>
    public static Item Load(string _path, string _fileName)
    {
        Debug.Log("Trying to open" + _path + _fileName);

        // if file doesn't exist, return null
        if (!File.Exists(_path + _fileName))
        {
            Debug.Log("File doesn't exist");
            return null;
        }

        //read file
        FileStream file = File.Open(_path + _fileName, FileMode.Open);
        StreamReader reader = new StreamReader(file);
        string itemType = reader.ReadLine();
        string json = reader.ReadToEnd();
        reader.Close();
        file.Close();

        Debug.Log("Loading " + itemType);

        // make scritableobject of same type
        ScriptableObject item = ScriptableObject.CreateInstance(itemType);

        // load data into item
        JsonUtility.FromJsonOverwrite(json, item);

        // cast item to Item
        item = (Item)item;

        if (item && ((Item)item).name == "")
        {
            ((Item)item).name = ((Item)item).id;
        }

        ((Item)item).ResourceListToPrefabs();

        // cast to item type
        return item as Item;
    }


    //Custom editor for item
    #if UNITY_EDITOR
    [CustomEditor(typeof(Item))]
    public class ItemEditor : Editor
    {
        static bool showTags = true;

        public override void OnInspectorGUI()
        {
            DrawUI();
        }

        /// <summary>
        /// Draws the base UI for the item.
        /// </summary>
        private void DrawUI()
        {
            Item item = (Item)target;

            // start recording stats
            EditorGUI.BeginChangeCheck();

            // Draw default inspector option
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            item.drawDefaultInspector = GUILayout.Toggle(item.drawDefaultInspector, "Default Inspector");
            // create instance button
            if (GUILayout.Button("Create New Instance"))
            {
                // find the item in the asset database and create a copy
                string path = UnityEditor.AssetDatabase.GetAssetPath(item);
                string newPath = path.Replace(".asset", " (Copy).asset");
                UnityEditor.AssetDatabase.CopyAsset(path, newPath);

                // get the new item
                Item newItem = UnityEditor.AssetDatabase.LoadAssetAtPath<Item>(newPath);

                // give the new item a new instance id
                newItem.m_instanceID = System.Guid.NewGuid().ToString();

                // set dirty
                UnityEditor.EditorUtility.SetDirty(newItem);

                // set the new item's name
                SetFileName(newItem);
            }
            // load data from file
            if (GUILayout.Button(new GUIContent("Load Data", "Loads data from file"), GUILayout.Width(120)))
            {
                // ask user for file
                string path = EditorUtility.OpenFilePanel("Load Item", Application.dataPath + "/Resources/Items/", "json");
                if (path.Length != 0)
                {
                    LoadDataFromFile(item, path);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (item.drawDefaultInspector)
            {
                DrawDefaultInspector();
            }


            //green box for base item class
            GUI.backgroundColor = Color.green;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;

            // Top label
            GUILayout.Label("Base Item Stats", CustomEditorStuff.center_bold_label);

            // Parameters
            GUILayout.BeginHorizontal();
            item.id = EditorGUILayout.TextField("ID: ", item.id);
            // update file name button
            if (GUILayout.Button(new GUIContent("Update File Name", "Updates the file name to match the item id"), GUILayout.Width(120)))
            {
                SetFileName(item);
            }
            GUILayout.EndHorizontal();

            // horiz
            GUILayout.BeginHorizontal();
            // disabled varint id
            EditorGUI.BeginDisabledGroup(item.instanceID == "");
            item.instanceID = EditorGUILayout.TextField(new GUIContent("Instance ID: ", " it something human readable e.g. 'legendary'\nSimply delete this ID to revert it to a normal item"), item.instanceID);
            EditorGUI.EndDisabledGroup();
            // generate instance id button
            EditorGUI.BeginDisabledGroup(item.instanceID != "");
            if (GUILayout.Button(new GUIContent("Convert>Instance", "Converts THIS item to an instance, by generating a new instance id for the item"), GUILayout.Width(120)))
            {
                item.instanceID = System.Guid.NewGuid().ToString();
                EditorUtility.SetDirty(item);
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            item.m_displayName = EditorGUILayout.TextField("Display Name: ", item.m_displayName);
            item.m_description = EditorGUILayout.TextField("Description: ", item.m_description);
            item.m_icon = EditorGUILayout.ObjectField("Icon: ", item.m_icon, typeof(Sprite), false) as Sprite;

            // Economy box
            GUILayout.BeginVertical("box");
            // bold text
            GUILayout.Label("Economy", CustomEditorStuff.center_bold_label);
            // horiz
            GUILayout.BeginHorizontal();
            // price
            GUILayout.Label("Price: ", GUILayout.Width(50));
            item.m_price = EditorGUILayout.IntField(item.m_price);
            //space
            GUILayout.Space(10);
            // discount
            GUILayout.Label("Allowed Discount? ");
            item.m_allowedDiscount = EditorGUILayout.Toggle(item.m_allowedDiscount);
            // end horiz
            GUILayout.EndHorizontal();


            // End Economy box
            GUILayout.EndVertical();


            // stack info box
            GUILayout.BeginVertical("box");
            // bold text
            GUILayout.Label("Stack Info", CustomEditorStuff.center_bold_label);

            // horiz
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Current Stack: ", "The current amount of items in this stack"));
            item.currentStackSize = EditorGUILayout.IntField(item.currentStackSize);

            GUILayout.Label(new GUIContent("Max Stack: ", "The maximum amount of items in this stack"));
            item.maxStackSize = EditorGUILayout.IntField(item.maxStackSize);
            GUILayout.EndHorizontal();

            item.mustMatchToStack = EditorGUILayout.Toggle(new GUIContent("Must Match To Stack: ", "Items of the same type must match in all other spects to stack"), item.mustMatchToStack);

            // end stack info box
            GUILayout.EndVertical();

            // tags box
            GUILayout.BeginVertical("box");
            // bold text
            GUILayout.Label("Tags", CustomEditorStuff.center_bold_label);

            //tags list
            EditorGUI.indentLevel++;
            showTags = EditorGUILayout.Foldout(showTags, "Tags", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            EditorGUI.indentLevel--;
            if (showTags)
            {
                GUI.backgroundColor = Color.white;
                GUILayout.BeginVertical("box");
                //draw tags
                for (int i = 0; i < item.m_tags.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("Tag Name: ", "The name of this tag"));
                    item.m_tags[i].name = EditorGUILayout.TextField(item.m_tags[i].name);
                    GUILayout.Label(new GUIContent("Payload: ", "The value of this tag"));
                    item.m_tags[i].payload = EditorGUILayout.TextField(item.m_tags[i].payload);
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        item.m_tags.RemoveAt(i);
                        // save
                        // if not in play mode, save (set dirty)
                        if (!Application.isPlaying)
                        {
                            EditorUtility.SetDirty(item);
                        }
                        break;
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Tag"))
                {
                    item.m_tags.Add(new TagManager.Tag("New Tag"));

                    int c = ItemDatabase.database.Count();

                    // save
                    // if not in play mode, save (set dirty)
                    if (!Application.isPlaying)
                    {
                        EditorUtility.SetDirty(item);
                    }
                }
                GUILayout.EndVertical();
            }

            //end tags box
            GUILayout.EndVertical();

            // stats info box
            GUILayout.BeginVertical("box");
            // bold text
            GUILayout.Label("Stats", CustomEditorStuff.center_bold_label);

            bool needToSave = false;
            StatsManager.StatTypeListDropdown(item, out needToSave);
            if (needToSave)
            {
                EditorUtility.SetDirty(item);
            }

            // copy paste buttons
            //horiz
            GUILayout.BeginHorizontal();
            //flex space
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Copy Stats To Clipboard", "Copies the stats to the clipboard"), GUILayout.MaxWidth(200)))
            {
                string stats = "";
                foreach (StatsManager.StatType stat in item.m_usedStatTypes)
                {
                    stats += stat.value + "\n";
                }
                EditorGUIUtility.systemCopyBuffer = stats;
            }
            if (GUILayout.Button(new GUIContent("Paste Stats From Clipboard", "Pastes the stats from the clipboard"), GUILayout.MaxWidth(200)))
            {
                string[] stats = EditorGUIUtility.systemCopyBuffer.Split('\n');
                for (int i = 0; i < stats.Length; i++)
                {
                    item.AddStatType(StatsManager.StatType.StringToStatType(stats[i]));
                }
                EditorUtility.SetDirty(item);
            }

            //flex space
            GUILayout.FlexibleSpace();
            //end horiz
            GUILayout.EndHorizontal();


            // space
            GUILayout.Space(10);

            GUILayout.Label("Stat Mods", CustomEditorStuff.center_bold_label);

            // stat mods (used property field)
            EditorGUI.indentLevel++;
            // use DrawStatMod
            if (StatsManager.DrawStatModList(item.m_statMods))
            {
                // save
                // if not in play mode, save (set dirty)
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(item);
                }
            }
            EditorGUI.indentLevel--;

            // end recording stats
            if (EditorGUI.EndChangeCheck())
            {
                // save
                // if not in play mode, save (set dirty)
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(item);
                }
            }

            // end stats info box
            GUILayout.EndVertical();

            // space
            GUILayout.Space(20);

            // move prefabs to resources
            if (GUILayout.Button(new GUIContent("Move Prefabs to Resources", "Makes sure that all prefabs are within the resources folder, to ensure they are loaded in build")))
            {
                item.MovePrefabsToResourceFolder();
            }

            //end green box
            GUILayout.EndVertical();

        
            // purple background
            GUI.backgroundColor = new Color(0.5f, 0, 0.5f);
            // achievements box
            GUILayout.BeginVertical("box");
            // reset background color
            GUI.backgroundColor = Color.white;

            // bold text
            GUILayout.Label("Achievements", CustomEditorStuff.center_bold_label);

            // achievements list
            EditorGUI.indentLevel++;
            // enum for unlock achievement
            item.m_unlockAchievement = (AchievementsManager.Achievement)EditorGUILayout.EnumPopup(new GUIContent("Unlock Achievement: ", "The achievement to unlock when this item is used"), item.m_unlockAchievement);
            
            // end achievements box
            GUILayout.EndVertical();


            // on change save
            if (GUI.changed)
            {
                // if not in play mode, save (set dirty)
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(item);
                }
            }
        }
        
        /// <summary>
        /// Updates the file name to match the item id and instance id
        /// </summary>
        /// <param name="item"></param>
        private static void SetFileName(Item item)
        {
            item.ValidateID();
            string itemID = item.id;

            //update file name
            string path = AssetDatabase.GetAssetPath(item);
            AssetDatabase.RenameAsset(path, item.id + (item.m_instanceID != "" ? "_" + item.m_instanceID : ""));

            //update item id
            item.id = itemID;

            // set dirty
            EditorUtility.SetDirty(item);
        }

        private static void LoadDataFromFile(Item item, string _fullPath)
        {
            //read file
            FileStream file = File.Open(_fullPath, FileMode.Open);
            StreamReader reader = new StreamReader(file);
            string itemType = reader.ReadLine();
            string json = reader.ReadToEnd();
            reader.Close();
            file.Close();

            Debug.Log("Loading " + itemType);

            // load data into item
            JsonUtility.FromJsonOverwrite(json, item);
        }
    }
    #endif
}