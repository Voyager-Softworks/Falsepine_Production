using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Console class lets the user interact with the game's code in a more direct way.
/// </summary>
[Serializable]
public class Console : ToggleableWindow
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
            ToggleWindow();
        }

        // if enter is pressed, try send command
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (!consoleInput) return;
            string text = consoleInput.text;
            if (text == "") return;

            Log("Input: " + text);
            Log();

            //split by "&" to get multiple commands
            string[] commands = text.Split('&');

            // for each command, try to run it
            for (int i = 0; i < commands.Length; i++)
            {
                string command = commands[i];
                if (command == "") continue;
                TrySendCommand(command);
            }

            //TrySendCommand(text);

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

    public override bool IsOpen()
    {
        return view.activeSelf;
    }

    public override void ToggleWindow()
    {
        base.ToggleWindow();
    }

    public override void OpenWindow()
    {
        base.OpenWindow();

        view.SetActive(true);
        input.SetActive(true);

        consoleInput.text = "";

        // pause game
        LevelController.RequestPause(this);

        StartTyping();
    }

    public override void CloseWindow()
    {
        base.CloseWindow();

        view.SetActive(false);
        input.SetActive(false);

        consoleInput.text = "";

        // unpause game
        LevelController.RequestUnpause(this);
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
        commandHistory.Add(_command);
        upIndex = commandHistory.Count;

        //remove any multiple spaces
        while (_command.Contains("  "))
        {
            _command = _command.Replace("  ", " ");
        }

        //remove any leading or trailing spaces
        _command = _command.Trim();

        Log(_command);

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

        // "give inventoryID itemID:instanceID? amount?"
        if (split.Length >= 3 && split[0] == "give")
        {
            string inventoryID = split[1];
            string itemIDWithInstanceID = split[2];
            string itemID = itemIDWithInstanceID.Split(':')[0];
            string instanceID = itemIDWithInstanceID.Split(':').Length > 1 ? itemIDWithInstanceID.Split(':')[1] : "";
            int amount = split.Length >= 4 ? int.Parse(split[3]) : 1;

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");
                return;
            }

            // give the item to the player
            if (inventory.TryAddItemToInventory(itemID, instanceID, amount)){
                Log("- " + inventoryID + " has received " + itemID);
            }
            else{
                Log("- Could not give " + inventoryID + " " + itemID);
            }

            Log();
            return;
        }

        // "remove inventoryID slotNumber"
        if (split.Length == 3 && split[0] == "remove")
        {
            string inventoryID = split[1];
            int slotNumber = 0;
            try { slotNumber = int.Parse(split[2]); }
            catch { Log("- Invalid slot number"); return; }

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");
                return;
            }


            Item removed = inventory.RemoveItemFromInventory(slotNumber);
            // remove the item from the inventory
            if (removed != null){
                Log("- " + inventoryID + " has removed " + removed.m_displayName + " from slot " + slotNumber);
            }
            else{
                Log("- Could not remove item from " + inventoryID + " slot " + slotNumber);
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

        // "fill_ammo inventoryID"
        if (split.Length >= 2 && split[0] == "fill_ammo")
        {
            string fillAmmo = split[0];
            string inventoryID = split[1];

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");

                Log();
                return;
            }
            // fill the ammo
            inventory.FillAmmo();
            Log("- Ammo filled");
            Log();
            return;
        }

        // "fill_stacks inventoryID"
        if (split.Length >= 2 && split[0] == "fill_stacks")
        {
            string fillStack = split[0];
            string inventoryID = split[1];

            Inventory inventory = Inventory.allInventories.Find(x => x.id == inventoryID);
            if (!inventory){
                Log("- Inventory not found");

                Log();
                return;
            }
            // fill the stacks
            inventory.FillStacks();
            Log("- Stack filled");
            Log();
            return;
        }

        // "drink itemID:instanceID?"
        if (split.Length >= 2 && split[0] == "drink")
        {
            string itemIDWithInstanceID = split[1];
            string itemID = itemIDWithInstanceID.Split(':')[0];
            string instanceID = itemIDWithInstanceID.Split(':').Length > 1 ? itemIDWithInstanceID.Split(':')[1] : "";

            // getee the item
            Item item = ItemDatabase.GetItem(itemID, instanceID);
            if (!item){
                Log("- Item not found");
                return;
            }

            // cast to Drink
            Drink drink = item as Drink;
            if (!drink){
                Log("- Item is not a drink");
                return;
            }

            // consume the item
            drink.Consume();
            Log("- Consumed " + item.m_displayName);

            Log();
            return;
        }

        // "clear_drinks"
        if (split.Length >= 1 && split[0] == "clear_drinks")
        {
            // clear all drinks
            StatsManager.ClearDrinkMods();
            Log("- Drinks cleared");

            Log();
            return;
        }

        // "set_saveslot slotNumber"
        if (split.Length == 2 && split[0] == "set_saveslot")
        {
            string set = split[0];
            string slotNumber = split[1];
            int slot = 0;
            try { slot = int.Parse(slotNumber); }
            catch { Log("- Invalid slot number"); return; }

            SaveManager.SetSaveSlot(slot);
            Log("- Save slot set to " + slot);

            Log();
            return;
        }

        // "get_saveslot"
        if (split.Length == 1 && split[0] == "get_saveslot")
        {
            Log("- Save slot is " + SaveManager.currentSaveSlot);
            Log();
            return;
        }

        // "save slotNumber"
        if (split.Length == 2 && split[0] == "save")
        {
            string save = split[0];
            string slotNumber = split[1];
            int slot = 0;
            try { slot = int.Parse(slotNumber); }
            catch { Log("- Invalid slot number"); return; }
            SaveManager.SaveAll(slot);
            Log("- Saved to slot " + slot);
            Log();
            return;
        }

        // "load slotNumber"
        if (split.Length == 2 && split[0] == "load")
        {
            string load = split[0];
            string slotNumber = split[1];
            int slot = 0;
            try { slot = int.Parse(slotNumber); }
            catch { Log("- Invalid slot number"); return; }
            SaveManager.LoadAll(slot);
            Log("- Loaded");
            Log();
            return;
        }

        // "kill_all"
        if (split.Length == 1 && split[0] == "kill_all")
        {
            // get all "enemies" that have a healthscript
            List<EnemyHealth> enemies = GameObject.FindObjectsOfType<EnemyHealth>(/* true */).ToList();

            // kill all of them
            foreach (EnemyHealth enemy in enemies){
                enemy.TakeDamage(new Health_Base.DamageStat(enemy.m_currentHealth, this.gameObject, transform.position, enemy.transform.position, null));
            }
            Log("- All enemies killed");
            Log();
            return;
        }

        // "heal_player"
        if (split.Length == 1 && split[0] == "heal_player")
        {
            PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>(/* true */);
            playerHealth.Heal(playerHealth.calcedMaxHealth);
            Log("- Player healed");
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

        // "list_scenes"
        if (split.Length == 1 && split[0] == "list_scenes")
        {
            Log("- Scenes:");
            // list the scenes
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                // get scene name, trim path and extension
                string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                Log("- - [" + i.ToString() + "] " + sceneName);
            }
            Log();
            return;
        }

        // "current_scene"
        if (split.Length == 1 && split[0] == "current_scene")
        {
            // get scene index in build settings
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            Log("- Current scene: [" + sceneIndex.ToString() + "] " + SceneManager.GetActiveScene().name);
            Log();
            return;
        }

        // "next_scene"
        if (split.Length == 1 && split[0] == "next_scene")
        {
            if (MissionManager.instance != null){
                MissionManager.instance?.LoadNextScene();

                Log("- Next scene loading");
            }
            else{
                Log("- MissionManager not found");
            }
            return;
        }

        // "restart_scene"
        if (split.Length == 1 && split[0] == "restart_scene")
        {
            Log("- Restarting Scene");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        // "scene sceneNumber"
        if (split.Length == 2 && split[0] == "scene")
        {
            string scene = split[0];
            string sceneNumber = split[1];
            int sceneIndex = 0;
            try { sceneIndex = int.Parse(sceneNumber); }
            catch { Log("- Invalid scene number"); return; }

            Log("- Loading scene " + sceneIndex);
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        // "scene sceneName"
        if (split.Length == 2 && split[0] == "scene")
        {
            string sceneName = split[1];
            SceneManager.LoadScene(sceneName);

            Log("- Scene " + sceneName + " loaded");

            return;
        }

        // "delete_mission_save"
        if (split.Length == 1 && split[0] == "delete_mission_save")
        {
            MissionManager.instance?.DeleteMissionSave(SaveManager.currentSaveSlot);

            Log("- Mission save deleted");

            return;
        }

        // "complete_mission"
        if (split.Length == 1 && split[0] == "complete_mission")
        {
            MissionManager.instance?.GetCurrentMission()?.SetState(MissionCondition.ConditionState.COMPLETE);

            Log("- Mission completed");

            return;
        }

        // "message 'message Text' iconName?"
        if (split.Length >= 2 && split[0] == "message")
        {
            // message may be multiple words, so we need to recombine them
            string message = "";
            int lastMessageIndex = split.Length - 2;
            for (int i = 1; i < split.Length; i++)
            {
                // keep adding words until we find a ' or " at the end
                if (split[i].EndsWith("'") || split[i].EndsWith("\""))
                {
                    message += split[i];
                    lastMessageIndex = i;
                    break;
                }
                else
                {
                    // if this is the last split, and it doesn't end with ' or ", then it's the icon
                    if (i == split.Length - 1)
                    {
                        break;
                    }
                    else
                    {
                        message += split[i] + " ";
                    }
                }
            }

            // remove the ' and " from the message
            message = message.Trim('\'');
            message = message.Trim('\"');

            // get the icon
            string icon = "";
            if (split.Length > lastMessageIndex + 1)
            {
                icon = split[lastMessageIndex + 1];
            }

            // get the message manager
            MessageManager messageManager = GameObject.FindObjectOfType<MessageManager>(/* true */);
            if (messageManager == null){
                Log("- MessageManager not found");
                return;
            }

            // show the message
            messageManager.AddMessage(message, icon);

            Log("- Message shown");

            return;
        }

        // command not found
        Log("- Command not found");
    }

    private List<string> commandList = new List<string>(){
        "help",
        "list_database",
        "list_inventories",
        "list_inventory inventoryID",
        "inspect inventoryID slotNumber",
        "give inventoryID itemID:instanceID? amount?",
        "remove inventoryID slotNumber",
        "clear inventoryID",
        "fill_ammo inventoryID",
        "fill_stacks inventoryID",
        "drink itemID:instanceID?",
        "clear_drinks",
        "set_saveslot slotNumber",
        "get_saveslot",
        "save slotNumber",
        "load slotNumber",
        "kill_all",
        "heal_player",
        "quit",
        "list_scenes",
        "current_scene",
        "restart_scene",
        "next_scene",
        "scene sceneNumber",
        "scene sceneName",
        "delete_mission_save",
        "complete_mission",
        "message 'message Text' iconName?",
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