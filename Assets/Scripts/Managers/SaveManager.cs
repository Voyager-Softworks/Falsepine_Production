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
    }

    public static void LoadAll(int saveSlot)
    {
        // set current save slot
        SetSaveSlot(saveSlot);

        // destroy mission manager
        if (MissionManager.instance) Destroy(MissionManager.instance.gameObject);

        // destroy inventory manager
        if (InventoryManager.instance) Destroy(InventoryManager.instance.gameObject);

        // destroy journal
        if (JournalManager.instance) Destroy(JournalManager.instance.gameObject);

        LevelController.LoadTown(false);
    }

    /// <summary>
    /// Deletes all inventories and missions, but keeps the journal.
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

            // destroy mission manager
            Destroy(MissionManager.instance.gameObject);
        }

        // destroy journal
        Destroy(JournalManager.instance.gameObject);
    }

    public static void HardDeleteAll(int saveSlot){
        // delete journal save file
        if (JournalManager.instance != null)
        {
            JournalManager.instance.DeleteJournalSave(saveSlot);

            // destroy journal
            Destroy(JournalManager.instance.gameObject);
        }
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
    }
    #endif
}
