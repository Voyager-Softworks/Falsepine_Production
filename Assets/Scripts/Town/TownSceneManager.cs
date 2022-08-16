using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @deprecated maybe
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy() {
        //if (MissionManager.instance) MissionManager.instance.SaveMissions();
    }
}
