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
    /// <param name="_saveSlot">the index of the save slot you want</param>
    /// <returns></returns>
    public static string GetSaveFolderPath(int _saveSlot)
    {
        return GetRootSaveFolder() + "/save" + _saveSlot;
    }

    /// <summary>
    /// Gets the death save folder for the current save. ".../saves[saveSlot]/deaths"
    /// </summary>
    /// <param name="_saveSlot"></param>
    /// <returns></returns>
    public static string GetDeathSaveFolderPath(int _saveSlot)
    {
        return GetSaveFolderPath(_saveSlot) + "/deaths";
    }

    public static string GetRecentDeathFolderPath(int _saveSlot)
    {
        // Get the death folder
        string deathFolder = GetDeathSaveFolderPath(_saveSlot);
        if (!Directory.Exists(deathFolder))
        {
            // If the death folder doesn't exist, return null
            return null;
        }

        // find the folder that contains the string "_RECENT"
        string[] folders = Directory.GetDirectories(deathFolder);
        foreach (string folder in folders)
        {
            if (folder.Contains("_RECENT"))
            {
                return folder;
            }
        }

        return null;
    }

    public static MissionZone.ZoneArea? GetRecentZone(int _saveSlot)
    {
        // get the save folder
        string saveFolder = GetSaveFolderPath(_saveSlot);
        if (!Directory.Exists(saveFolder))
        {
            // if the save folder doesn't exist, return null
            return null;
        }

        // get the "recentZone.json" file
        string recentZoneFile = saveFolder + "/recentZone.json";

        // if the file doesn't exist, return null
        if (!File.Exists(recentZoneFile))
        {
            return null;
        }

        // read the file
        string json = File.ReadAllText(recentZoneFile);

        // parse the json
        MissionZone.ZoneArea zone = JsonUtility.FromJson<MissionZone.ZoneArea>(json);

        return zone;
    }

    public static void SaveRecentZone(int _saveSlot, MissionZone.ZoneArea? _zone)
    {
        // if the save folder doesn't exist, create it
        string saveFolder = GetSaveFolderPath(_saveSlot);
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string recentZoneFile = saveFolder + "/recentZone.json";

        // if null, delete the file
        if (_zone == null)
        {
            if (File.Exists(recentZoneFile))
            {
                //File.Delete(recentZoneFile);
            }
            return;
        }

        FileStream file = File.Create(recentZoneFile);

        string json = JsonUtility.ToJson(_zone);
        
        StreamWriter writer = new StreamWriter(file);
        writer.Write(json);
        writer.Close();

        file.Close();
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
        if (MissionManager.instance != null)
        {
            MissionManager.instance.SaveMissions(saveSlot);
            SaveRecentZone(saveSlot, MissionManager.instance.GetCurrentZone()?.m_area);
        }

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
    /// Also, perhaps instead of deleting, we could keep the save file in a different folder
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
    /// This function is called when the game is over (player dies). <br/>
    /// It saves the current game to a death save file, and then deletes the current save file. <br/>
    /// It also deletes all inventories and missions, but keeps the journal, parts of the economy, and parts of the stats.
    /// </summary>
    public static void GameOverRestart(int _saveSlot){
        // save, then move the save files to the death save folder
        SaveAll(_saveSlot);

        // if death save folder doesn't exist, create it
        if (!Directory.Exists(GetDeathSaveFolderPath(_saveSlot)))
        {
            Directory.CreateDirectory(GetDeathSaveFolderPath(_saveSlot));
        }

        // remove the string "_RECENT" from all death folders in GetDeathSaveFolderPath(_saveSlot)
        string[] deathSaveFolders = Directory.GetDirectories(GetDeathSaveFolderPath(_saveSlot));
        foreach (string deathFolder in deathSaveFolders)
        {
            string deathSaveFolderName = Path.GetFileName(deathFolder);
            if (deathSaveFolderName.Contains("_RECENT"))
            {
                string newDeathSaveFolderName = deathSaveFolderName.Replace("_RECENT", "");
                string newDeathSaveFolderPath = deathFolder.Replace(deathSaveFolderName, newDeathSaveFolderName);
                Directory.Move(deathFolder, newDeathSaveFolderPath);
            }
        }

        string deathSaveFolder = GetDeathSaveFolderPath(_saveSlot) + "/death_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_RECENT";
        
        // create the death save folder if it doesn't exist, if it does exist, delete it
        if (!Directory.Exists(deathSaveFolder))
        {
            Directory.CreateDirectory(deathSaveFolder);
        }
        else
        {
            Directory.Delete(deathSaveFolder, true);
            Directory.CreateDirectory(deathSaveFolder);
        }

        // get all folders in the save folder
        string[] folders = Directory.GetDirectories(GetSaveFolderPath(_saveSlot));

        // COPY all folders to the death save folder (except for "deaths", "journal",)
        foreach (string folder in folders)
        {
            string folderName = Path.GetFileName(folder);
            if (folderName == "deaths") continue;

            string newFolder = deathSaveFolder + "/" + folderName;

            // copy the folders (including subfolders)
            Copy(folder, newFolder);
        }

        SoftDeleteAll(_saveSlot);

        // load some manages from death
        EconomyManager.instance.RestoreFromLastDeath(_saveSlot);
        StatsManager.instance.RestoreFromLastDeath(_saveSlot);

        // Reset turorial
        JournalManager.instance.m_showTutorial = true;
        JournalManager.instance.SaveJournal(_saveSlot);

        #if UNITY_EDITOR
        // remove all .meta files in the root save folder (recursively)
        string[] metaFiles = Directory.GetFiles(GetRootSaveFolder(), "*.meta", SearchOption.AllDirectories);
        foreach (string metaFile in metaFiles)
        {
            File.Delete(metaFile);
        }
        #endif
    }

    public static void CompleteSave(int _saveSlot){
        // end the current game
        GameOverRestart(_saveSlot);

        // add "_complete" to the save folder name
        string currentPath = GetSaveFolderPath(_saveSlot);
        string newPath = currentPath + "_complete";

        //make new folder if it doesn't exist
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        else {
            Directory.Delete(newPath, true);
            Directory.CreateDirectory(newPath);
        }

        // copy all folders the current save folder to the new folder
        string[] folders = Directory.GetDirectories(currentPath);
        foreach (string folder in folders)
        {
            string folderName = Path.GetFileName(folder);
            string newFolder = newPath + "/" + folderName;

            // copy the folders (including subfolders)
            //@todo use a build friendly way to copy folders
            Copy(folder, newFolder);
        }

        // delete the current save folder
        Directory.Delete(currentPath, true);
    }

    /// <summary>
    /// Copies all folders and files (including subfolders) from the sourceDirectory to the targetDirectory.
    /// </summary>
    /// <param name="sourceDirectory"></param>
    /// <param name="targetDirectory"></param>
    public static void Copy(string sourceDirectory, string targetDirectory)
    {
        DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
        DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

        CopyAll(diSource, diTarget);
    }

    /// <summary>
    /// Copies all contents of source to target, intended to be used with the Copy function
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        Directory.CreateDirectory(target.FullName);

        // Copy each file into the new directory.
        foreach (FileInfo fi in source.GetFiles())
        {
            //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
            string dest = Path.Combine(target.FullName, fi.Name);
            fi.CopyTo(dest, true);
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir =
                target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
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

        // delete all player prefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

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
        static void DeleteAllSaves()
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
