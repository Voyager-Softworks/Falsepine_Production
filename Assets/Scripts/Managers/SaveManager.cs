using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveManager : MonoBehaviour  /// @todo Comment
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string GetRootSaveFolder()
    {
        return Application.dataPath + "/saves";
    }

    /// <summary>
    /// Gets the folder for the current save. ".../saves[saveSlot]"
    /// </summary>
    /// <param name="saveSlot">the index of the save slot you want</param>
    /// <returns></returns>
    public static string GetSaveFolderPath(int saveSlot = 0)
    {
        return GetRootSaveFolder() + "/save" + saveSlot;
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
    }

    #endif
}
