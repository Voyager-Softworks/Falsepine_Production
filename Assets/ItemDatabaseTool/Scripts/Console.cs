using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Console : MonoBehaviour
{
    public GameObject view;
    public GameObject input;

    public TMP_InputField consoleInput;
    public TextMeshProUGUI consoleLog;

    private List<string> commandHistory = new List<string>();
    private int upIndex = 0;

    // Update is called once per frame
    void Update()
    {
        // if console input is not selected
        if (!consoleInput.isFocused){
           
        }

        // if ` is pressed, toggle console
        if (Keyboard.current.backquoteKey.wasPressedThisFrame){
            ToggleConsole();
        }

        // if enter is pressed, try send command
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (!consoleInput) return;
            string text = consoleInput.text;
            if (text == "") return;

            TrySendCommand(text);

            consoleInput.text = "";

            StartTyping();
        }

        // if up arrow is pressed, go to previous command
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            upIndex--;
            if (upIndex < 0) upIndex = 0;
            if (commandHistory.Count > 0)
            {
                consoleInput.text = commandHistory[upIndex];
            }

            StartTyping();
        }

        // if down arrow is pressed, go to next command
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            upIndex++;
            if (upIndex > commandHistory.Count - 1) upIndex = commandHistory.Count - 1;
            if (commandHistory.Count > 0)
            {
                consoleInput.text = commandHistory[upIndex];
            }

            StartTyping();
        }
    }

    private void ToggleConsole()
    {
        if (view.activeSelf)
        {
            view.SetActive(false);
            input.SetActive(false);

            consoleInput.text = "";
        }
        else
        {
            view.SetActive(true);
            input.SetActive(true);

            consoleInput.text = "";

            StartTyping();
        }
    }

    /// <summary>
    /// Forces user to start typing
    /// </summary>
    public void StartTyping()
    {
        // select the box, start typing, and move cursor to end
        consoleInput.Select();
        consoleInput.ActivateInputField();
        consoleInput.caretPosition = consoleInput.text.Length;
    }

    /// <summary>
    /// Trys to execute a command
    /// </summary>
    public void TrySendCommand(string _command){
        // to lower
        _command = _command.ToLower();
        Log(_command);
        commandHistory.Add(_command);
        upIndex = commandHistory.Count;

        string[] split = _command.Split(' ');

        // "help"
        if (split[0] == "help")
        {
            Log("- Commands:");
            foreach (string command in commandList)
            {
                Log("- - " + command);
            }
            
            Log();
            return;
        }

                // "list_inventories"
        if (split.Length >= 1 && split[0] == "list_inventories")
        {
            Log("- Available inventories:");
            foreach (Inventory inventory in Inventory.allInventories)
            {
                Log("- - " + inventory.id);
            }

            Log();
            return;
        }

        // "list_inventory inventoryID"
        if (split.Length == 2 && split[0] == "list_inventory")
        {
            string inventoryID = split[1];

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");

                Log();
                return;
            }

            Log("- Inventory " + inventoryID + " contains:");

            // list the inventory
            for (int i = 0; i < inventory.slots.Count; i++)
            {
                Item item = inventory.slots[i].item;
                if (item != null)
                {
                    Log("- - " + item.name + " x" + item.currentStackSize);
                }
                else{
                    Log("- - Empty, Filters[");
                    if (inventory.slots[i].typeFilter.Count <= 0){
                        Log("None", false);
                    }
                    else{
                        // list filters
                        for (int j = 0; j < inventory.slots[i].typeFilter.Count; j++)
                        {
                            Log(inventory.slots[i].typeFilter[j] + (j == inventory.slots[i].typeFilter.Count - 1 ? "" : ", "), false);
                        }
                    }
                    Log("]", false);
                }
            }

            Log();
            return;
        }

        // "give inventoryID itemID amount?"
        if (split.Length >= 3 && split[0] == "give")
        {
            string inventoryID = split[1];
            string itemID = split[2];
            int amount = split.Length >= 4 ? int.Parse(split[3]) : 1;

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");
                return;
            }

            // give the item to the player
            if (inventory.TryAddItemToInventory(itemID, amount)){
                Log("- " + inventoryID + " has received " + itemID);
            }
            else{
                Log("- Could not give " + inventoryID + " " + itemID);
            }

            Log();
            return;
        }

        // "clear inventoryID"
        if (split.Length >= 2 && split[0] == "clear")
        {
            string inventoryID = split[1];

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");
                return;
            }

            // clear the inventory
            inventory.ClearInventory();
            Log("- Inventory cleared");

            Log();
            return;
        }

        // "save inventoryID"
        if (split.Length >= 2 && split[0] == "save")
        {
            string save = split[0];
            string inventoryID = split[1];

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");

                Log();
                return;
            }

            // save the inventory
            inventory.SaveInventory();
            Log("- Inventory saved");

            Log();
            return;
        }

        // "load inventoryID"
        if (split.Length >= 2 && split[0] == "load")
        {
            string load = split[0];
            string inventoryID = split[1];

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");

                Log();
                return;
            }

            // load the inventory
            inventory.LoadInventory();
            Log("- Inventory loaded");

            Log();
            return;
        }

        // "list_database"
        if (split.Length == 1 && split[0] == "list_database")
        {
            Log("- Database contains:");

            // list the database
            for (int i = 0; i < ItemDatabase.database.Count; i++)
            {
                Item item = ItemDatabase.database[i];
                Log("- - " + item.name);
            }

            Log();
            return;
        }

        // "quit"
        if (split.Length == 1 && split[0] == "quit")
        {
            Log("- Quitting");

            Application.Quit();
            return;
        }

        // "restart"
        if (split.Length == 1 && split[0] == "restart")
        {
            Log("- Restarting");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        // "scene sceneNumber"
        if (split.Length == 2 && split[0] == "scene")
        {
            int sceneNumber = int.Parse(split[1]);
            SceneManager.LoadScene(sceneNumber);

            Log("- Scene " + sceneNumber + " loaded");

            return;
        }

        // "delete_mission_save"
        if (split.Length == 1 && split[0] == "delete_mission_save")
        {
            MissionManager.instance?.DeleteMissionSave();

            Log("- Mission save deleted");

            return;
        }

        // "complete_mission"
        if (split.Length == 1 && split[0] == "complete_mission")
        {
            MissionManager.instance?.currentMission.SetCompleted(true);

            Log("- Mission completed");

            return;
        }

        // "inspect inventoryID slotNumber"
        if (split.Length == 3 && split[0] == "inspect")
        {
            string inventoryID = split[1];
            int slotNumber = int.Parse(split[2]);

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");

                Log();
                return;
            }

            // inspect the item
            Item item = inventory.slots[slotNumber].item;
            if (!item) 
            {
                Log("- Slot " + slotNumber + " is empty");
                return;
            }
            else{
                Log("- Slot " + slotNumber + " contains " + item.name);
                
                // get all fields of item
                FieldInfo[] fields = item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object value = field.GetValue(item);
                    if (value != null)
                    {
                        Log("- - " + field.Name + ": " + value);
                    }

                    //if field is a list, get the count
                    if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Log("- - " + field.Name + ": " + field.GetValue(item).GetType().GetProperty("Count").GetValue(field.GetValue(item)));
                    }
                }

                return;
            }
        }

        // command not found
        Log("- Command not found");
    }

    private List<string> commandList = new List<string>(){
        "help",
        "list_inventories",
        "list_inventory inventoryID",
        "give inventoryID itemID amount?",
        "clear inventoryID",
        "save inventoryID",
        "load inventoryID",
        "list_database",
        "quit",
        "restart",
        "scene sceneNumber",
        "delete_mission_save",
        "complete_mission",
        "inspect inventoryID slotNumber"
    };

    /// <summary>
    /// Logs a message to the console
    /// </summary>
    public void Log(string _text = "", bool _newLine = true)
    {
        if (!consoleLog) return;

        if (_newLine)
        {
            consoleLog.text += "\n" + _text;
        }
        else
        {
            consoleLog.text += _text;
        }
    }
}