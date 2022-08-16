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

/// <summary>
/// Town building for the mission board.
/// </summary>
public class TownBuilding_MissionBoard : TownBuilding
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

        LoadMissionBoard(SaveManager.currentSaveSlot);
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

    /// <summary>
    /// Saves the state of the mission board.
    /// </summary>
    /// <param name="_saveSlot"></param>
    public void SaveMissionBoard(int _saveSlot){
        //save the missionboard current page
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SaveManager.GetSaveFolderPath(_saveSlot) + "/missionboard.dat");
        MissionBoardData data = new MissionBoardData();
        data.currentPage = currentPage;
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Loads the state of the mission board.
    /// </summary>
    public void LoadMissionBoard(int _saveSlot){
        if (File.Exists(SaveManager.GetSaveFolderPath(_saveSlot) + "/missionboard.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(SaveManager.GetSaveFolderPath(_saveSlot) + "/missionboard.dat", FileMode.Open);
            MissionBoardData data = (MissionBoardData)bf.Deserialize(file);
            file.Close();

            currentPage = data.currentPage;
        }
    }

    /// <summary>
    /// Updates the UI of the mission board, including the mission cards within.
    /// </summary>
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

    /// <summary>
    /// Counts how many of the current missions are turned in, based on total that can be turned in.
    /// </summary>
    /// <param name="_size"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Called when the big unlock button is pressed.
    /// </summary>
    public void UnlockButtonPressed(){
        if (MissionManager.instance == null) return;
        if (bossUnlockButton == null) return;

        // Tries to go to the next page/zone.
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

    /// <summary>
    /// Sets the page of the mission board, and saves the state.
    /// </summary>
    /// <param name="_page"></param>
    public void SetPage(Mission.MissionSize _page){
        currentPage = _page;

        SaveMissionBoard(SaveManager.currentSaveSlot);
    }

    /// <summary>
    /// Draws the mission cards for the greater missions.
    /// </summary>
    public void DrawGreaterPage(){
        int greatersTurnedIn = CountTurnedIn(Mission.MissionSize.GREATER);

        // update unlock button
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

        // update mission cards
        for (int i = 0; i < lesserMissionCardUIList.Count; i++)
        {
            lesserMissionCardUIList[i].gameObject.SetActive(false);

        }
        for (int i = 0; i < greaterMissionCardUIList.Count; i++)
        {
            greaterMissionCardUIList[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Draws the mission cards for the lesser missions.
    /// </summary>
    public void DrawLesserPage(){

        // update unlock button
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

        // update mission cards
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
