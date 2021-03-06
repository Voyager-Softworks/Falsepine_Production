using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MissionCardUI : MonoBehaviour /// @todo Comment
{
    [Header("Self")]
    public GameObject missionCard;

    [Header("Main")]
    public TextMeshProUGUI missionTitle;
    public TextMeshProUGUI missionDescription;
    public Image backgroundImage;
    public GameObject takenGroup;
    public Image missionStamp;
    public Image dropShadow;

    [Header("Button")]
    public GameObject button;
    public TextMeshProUGUI buttonText;

    [HideInInspector] public Mission associatedMission;

    [Header("Tracking")]
    public bool trackCurrentMission = false;
    public Mission.MissionSize missionSize;
    public int missionIndex;

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        UpdateCard();
    }

    public void ButtonClicked(){
        if (MissionManager.instance == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        //if the current mission is this mission, return the mission
        if (associatedMission == MissionManager.instance.currentMission)
        {
            MissionManager.instance.TryReturnMission();
        }
        else
        {
            //otherwise, start the mission
            MissionManager.instance.TryStartMission(associatedMission);
        }

        // deselect the button
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UpdateCard(){
        if (MissionManager.instance == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        //update the associated mission
        associatedMission = MissionManager.instance.GetMission(missionSize, missionIndex);

        if (trackCurrentMission){
            //get current mission from manager
            associatedMission = MissionManager.instance.currentMission;
        }

        //if no mission associated, disable the mission card
        if (associatedMission == null)
        {
            HideCard();
            return;
        }

        //if not tracking current mission, and the mission is the current mission, say so
        if (!(trackCurrentMission) && MissionManager.instance.currentMission != null && associatedMission == MissionManager.instance.currentMission)
        {
            ShowCard();

            missionTitle.text = "Mission Taken";
            if (associatedMission.isCompleted){
                missionDescription.text = "The mission is completed, turn it in.";
            }
            else{
                missionDescription.text = "Check the journal.\nEmbark to complete the mission.";
            }

            //set title and descript to be white
            missionTitle.color = Color.white;
            missionDescription.color = Color.white;

            //hide stamp
            missionStamp.enabled = false;

            //hide background image
            //backgroundImage.enabled = false;
            takenGroup.SetActive(true);

            //button
            button.SetActive(true);
            //if complete
            if (associatedMission.isCompleted)
            {
                buttonText.text = "Turn In";
            }
            else
            {
                buttonText.text = "Return";
            }
            return;
        }
        else {
            //otherwise show card
            ShowCard();

            //update text
            missionTitle.text = associatedMission.title;
            missionDescription.text = associatedMission.description;

            //update stamp
            missionStamp.enabled = associatedMission.isCompleted;

            //update background image
            //backgroundImage.enabled = true;
            takenGroup.SetActive(false);

            //update button

            //if tracking current or completed, hide button
            if ((trackCurrentMission) || associatedMission.isCompleted)
            {
                button.SetActive(false);
            }
            else{
                //else, show button and update text
                if (MissionManager.instance.currentMission != null)
                {
                    buttonText.text = "Switch";
                }
                else
                {
                    buttonText.text = "Accept";
                }
            }
        }
    }

    private void ShowCard()
    {
        missionTitle.enabled = true;
        missionDescription.enabled = true;
        //set title and descript to be black
        missionTitle.color = Color.black;
        missionDescription.color = Color.black;

        missionStamp.enabled = true;
        backgroundImage.enabled = true;
        dropShadow.enabled = true;
        button.SetActive(true);
    }

    private void HideCard()
    {
        missionTitle.enabled = false;
        missionDescription.enabled = false;
        missionStamp.enabled = false;
        backgroundImage.enabled = false;
        dropShadow.enabled = false;
        takenGroup.SetActive(false);
        button.SetActive(false);
    }
}
