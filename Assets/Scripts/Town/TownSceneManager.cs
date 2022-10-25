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

        // if any missions are not complete or failed, reset them
        foreach (Mission m in MissionManager.instance.GetMissions())
        {
            if (m.GetState() != MissionCondition.ConditionState.COMPLETE && m.GetState() != MissionCondition.ConditionState.FAILED)
            {
                m.Reset();
            }
        }

        // if this is the final zone, embark
        if (MissionManager.instance && MissionManager.instance.GetZoneIndex(MissionManager.instance.GetCurrentZone()) >= MissionManager.instance.m_missionZones.Count - 1)
        {
            MissionManager.instance.TryEmbark();
        }

        // heal
        if (StatsManager.instance){
            StatsManager.instance.m_playerCurrentHealth = StatsManager.instance.m_calcedPlayerMaxHealth;
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
