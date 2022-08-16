using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test class to complete a mission
/// </summary>
public class DebugMissionCompleter : MonoBehaviour
{
    public bool CompleteOnTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Completes current mission
    /// </summary>
    public void TryCompleteMission()
    {
        if (MissionManager.instance == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        if (MissionManager.instance != null && MissionManager.instance.GetCurrentMission() != null)
        {
            MissionManager.instance.GetCurrentMission().SetCompleted(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompleteOnTrigger)
        {
            TryCompleteMission();
        }
    }
}
