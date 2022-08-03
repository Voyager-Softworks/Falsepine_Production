using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TownBuilding_MissionBoard : TownBuilding  /// @todo Comment
{
    public List<MissionCardUI> lesserMissionCardUIList;
    public List<MissionCardUI> greaterMissionCardUIList;
    public int lessersNeeded = 3;
    public int greatersNeeded = 1;

    public GameObject bossUnlockButton;
    public GameObject bossIconLocked;
    public GameObject bossIconUnlocked;

    public Mission.MissionSize currentPage = Mission.MissionSize.LESSER;

    [Serializable]
    public class MissionBoardData{
        public Mission.MissionSize currentPage = Mission.MissionSize.LESSER;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        LoadMissionBoard();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //if UI is acvitve, update the UI
        if (UI.activeSelf)
        {
            UpdateUI();
        }
    }

    public void SaveMissionBoard(){
        //save the missionboard current page
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/missionboard.dat");
        MissionBoardData data = new MissionBoardData();
        data.currentPage = currentPage;
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadMissionBoard(){
        if (File.Exists(Application.persistentDataPath + "/missionboard.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/missionboard.dat", FileMode.Open);
            MissionBoardData data = (MissionBoardData)bf.Deserialize(file);
            file.Close();

            currentPage = data.currentPage;
        }
    }

    private void UpdateUI()
    {
        if (MissionManager.instance == null) return;
        if (bossUnlockButton == null) return;
        int lessersTurnedIn = CountTurnedIn(Mission.MissionSize.LESSER);

        //if current page is greater, but not enough missions are turned in, change page
        if (currentPage == Mission.MissionSize.GREATER && lessersTurnedIn < greaterMissionCardUIList.Count)
        {
            SetPage(Mission.MissionSize.LESSER);
        }

        switch (currentPage)
        {
            case Mission.MissionSize.LESSER:
                DrawLesserPage();
                break;
            case Mission.MissionSize.GREATER:
                DrawGreaterPage();
                break;
            default:
                DrawLesserPage();
                break;
        }
    }

    private static int CountTurnedIn(Mission.MissionSize _size)
    {
        int totalTurnedIn = 0;
        switch (_size){
            case Mission.MissionSize.LESSER:
                for (int i = 0; i < MissionManager.instance.GetLesserMissions().Count; i++)
                {
                    if (MissionManager.instance.GetLesserMissions()[i].m_isCompleted && MissionManager.instance.GetLesserMissions()[i] != MissionManager.instance.GetCurrentMission())
                    {
                        totalTurnedIn++;
                    }
                }
                break;
            case Mission.MissionSize.GREATER:
                for (int i = 0; i < MissionManager.instance.GetGreaterMissions().Count; i++)
                {
                    if (MissionManager.instance.GetGreaterMissions()[i].m_isCompleted && MissionManager.instance.GetGreaterMissions()[i] != MissionManager.instance.GetCurrentMission())
                    {
                        totalTurnedIn++;
                    }
                }
                break;
        }
        

        return totalTurnedIn;
    }

    public void UnlockButtonPressed(){
        if (MissionManager.instance == null) return;
        if (bossUnlockButton == null) return;

        switch (currentPage)
        {
            case Mission.MissionSize.LESSER:
                SetPage(Mission.MissionSize.GREATER);
                break;
            case Mission.MissionSize.GREATER:
                if (MissionManager.instance) MissionManager.instance.ResetAllZones();
                SetPage(Mission.MissionSize.LESSER);
                break;
            default:
                SetPage(Mission.MissionSize.LESSER);
                break;
        }
    }

    public void SetPage(Mission.MissionSize _page){
        currentPage = _page;

        SaveMissionBoard();
    }

    public void DrawGreaterPage(){
        int greatersTurnedIn = CountTurnedIn(Mission.MissionSize.GREATER);

        if (greatersTurnedIn >= greatersNeeded)
        {
            bossUnlockButton.SetActive(true);
            bossUnlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unlock Next Zone";
            bossIconUnlocked.SetActive(true);
            bossIconLocked.SetActive(false);
        }
        else
        {
            bossUnlockButton.SetActive(false);
        }

        for (int i = 0; i < lesserMissionCardUIList.Count; i++)
        {
            lesserMissionCardUIList[i].gameObject.SetActive(false);

        }
        for (int i = 0; i < greaterMissionCardUIList.Count; i++)
        {
            greaterMissionCardUIList[i].gameObject.SetActive(true);
        }
    }

    public void DrawLesserPage(){

        bossUnlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unlock Greater Missions (" + CountTurnedIn(Mission.MissionSize.LESSER) + "/" + lessersNeeded + ")";
        if (CountTurnedIn(Mission.MissionSize.LESSER) < lessersNeeded) {
            bossUnlockButton.GetComponentInChildren<Button>().interactable = false;
            bossIconLocked.SetActive(true);
            bossIconUnlocked.SetActive(false);
            bossUnlockButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }
        else {
            bossUnlockButton.GetComponentInChildren<Button>().interactable = true;
            bossIconUnlocked.SetActive(true);
            bossIconLocked.SetActive(false);
            bossUnlockButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }

        for (int i = 0; i < lesserMissionCardUIList.Count; i++)
        {
            lesserMissionCardUIList[i].gameObject.SetActive(true);

        }
        for (int i = 0; i < greaterMissionCardUIList.Count; i++)
        {
            greaterMissionCardUIList[i].gameObject.SetActive(false);
        }
    }
}
