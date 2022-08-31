using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Manages the saving and loading of multiple systems. <br/>
/// It also manages the current save slot. <br/>
/// @todo Add a way to save and load the current save slot, as well as a way to get a list of current save slots.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static int currentSaveSlot = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static string GetRootSaveFolder()
    {
        return Application.dataPath + "/saves";
    }

    /// <summary>
    /// Gets the folder for the current save. ".../saves[saveSlot]"
    /// </summary>
    /// <param name="saveSlot">the index of the save slot you want</param>
    /// <returns></returns>
    public static string GetSaveFolderPath(int saveSlot)
    {
        return GetRootSaveFolder() + "/save" + saveSlot;
    }

    /// <summary>
    /// Sets the current save slot index
    /// </summary>
    /// <param name="_saveSlot"></param>
    public static void SetSaveSlot(int _saveSlot)
    {
        currentSaveSlot = _saveSlot;
    }

    /// <summary>
    /// Saves Inventories, Missions, and Journal to the save folder.
    /// </summary>
    public static void SaveAll(int saveSlot)
    {
        // save all inventories
        if (InventoryManager.instance != null) InventoryManager.instance.SaveInventories(saveSlot);

        // save missions
        if (MissionManager.instance != null) MissionManager.instance.SaveMissions(saveSlot);

        // save journal
        if (JournalManager.instance != null) JournalManager.instance.SaveJournal(saveSlot);

        // save economy
        if (EconomyManager.instance != null) EconomyManager.instance.SaveEconomy(saveSlot);

        // save stats
        if (StatsManager.instance != null) StatsManager.instance.SaveStats(saveSlot);
    }

    public static void LoadAll(int saveSlot)
    {
        // set current save slot
        SetSaveSlot(saveSlot);

        LevelController.DestroyManagers();

        LevelController.LoadTown(false);
    }

    /// <summary>
    /// Deletes all inventories and missions, but keeps the journal.
    /// @todo make sure that some parts of the economy and stats are kept!
    /// Also, perhaps instead of deleting, we could keep the save file in a different folder "old saves"
    /// </summary>
    public static void SoftDeleteAll(int saveSlot)
    {
        // delete inventory save file for player, home, and store
        if (InventoryManager.instance != null)
        {
            foreach (Inventory inv in InventoryManager.instance.inventories)
            {
                if (inv.id == "player" || inv.id == "home" || inv.id == "store")
                {
                    inv.ClearInventory();
                    inv.DeleteSaveFile(saveSlot);
                }
            }
        }

        // delete misison save file
        if (MissionManager.instance != null)
        {
            MissionManager.instance.DeleteMissionSave(saveSlot);
        }

        // delete the mission board save file
        TownBuilding_MissionBoard.DeleteMissionBoardSave(saveSlot);

        // delete the economy save file
        if (EconomyManager.instance != null)
        {
            EconomyManager.instance.DeleteEconomySave(saveSlot);
        }

        // delete stats save file
        if (StatsManager.instance != null)
        {
            StatsManager.instance.DeleteStatsSave(saveSlot);
        }
    }

    /// <summary>
    /// Deletes literally all save data at index.
    /// </summary>
    /// <param name="saveSlot"></param>
    public static void HardDeleteAll(int saveSlot){
        // delete all inventories save files
        if (InventoryManager.instance != null) InventoryManager.instance.DeleteInventories(saveSlot);

        // delete missions save file
        if (MissionManager.instance != null) MissionManager.instance.DeleteMissionSave(saveSlot);

        // delete mission board save file
        TownBuilding_MissionBoard.DeleteMissionBoardSave(saveSlot);

        // delete journal save file
        if (JournalManager.instance != null) JournalManager.instance.DeleteJournalSave(saveSlot);

        // delete economy save file
        if (EconomyManager.instance != null) EconomyManager.instance.DeleteEconomySave(saveSlot);

        // delete stats save file
        if (StatsManager.instance != null) StatsManager.instance.DeleteStatsSave(saveSlot);
    }

    public static void DeleteAllSaves(){
        //get save folder
        string saveFolderPath = GetRootSaveFolder();

        //delete files and folders in save folder
        if (Directory.Exists(saveFolderPath))
        {
            Directory.Delete(saveFolderPath, true);
        }

        //create new save folder
        Directory.CreateDirectory(saveFolderPath);
    }

    #if UNITY_EDITOR
    // custom Editor button to delete all save files
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : Editor
    {
        // Menu item to delete saves
        [MenuItem("Save System/Delete All Saves")]
        static void DellSaves()
        {
            SaveManager.DeleteAllSaves();
        }

        // Open save folder
        [MenuItem("Save System/Open Save Folder")]
        static void OpenSaveFolder()
        {
            string saveFolderPath = GetRootSaveFolder();
            if (Directory.Exists(saveFolderPath))
            {
                System.Diagnostics.Process.Start(saveFolderPath);
            }
        }
    }
    #endif
}
