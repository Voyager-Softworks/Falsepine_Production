using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Does stuff on town scene load only
/// </summary>
public class TownSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //if (MissionManager.instance) MissionManager.instance.SaveMissions();

        //unlock and unhide cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // set time scale to 1
        Time.timeScale = 1;

        // remove drinks
        StatsManager.ClearDrinkMods();

        // refill all ammo
        Inventory inventory = InventoryManager.instance.GetInventory("player");
        if (inventory)
        {
            inventory.FillAmmo();
        }

        // find and destroy all mission music
        AudioControllerPersistance[] audioController = FindObjectsOfType<AudioControllerPersistance>();
        foreach (AudioControllerPersistance a in audioController)
        {
            Destroy(a.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy() {
        //if (MissionManager.instance) MissionManager.instance.SaveMissions();
    }
}
