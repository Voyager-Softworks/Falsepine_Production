using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

public class PanelInn : MonoBehaviour
{
    public List<GameObject> missionCardUIList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable() {
        //update the town UI
        MissionManager missionManager = FindObjectOfType<MissionManager>();

        if (missionManager == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Update the UI with the current missions
    /// </summary>
    public void UpdateUI(){
        MissionManager missionManager = FindObjectOfType<MissionManager>();

        if (missionManager == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        if (missionCardUIList.Count <= 0) return;

        missionManager.UpdateAllMissionCards();
    }
}
