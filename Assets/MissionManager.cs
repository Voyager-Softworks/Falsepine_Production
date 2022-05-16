using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public List<Mission> missionList;
    public int currentMissionIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTownUI(){
        //update the town UI
        PanelInn panelInn = FindObjectOfType<PanelInn>();

        if (panelInn == null)
        {
            Debug.Log("No PanelInn found in the scene");
            return;
        }

        for (int i = 0; i < panelInn.missionCardUIList.Count; i++)
        {
            MissionCardUI missionCardUI = panelInn.missionCardUIList[i].GetComponent<MissionCardUI>();
            if (missionCardUI == null) continue;

            //bind associated mission to the UI
            missionCardUI.associatedMission = missionList[i];

            missionCardUI.gameObject.SetActive(false);

            if (i == currentMissionIndex) continue;

            if (i < missionList.Count)
            {
                missionCardUI.gameObject.SetActive(true);
                missionCardUI.missionTitle.text = missionList[i].title;
                missionCardUI.missionDescription.text = missionList[i].description;

                if (currentMissionIndex == -1)
                {
                    missionCardUI.buttonText.text = "Accept";
                }
                else
                {
                    missionCardUI.buttonText.text = "Switch";
                }
            }
        }
    }

    /// <summary>
    /// Try to accept and start the given mission
    /// </summary>
    public void TryStartMission(Mission mission){
        if (mission == null) return;

        int missionIndex = missionList.IndexOf(mission);

        if (missionIndex == -1) return;

        currentMissionIndex = missionIndex;

        //update the town UI
        UpdateTownUI();
    }
}