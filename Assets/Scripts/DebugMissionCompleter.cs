using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMissionCompleter : MonoBehaviour  /// @todo Comment
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryCompleteMission()
    {
        if (MissionManager.instance == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        if (MissionManager.instance.currentMission) MissionManager.instance.currentMission.isCompleted = true;
    }
}
