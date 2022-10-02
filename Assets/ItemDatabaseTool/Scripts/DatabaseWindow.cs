using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// The main window for the Item Database Tool.
/// </summary>
public class DatabaseWindow : EditorWindow {

    static string itemPath = "Assets/ItemDatabaseTool/Resources/Items/";
    static string itemTypePath = "Assets/ItemDatabaseTool/ItemTypes/";

    static string searchText = "";

    public static Item selected;
    static public int selectedType;

    Vector2 listScroll = Vector2.zero;
    Vector2 itemScroll = Vector2.zero;

    // Open window button
    [MenuItem("ItemDatabaseTool/Database Window")]
    private static void ShowWindow() {
        EditorWindow window = GetWindow<DatabaseWindow>();
        window.titleContent = new GUIContent("Database Window");
        window.Show();
    }

    // Remake the database .json files
    [MenuItem("ItemDatabaseTool/Re-make Database folder")]
    private static void ResetDatabaseFolder() {
        ItemDatabase.Refresh();

        ItemDatabase.ResetDatabaseFolder();
    }

    // Make new item type button
    [MenuItem("ItemDatabaseTool/Make New Item Type")]
    private static void MakeNewItemType() {
        // popup window to get the name of the new item type
        string newItemTypeName = EditorInputDialog.Show("New Item Type Name", "Enter the name of the new item type:", "");

        // create the new item type
        CreateNewItemType(newItemTypeName);
    }

    private void OnValidate() {
        ItemDatabase.Refresh();
    }

    private void OnEnable() {
        ItemDatabase.Refresh();
    }

    private void OnGUI()
    {
        //check selected
        if (!selected)
        {
            selected = ItemDatabase.database.FirstOrDefault();
        }
        

        GUILayout.BeginHorizontal();
        DrawList();
        DrawItemInspector();
        GUILayout.EndHorizontal();

        //Refresh();
        
        //save on change
        if (GUI.changed)
        {
            ItemDatabase.Refresh();
            // if not in play mode, save (set dirty)
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
            }
        }

        Event e = Event.current;
        Rect windowRect = new Rect(0, 0, position.width, position.height);
        if (GUI.Button(windowRect, "", GUIStyle.none))
        {
            GUI.FocusControl(null);
        }
    }

    /// <summary>
    /// Draw the list of items
    /// </summary>
    private void DrawList()
    {
        //draw scroll list of items
        listScroll = GUILayout.BeginScrollView(listScroll, false, true, GUILayout.Width(position.width / 2.5f), GUILayout.Height(position.height));
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Items", CustomEditorStuff.center_bold_label);
        // save to file button
        if (GUILayout.Button(new GUIContent("Re-make Files", "DELETES ALL .json and .meta files in the ItemDatabase folder.\n\nThen re-makes the current database into .json files"), GUILayout.Width(100)))
        {
            ItemDatabase.ResetDatabaseFolder();
        }
        //refresh button
        if (GUILayout.Button(new GUIContent("Refresh", "Refreshes the list below"), GUILayout.MaxWidth(75)))
        {
            ItemDatabase.Refresh();
        }
        GUILayout.EndHorizontal();

        //space
        GUILayout.Space(10);

        // create new item
        GUILayout.BeginHorizontal();
        string[] names = Item.AllTypes.Select(t => t.Name).ToArray();
        names = names.Concat(new string[] { typeof(Item).Name }).ToArray();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button(new GUIContent("Create New", "Create new item of selected type")))
        {
            CreateNewItem(names[selectedType]);
        }

        selectedType = EditorGUILayout.Popup(selectedType,names);
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        // choose path
        GUILayout.BeginHorizontal();
        GUILayout.Label("Path: " + EditorPrefs.GetString("ItemDatabaseTool_Path", itemPath));
        if (GUILayout.Button(new GUIContent("Change", "Change the path to the items folder")))
        {
            string path = EditorUtility.OpenFolderPanel("Choose Items Folder", "", "");

            // check if path is valid
            if (path.Length != 0)
            {
                // path must be in this project
                if (path.Contains(Application.dataPath))
                {
                    itemPath = path.Replace(Application.dataPath, "Assets");

                    // save
                    EditorPrefs.SetString("ItemDatabaseTool_ItemPath", itemPath);
                }
                else
                {
                    Debug.LogError("Path must be within assets folder");
                }
            }
        }
        GUILayout.EndHorizontal();

        //space
        GUILayout.Space(10);

        //search bar
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search: ", GUILayout.MaxWidth(50));
        searchText = GUILayout.TextField(searchText, GUILayout.MinWidth(50));
        if (GUILayout.Button(new GUIContent("Clear", "Clears the search bar"), GUILayout.MaxWidth(75)))
        {
            searchText = "";
        }
        GUILayout.EndHorizontal();
        // space
        GUILayout.Space(10);

        // new list of items
        List<Item> items = ItemDatabase.database;

        List<Item> displayedItems = new List<Item>();

        // if search text is not empty, show ID/Name matches
        if (searchText.Length > 0)
        {
            items = ItemDatabase.GetItemsByORFilter(_id: searchText, _instanceID: searchText, _name: searchText);
            items = items.Except(displayedItems).ToList();
            // add to displayed items
            displayedItems = displayedItems.Union(items).ToList();

            if (items.Count > 0) {
                // space
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Matching ID/Name: " + items.Count, CustomEditorStuff.center_bold_label);
                GUILayout.EndHorizontal();

                foreach (Item _item in items)
                {
                    int listSize = items.Count;

                    DrawItemButton(_item);

                    // if deleting or adding
                    if (listSize != items.Count)
                    {
                        break;
                    }
                }
            }
        }

        // if search text is not empty, show Description matches
        if (searchText.Length > 0)
        {
            items = ItemDatabase.GetItemsByORFilter(_description: searchText);
            items = items.Except(displayedItems).ToList();
            // add to displayed items
            displayedItems = displayedItems.Union(items).ToList();

            if (items.Count > 0) {
                // space
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Matching Description: " + items.Count, CustomEditorStuff.center_bold_label);
                GUILayout.EndHorizontal();

                foreach (Item _item in items)
                {
                    int listSize = items.Count;

                    DrawItemButton(_item);

                    // if deleting or adding
                    if (listSize != items.Count)
                    {
                        break;
                    }
                }
            }
        }

        // if search text is not empty, show Type matches
        if (searchText.Length > 0)
        {
            items = ItemDatabase.GetItemsByORFilter(_type_s: searchText);
            items = items.Except(displayedItems).ToList();
            // add to displayed items
            displayedItems = displayedItems.Union(items).ToList();

            if (items.Count > 0) {
                // space
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Matching Type: " + items.Count, CustomEditorStuff.center_bold_label);
                GUILayout.EndHorizontal();

                foreach (Item _item in items)
                {
                    int listSize = items.Count;

                    DrawItemButton(_item);

                    // if deleting or adding
                    if (listSize != items.Count)
                    {
                        break;
                    }
                }
            }
        }

        items = ItemDatabase.database.Except(displayedItems).ToList();

        // if search text is not empty, show rest
        if (searchText.Length > 0)
        {
            if (items.Count > 0) {
                // space
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("No Match: " + items.Count, CustomEditorStuff.center_bold_label);
                GUILayout.EndHorizontal();
            }
        }

        foreach (Item _item in items)
        {
            int listSize = items.Count;

            DrawItemButton(_item);

            // if deleting or adding
            if (listSize != items.Count)
            {
                break;
            }
        }

        //space
        GUILayout.Space(10);

        GUILayout.EndScrollView();
    }

    private static void DrawItemButton(Item _item)
    {
        //horiz
        GUILayout.BeginHorizontal();
        //begind disabled group
        EditorGUI.BeginDisabledGroup(_item == selected);
        // if instanceID is not empty set color to yellow
        if (_item.instanceID != "")
        {
            GUI.backgroundColor = Color.yellow;
        }
        if (GUILayout.Button(_item.id + (_item.instanceID != "" ? ":" + _item.instanceID : "")))
        {
            selected = _item;
            GUI.FocusControl(null);
        }
        GUI.backgroundColor = Color.white;
        EditorGUI.EndDisabledGroup();

        GUI.backgroundColor = Color.red;
        //delete button
        if (GUILayout.Button("X", GUILayout.Width(20)))
        {
            GUI.backgroundColor = Color.white;
            //popup to confirm
            if (EditorUtility.DisplayDialog("Delete Item", "Are you sure you want to delete " + _item.name + "?\nThis CANNOT be undone!", "DELETE", "KEEP"))
            {
                Debug.Log("Deleting " + _item.name);

                //delete from asset database
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_item));
                //remove from list
                ItemDatabase.database.Remove(_item);
                //refresh
                ItemDatabase.Refresh();
                // break;
            }
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
    }

    private void DrawItemInspector()
    {
        //vert
        GUILayout.BeginVertical();
        itemScroll = GUILayout.BeginScrollView(itemScroll);
        //horiz
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Selected: ");
        //disable group
        EditorGUI.BeginDisabledGroup(true);
        selected = EditorGUILayout.ObjectField(selected, typeof(Item), false) as Item;
        EditorGUI.EndDisabledGroup();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //if selected item is not null, show the selected item
        if (selected != null)
        {
            Editor editor = Editor.CreateEditor(selected);
            editor?.OnInspectorGUI();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Creates a new scriptable object item in unity
    /// </summary>
    static public void CreateNewItem(string type)
    {
        // if path doesnt exist, create it
        if (!Directory.Exists(itemPath))
        {
            Directory.CreateDirectory(itemPath);
        }

        string assetType = type;

        //create the item
        ScriptableObject item = ScriptableObject.CreateInstance(assetType);

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(itemPath + "/" + assetType + ".asset");

        //save
        AssetDatabase.CreateAsset(item, assetPathAndName);
        AssetDatabase.SaveAssets();

        //refresh
        AssetDatabase.Refresh();

        //cast item to item
        Item newItem = item as Item;
        //if item is not null, select it
        if (newItem != null)
        {
            //select it
            selected = newItem;
        }

        ItemDatabase.Refresh();
    }

    /// <summary>
    /// Creates a new Item Type in unity
    /// </summary>
    private static void CreateNewItemType(string newItemTypeName)
    {
        // check if string is valid
        if (string.IsNullOrEmpty(newItemTypeName))
        {
            return;
        }

        // validate the name
        //replace non letters with ""
        newItemTypeName = Regex.Replace(newItemTypeName, @"[^A-Za-z]", "");
        //replace space with ""
        newItemTypeName = newItemTypeName.Replace(" ", "");

        // if path doesnt exist, create it (itemTypePath)
        if (!Directory.Exists(itemTypePath))
        {
            Directory.CreateDirectory(itemTypePath);
        }

        // create new .cs file for the new item type and add it to the project
        string newItemTypeFilePath = itemTypePath + "/" + newItemTypeName + ".cs";
        if (File.Exists(newItemTypeFilePath))
        {
            Debug.LogWarning("Item type with name " + newItemTypeName + " already exists.");
            return;
        }

        // get template file
        string templateFilePath = itemTypePath + "/" + "NewItemTypeTemplate";

        // get template file content as list of lines
        List<string> templateFileContent = new List<string>();

        if (File.Exists(templateFilePath))
        {
            templateFileContent = File.ReadAllLines(templateFilePath).ToList();

            //replace '[ITEM_TYPE]' with newItemTypeName
            templateFileContent = templateFileContent.Select(line => line.Replace("[ITEM_TYPE]", newItemTypeName)).ToList();
        }
        else
        {
            Debug.LogError("Template file " + templateFilePath + " does not exist.");
            return;
        }

        // create the new file and write the content
        File.WriteAllLines(newItemTypeFilePath, templateFileContent);

        // add the new file to the project
        AssetDatabase.ImportAsset(newItemTypeFilePath);

        // open the new file in the editor
        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(newItemTypeFilePath, typeof(MonoScript)));

        // refresh the database
        ItemDatabase.Refresh();

        // reset itemdatabase folder
        ItemDatabase.ResetDatabaseFolder();
    }
}

#endif