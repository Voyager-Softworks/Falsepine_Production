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

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Item : ScriptableObject
{
    // List of all the item classes in the game
    [NonSerialized] public static readonly IEnumerable<System.Type> AllTypes;

    /// <summary>
    /// Gets the folder to save item instances to.
    /// </summary>
    public static string GetInstanceSavePath(int saveSlot = 0){
        return Application.dataPath + "/saves/save" + saveSlot + "/instances/";
    }

    /// <summary>
    /// Gets the potential file name for this item instance.
    /// </summary>
    public string GetInstanceFileName(){
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


    [Serializable]
    public class FieldResourceLink{
        [SerializeField] public string fieldName = "";
        [SerializeField] public string resourceName = "";
        [SerializeField] public Type resourceType = typeof(Item);
    }
    [SerializeField] public List<FieldResourceLink> m_resourceLinks = new List<FieldResourceLink>();

    //[SerializeField] public List<String> m_resources = new List<String>();

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
    public void DestroyInstance()
    {
        Debug.Log("Destroying instance of item: " + this.id);

        if (string.IsNullOrEmpty(instanceID))
        {
            return;
        }

        string path = GetInstanceSavePath() + GetInstanceFileName();
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

        // check invalid type
        if (this.GetType() != _item.GetType() || this.id != _item.id){
            Debug.LogWarning("Trying to add an item of a different type or id to a stack");
            return _item;
        }

        // compare strings
        if (mustMatchToStack){
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
            if (thisJson != otherJson){
                Debug.LogWarning("Trying to add an item with different data to a stack");
                return _item;
            }
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

    /// <summary>
    /// Validates the id of the item.
    /// </summary>
    public void ValidateID(){
        //correct id
        //to lower
        m_id = m_id.ToLower();
        //replace non letters and numbers with ""
        m_id = Regex.Replace(m_id, @"[^a-zA-Z0-9_]", "");
        //replace space with _
        m_id = m_id.Replace(" ", "_");
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

        item.currentStackSize = this.m_currentStackSize;
        item.maxStackSize = this.m_maxStackSize;
        item.mustMatchToStack = this.mustMatchToStack;

        if (item.name == "")
        {
            item.name = item.id;
        }

        return item;
    }

    public void PrefabsToResourceList(){
        m_resourceLinks.Clear();
        //m_resources.Clear();

        // get all GameObject varibles from item
        List<FieldInfo> fields = new List<FieldInfo>(this.GetType().GetFields());
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(GameObject))
            {
                GameObject prefab = (GameObject)field.GetValue(this);
                if (prefab != null)
                {
                    //m_resources.Add(prefab.name);

                    // add resource link
                    FieldResourceLink link = new FieldResourceLink();
                    link.fieldName = field.Name;
                    link.resourceName = prefab.name;
                    link.resourceType = typeof(GameObject);
                    m_resourceLinks.Add(link);
                }
            }
        }

        // get all sprite varibles from item
        fields = new List<FieldInfo>(this.GetType().GetFields());
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(Sprite))
            {
                Sprite sprite = (Sprite)field.GetValue(this);
                if (sprite != null)
                {
                    //m_resources.Add(sprite.name);

                    // add resource link
                    FieldResourceLink link = new FieldResourceLink();
                    link.fieldName = field.Name;
                    link.resourceName = sprite.name;
                    link.resourceType = typeof(Sprite);
                    m_resourceLinks.Add(link);
                }
            }
        }

    }

    public void ResourceListToPrefabs(){
        foreach (FieldResourceLink link in m_resourceLinks)
        {
            FieldInfo field = this.GetType().GetField(link.fieldName);

            UnityEngine.Object value = Resources.Load(link.resourceName, field.FieldType);

            if (value != null)
            {   
                if (field != null)
                {
                    field.SetValue(this, value);
                }
            }
        }
    }

    public void MovePrefabsToResourceFolder(){
        #if UNITY_EDITOR
        // get all GameObject varibles from item    
        List<FieldInfo> fields = new List<FieldInfo>(this.GetType().GetFields());
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(GameObject))
            {
                GameObject prefab = (GameObject)field.GetValue(this);
                if (prefab != null)
                {
                    // move prefab to resources folder
                    UnityEditor.AssetDatabase.MoveAsset(UnityEditor.AssetDatabase.GetAssetPath(prefab), "Assets/Resources/" + prefab.name + ".prefab");
                }
            }
        }

        // get all sprite varibles from item
        fields = new List<FieldInfo>(this.GetType().GetFields());
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(Sprite))
            {
                Sprite sprite = (Sprite)field.GetValue(this);
                if (sprite != null)
                {
                    // move sprite to resources folder
                    UnityEditor.AssetDatabase.MoveAsset(UnityEditor.AssetDatabase.GetAssetPath(sprite), "Assets/Resources/" + sprite.name + ".png");
                }
            }
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

        _item.PrefabsToResourceList();

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

        //deserialize item as type
        ScriptableObject item = ScriptableObject.CreateInstance(itemType);

        // cast to item type
        JsonUtility.FromJsonOverwrite(json, item);

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

            // Draw default inspector option
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            item.drawDefaultInspector = GUILayout.Toggle(item.drawDefaultInspector, "Default Inspector");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (item.drawDefaultInspector){
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
                item.ValidateID();
                string itemID = item.id;

                //update file name
                string path = AssetDatabase.GetAssetPath(item);
                string newPath = path.Replace(item.id, item.id);
                AssetDatabase.RenameAsset(path, item.id);

                //update item id
                item.id = itemID;

                // set dirty
                EditorUtility.SetDirty(item);
            }
            GUILayout.EndHorizontal();
            // disabled varint id
            EditorGUI.BeginDisabledGroup(true);
            item.instanceID = EditorGUILayout.TextField("Instance ID: ", item.instanceID);
            EditorGUI.EndDisabledGroup();
            
            item.m_displayName = EditorGUILayout.TextField("Display Name: ", item.m_displayName);
            item.m_description = EditorGUILayout.TextField("Description: ", item.m_description);
            item.m_icon = EditorGUILayout.ObjectField("Icon: ", item.m_icon, typeof(Sprite), false) as Sprite;

            // stack info box
            GUILayout.BeginVertical("box");
            // bold text
            GUILayout.Label("Stack Info", CustomEditorStuff.center_bold_label);

            // horiz
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Current Stack: ","The current amount of items in this stack"));
            item.currentStackSize = EditorGUILayout.IntField(item.currentStackSize);

            GUILayout.Label(new GUIContent("Max Stack: ","The maximum amount of items in this stack"));
            item.maxStackSize = EditorGUILayout.IntField(item.maxStackSize);
            GUILayout.EndHorizontal();
            
            item.mustMatchToStack = EditorGUILayout.Toggle(new GUIContent("Must Match To Stack: ", "Items of the same type must match in all other spects to stack"), item.mustMatchToStack);

            // end stack info box
            GUILayout.EndVertical();

            //tags list
            EditorGUI.indentLevel++;
            showTags = EditorGUILayout.Foldout(showTags, "Tags", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold});
            EditorGUI.indentLevel--;
            if (showTags){
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
                            EditorUtility.SetDirty(this);
                        }
                        break;
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Tag"))
                {
                    item.m_tags.Add(new TagManager.Tag("New Tag"));

                    int c =ItemDatabase.database.Count();

                    // save
                    // if not in play mode, save (set dirty)
                    if (!Application.isPlaying)
                    {
                        EditorUtility.SetDirty(this);
                    }
                }
                GUILayout.EndVertical();
            }

            // move prefabs to resources
            if (GUILayout.Button("Move Prefabs to Resources"))
            {
                item.MovePrefabsToResourceFolder();
            }

            //end green box
            GUILayout.EndVertical();


            // on change save
            if (GUI.changed)
            {
                // if not in play mode, save (set dirty)
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(this);
                }
            }
        }
    }
    #endif
}