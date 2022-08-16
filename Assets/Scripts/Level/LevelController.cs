using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// The primary way to travel and load levels.
/// </summary>
public class LevelController : MonoBehaviour  /// @todo comment
{
    static public void LoadMenu(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("Menu");
    }

    static public void LoadTown(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("TownScene");
    }

    static public void LoadSnow(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("SnowLevel");
    }

    static public void LoadSnowBoss(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("BoneStag");
    }

    static public void LoadScene(string sceneName, bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene(sceneName);
    }

    static public void LoadComplete(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("Scene_Complete");
    }

    static public void LoadGameOver()
    {
        SaveManager.SoftDeleteAll(SaveManager.currentSaveSlot);

        // destroy mission manager
        if (MissionManager.instance) Destroy(MissionManager.instance.gameObject);

        // destroy inventory manager
        if (InventoryManager.instance) Destroy(InventoryManager.instance.gameObject);

        // destroy journal
        if (JournalManager.instance) Destroy(JournalManager.instance.gameObject);

        SceneManager.LoadScene("Scene_GameOver");
    }

    static public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    static public void Quit(/* bool _doSave = true */)
    {
        // if (_doSave)
        // {
        //     SaveManager.SaveAll(SaveManager.currentSaveSlot);
        // }

        Application.Quit();
    }

    // static public void LoadScene(int _index){
    //     SceneManager.LoadScene(_index);
    // }
}
