using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// The primary way to travel and load levels. Communicates with the SaveManager to save and load the state of the game.
/// </summary>
public class LevelController : MonoBehaviour
{
    static public void LoadMenu(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        DestroyManagers();

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

    /// <summary>
    /// Soft deletes progress and loads the game over scene.
    /// </summary>
    static public void LoadGameOver()
    {
        SaveManager.SoftDeleteAll(SaveManager.currentSaveSlot);

        DestroyManagers();

        SceneManager.LoadScene("Scene_GameOver");
    }

    static public void DestroyManagers(){
        if (MissionManager.instance) Destroy(MissionManager.instance.gameObject);
        if (InventoryManager.instance) Destroy(InventoryManager.instance.gameObject);
        if (JournalManager.instance) Destroy(JournalManager.instance.gameObject);
        if (EconomyManager.instance) Destroy(EconomyManager.instance.gameObject);
        if (StatsManager.instance) Destroy(StatsManager.instance.gameObject);
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
