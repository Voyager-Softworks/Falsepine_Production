using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuZoneManager : MonoBehaviour
{
    [System.Serializable]
    public class MenuZone
    {
        public MissionZone.ZoneArea zoneArea;
        public GameObject menuZone;
    }

    [Tooltip("First Zone will be the default zone")]
    public List<MenuZone> menuZones;

    // Start is called before the first frame update
    void Start()
    {
        // disable all menu zones
        foreach (MenuZone menuZone in menuZones)
        {
            menuZone.menuZone.SetActive(false);
        }
        
        MissionZone.ZoneArea? recentZone = SaveManager.GetRecentZone(SaveManager.currentSaveSlot);
        bool foundZone = false;
        if (recentZone != null)
        {
            foreach (MenuZone menuZone in menuZones)
            {
                if (menuZone.zoneArea == recentZone)
                {
                    menuZone.menuZone.SetActive(true);
                    foundZone = true;
                    break;
                }
            }
        }

        if (!foundZone)
        {
            menuZones[0].menuZone.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
