using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

public class MissionCardUI : MonoBehaviour
{
    public TextMeshProUGUI missionTitle;

    public TextMeshProUGUI missionDescription;

    public TextMeshProUGUI buttonText;

    public Mission associatedMission;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonClicked(){
        MissionManager missionManager = FindObjectOfType<MissionManager>();

        if (missionManager == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        missionManager.TryStartMission(associatedMission);
    }
}
