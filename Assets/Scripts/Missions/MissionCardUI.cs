using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Mission card represents a mission that the player can interact with.
/// </summary>
public class MissionCardUI : MonoBehaviour
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

    #if UNITY_EDITOR
    [ReadOnly]
    #endif 
    public Mission associatedMission;

    [Header("Tracking")]
    public bool trackCurrentMission = false;
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

    /// <summary>
    /// Called when the button on the card is clicked.
    /// </summary>
    public void ButtonClicked(){
        if (MissionManager.instance == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        Mission cm = MissionManager.instance.GetCurrentMission();

        //if the current mission is this mission, return the mission
        if (associatedMission == cm)
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

    /// <summary>
    /// Updates the cards UI
    /// </summary>
    public void UpdateCard(){
        if (MissionManager.instance == null)
        {
            Debug.Log("No MissionManager found in the scene");
            return;
        }

        //update the associated mission
        associatedMission = MissionManager.instance.GetMission(missionIndex);

        if (trackCurrentMission){
            //get current mission from manager
            associatedMission = MissionManager.instance.GetCurrentMission();

            if (associatedMission == null){
                DisplayNoMission();
                return;
            }
        }
        //if not tracking mission, and no mission associated, disable the mission card
        else if (associatedMission == null)
        {
            HideCard();
            return;
        }

        //if not tracking current mission, and the mission is the current mission, say so
        if (!(trackCurrentMission) && MissionManager.instance.GetCurrentMission() != null && associatedMission == MissionManager.instance.GetCurrentMission())
        {
            ShowCard();

            missionTitle.text = "Mission Taken";
            switch (associatedMission.GetState()){
                case MissionCondition.ConditionState.COMPLETE:
                    missionDescription.text = "The mission is completed, turn it in.";
                    break;
                case MissionCondition.ConditionState.INCOMPLETE:
                    missionDescription.text = "Check the journal.\nEmbark to start the mission.";
                    break;
                case MissionCondition.ConditionState.FAILED:
                    missionDescription.text = "Mission Failed";
                    break;
            }
            if (associatedMission.GetState() == MissionCondition.ConditionState.COMPLETE){
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
            if (associatedMission.m_isCompleted)
            {
                buttonText.text = "Turn In";
            }
            else
            {
                // disable button
                button.SetActive(false);
            }
            return;
        }
        else {
            //otherwise show card
            ShowCard();

            //update text
            missionTitle.text = associatedMission.m_title;
            missionDescription.text = associatedMission.m_description;

            //update stamp
            missionStamp.enabled = associatedMission.m_isCompleted;

            //update background image
            //backgroundImage.enabled = true;
            takenGroup.SetActive(false);

            //update button

            //if tracking current or completed, hide button
            if ((trackCurrentMission) || associatedMission.m_isCompleted)
            {
                button.SetActive(false);
            }
            else{
                //else, show button and update text
                if (MissionManager.instance.GetCurrentMission() != null)
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

    /// <summary>
    /// Displays filler text to show that there is no mission to display.
    /// </summary>
    private void DisplayNoMission()
    {
        ShowCard();

        missionTitle.text = "No Bounty Selected";
        missionDescription.text = "Accept a Bounty from the bounty board in town.";

        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0.5f);

        missionStamp.enabled = false;
        dropShadow.enabled = false;
        takenGroup.SetActive(false);
        button.SetActive(false);
    }

    /// <summary>
    /// Shows the card completely.
    /// </summary>
    private void ShowCard()
    {
        missionTitle.enabled = true;
        missionDescription.enabled = true;
        //set title and descript to be black
        missionTitle.color = Color.black;
        missionDescription.color = Color.black;

        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 1.0f);

        missionStamp.enabled = true;
        backgroundImage.enabled = true;
        dropShadow.enabled = true;
        button.SetActive(true);
    }

    /// <summary>
    /// Hides the card completely.
    /// </summary>
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
