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
    [Header("Self")]
    public GameObject missionCard;

    [Header("Main")]
    public TextMeshProUGUI missionTitle;
    public TextMeshProUGUI missionDescription;
    public Image backgroundImage;
    public Image missionStamp;

    [Header("Button")]
    public GameObject button;
    public TextMeshProUGUI buttonText;

    public Mission associatedMission;

    public MissionManager.MissionRefernceType missionReferenceType;


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

    public void UpdateCard(){
        MissionManager missionManager = FindObjectOfType<MissionManager>();
        if (missionManager == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        Mission desiredMission = missionManager.GetMissionByRefernceType(missionReferenceType);
        associatedMission = desiredMission;

        if (missionReferenceType == MissionManager.MissionRefernceType.CURRENT){
            Mission currentMission = null;
            //if current mission index is within range, then get the mission from the list
            if (MissionManager.instance != null && MissionManager.instance.currentMissionIndex >= 0 && MissionManager.instance.currentMissionIndex < MissionManager.instance.lesserMissionList.Count){
                currentMission = MissionManager.instance.lesserMissionList[MissionManager.instance.currentMissionIndex];
            }

            if (currentMission)
            {
                associatedMission = currentMission;
            }
            else
            {
                associatedMission = null;
            }
        }

        //if no mission associated, disable the mission card
        if (associatedMission == null)
        {
            HideCard();
            return;
        }

        //if not tracking current mission, and the mission is the current mission, disable the mission card
        if (!(missionReferenceType == MissionManager.MissionRefernceType.CURRENT) && missionManager.currentMissionIndex != -1 && associatedMission == missionManager.lesserMissionList[missionManager.currentMissionIndex])
        {
            HideCard();
            return;
        }

        //otherwise show card
        ShowCard();

        //update text
        missionTitle.text = associatedMission.title;
        missionDescription.text = associatedMission.description;

        //update stamp
        missionStamp.enabled = associatedMission.isCompleted;

        //update button

        //if tracking current or completed, hide button
        if ((missionReferenceType == MissionManager.MissionRefernceType.CURRENT) || associatedMission.isCompleted)
        {
            button.SetActive(false);
        }
        else{
            //else, show button and update text
            if (missionManager && missionManager.currentMissionIndex != -1)
            {
                buttonText.text = "Switch";
            }
            else
            {
                buttonText.text = "Accept";
            }
        }
    }

    private void ShowCard()
    {
        missionTitle.enabled = true;
        missionDescription.enabled = true;
        missionStamp.enabled = true;
        backgroundImage.enabled = true;
        button.SetActive(true);
    }

    private void HideCard()
    {
        missionTitle.enabled = false;
        missionDescription.enabled = false;
        missionStamp.enabled = false;
        backgroundImage.enabled = false;
        button.SetActive(false);
    }
}
