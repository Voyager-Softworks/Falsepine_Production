using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// The primary way to travel and load levels.
/// </summary>
public class LevelController : MonoBehaviour
{
    static public void LoadMenu()
    {
        // save all inventories
        if (InventoryManager.instance != null) InventoryManager.instance.SaveInventories();

        // save missions
        if (MissionManager.instance != null) MissionManager.instance.SaveMissions();

        SceneManager.LoadScene(0);
    }

    static public void LoadTown(){
        // save all inventories
        if (InventoryManager.instance != null) InventoryManager.instance.SaveInventories();

        // save missions
        if (MissionManager.instance != null) MissionManager.instance.SaveMissions();

        SceneManager.LoadScene(1);
    }

    static public void LoadSnow()
    {
        // save all inventories
        if (InventoryManager.instance != null) InventoryManager.instance.SaveInventories();

        // save missions
        if (MissionManager.instance != null) MissionManager.instance.SaveMissions();

        SceneManager.LoadScene(2);
    }

    static public void LoadSnowBoss()
    {
        // save all inventories
        if (InventoryManager.instance != null) InventoryManager.instance.SaveInventories();

        // save missions
        if (MissionManager.instance != null) MissionManager.instance.SaveMissions();

        SceneManager.LoadScene(3);
    }

    static public void LoadComplete()
    {
        // save all inventories
        if (InventoryManager.instance != null) InventoryManager.instance.SaveInventories();

        // save missions
        if (MissionManager.instance != null) MissionManager.instance.SaveMissions();

        SceneManager.LoadScene("Scene_Complete");
    }

    static public void LoadGameOver()
    {
        // delete misison save file
        if (MissionManager.instance != null)
        {
            MissionManager.instance.DeleteMissionSave();

            // destroy mission manager
            Destroy(MissionManager.instance.gameObject);
        }

        // delete inventory save file for player, home, and shop
        if (InventoryManager.instance != null)
        {
            foreach (Inventory inv in InventoryManager.instance.inventories)
            {
                if (inv.id == "player" || inv.id == "home" || inv.id == "shop")
                {
                    inv.ClearInventory();
                    inv.DeleteSaveFile();
                }
            }

            // destroy inventory manager
            Destroy(InventoryManager.instance.gameObject);
        }

        // destroy journal

        SceneManager.LoadScene("Scene_GameOver");
    }

    static public void Quit(){
        Application.Quit();
    }

    static public void LoadScene(int _index){
        SceneManager.LoadScene(_index);
    }
}
