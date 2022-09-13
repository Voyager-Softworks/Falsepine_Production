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
    public Image dropShadow;

    [Header("Stamp")]
    public Image missionStamp;
    public Sprite missionStampComplete;
    public Sprite missionStampFailed;
    public Sprite missionStampIncomplete;

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
    private int m_framesToWait = 0;
    void Update()
    {
        // every 20-30 frames, update the mission card
        if (m_framesToWait <= 0)
        {
            UpdateCardUI();
            m_framesToWait = UnityEngine.Random.Range(20, 30);
        }
        else
        {
            m_framesToWait--;
        }
    }

    private void OnEnable() {
        UpdateCardUI();
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
    public void UpdateCardUI(){
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
                    missionDescription.text = "Mission Failed. \nCancel the mission to try again.";
                    break;
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
            if (associatedMission.GetState() == MissionCondition.ConditionState.COMPLETE)
            {
                buttonText.text = "Turn In";
            }
            else
            {
                buttonText.text = "Cancel";
            }
            return;
        }
        else
        {
            //otherwise show card
            ShowCard();

            //update text
            Color missionCol = Color.white;
            switch (associatedMission.GetState()){
                case MissionCondition.ConditionState.COMPLETE:
                    missionCol = Color.green;
                    break;
                case MissionCondition.ConditionState.INCOMPLETE:
                    missionCol = Color.white;
                    break;
                case MissionCondition.ConditionState.FAILED:
                    missionCol = Color.red;
                    break;
            }
            missionTitle.text = "<sprite=0 color=#" + ColorUtility.ToHtmlStringRGB(missionCol) + ">" + associatedMission.m_title;
            missionDescription.text = associatedMission.m_description;
            // add conditions to description
            if (associatedMission){
                missionDescription.text += "\n\nConditions:";
                foreach (MissionCondition condition in associatedMission.m_conditions)
                {
                    Color conditionCol = Color.white;
                    switch (condition.GetState())
                    {
                        case MissionCondition.ConditionState.COMPLETE:
                            conditionCol = Color.green;
                            break;
                        case MissionCondition.ConditionState.INCOMPLETE:
                            conditionCol = Color.white;
                            break;
                        case MissionCondition.ConditionState.FAILED:
                            conditionCol = Color.red;
                            break;
                    }

                    // show condition, and coloured sprite to show state
                    missionDescription.text += "\n" + "<sprite=0 color=#" + ColorUtility.ToHtmlStringRGB(conditionCol) + ">" + condition.GetDescription();
                }
            }

            //update stamp
            //missionStamp.enabled = true;
            //SetStamp();

            //update background image
            //backgroundImage.enabled = true;
            takenGroup.SetActive(false);

            //update button

            //if tracking current or completed, hide button
            if ((trackCurrentMission) || associatedMission.GetState() == MissionCondition.ConditionState.COMPLETE)
            {
                button.SetActive(false);
            }
            else
            {
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

    private void SetStamp()
    {
        switch (associatedMission.GetState())
        {
            case MissionCondition.ConditionState.COMPLETE:
                missionStamp.sprite = missionStampComplete;
                break;
            case MissionCondition.ConditionState.INCOMPLETE:
                missionStamp.sprite = missionStampIncomplete;
                break;
            case MissionCondition.ConditionState.FAILED:
                missionStamp.sprite = missionStampFailed;
                break;
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

        //missionStamp.enabled = true;
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
