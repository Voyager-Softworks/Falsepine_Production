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
    public int lessersNeeded = 3;
    public int greatersNeeded = 1;

    public GameObject unlockButton;
    public GameObject iconLocked;
    public GameObject iconUnlocked;

    /// <summary>
    /// Gets the path to the save file for the mission board, at the save slot.
    /// </summary>
    /// <param name="_saveSlot"></param>
    /// <returns></returns>
    public static string GetSaveFolderPath(int _saveSlot)
    {
        return SaveManager.GetSaveFolderPath(_saveSlot) + "/missions/";
    }

    /// <summary>
    /// Gets the save file path for the current save slot.
    /// </summary>
    /// <param name="_saveSlot"></param>
    /// <returns></returns>
    public static string GetSaveFilePath(int _saveSlot)
    {
        return GetSaveFolderPath(_saveSlot) + "/missionboard.dat";
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
        // if the save folder doesn't exist, create it
        if (!Directory.Exists(GetSaveFolderPath(_saveSlot)))
        {
            Directory.CreateDirectory(GetSaveFolderPath(_saveSlot));
        }

        // This save is serialized, as it is not intended to be edited.
        BinaryFormatter bf = new BinaryFormatter();

        // make file
        FileStream file = File.Create(GetSaveFilePath(_saveSlot));

        // // save page data
        // MissionBoardData data = new MissionBoardData();
        // data.currentPage = currentPage;

        // // serialize data
        // bf.Serialize(file, data);
        
        file.Close();
    }

    /// <summary>
    /// Loads the state of the mission board.
    /// </summary>
    public void LoadMissionBoard(int _saveSlot){
        if (File.Exists(GetSaveFilePath(_saveSlot)))
        {
            // BinaryFormatter bf = new BinaryFormatter();
            // FileStream file = File.Open(GetSaveFilePath(_saveSlot), FileMode.Open);
            // MissionBoardData data = (MissionBoardData)bf.Deserialize(file);
            // file.Close();

            // currentPage = data.currentPage;
        }
    }

    /// <summary>
    /// Deletes the save file for the current save slot.
    /// </summary>
    /// <param name="_saveSlot"></param>
    public static void DeleteMissionBoardSave(int _saveSlot){
        if (File.Exists(GetSaveFilePath(_saveSlot)))
        {
            File.Delete(GetSaveFilePath(_saveSlot));
        }
    }

    /// <summary>
    /// Updates the UI of the mission board, including the mission cards within.
    /// </summary>
    private void UpdateUI()
    {
        if (MissionManager.instance == null) return;
        if (unlockButton == null) return;
        
        DrawPage();
    }

    /// <summary>
    /// Counts how many of the current missions are turned in, based on total that can be turned in.
    /// </summary>
    /// <param name="_size"></param>
    /// <returns></returns>
    private static int CountTurnedIn()
    {
        int totalTurnedIn = 0;
        for (int i = 0; i < MissionManager.instance.GetMissions().Count; i++)
        {
            if (MissionManager.instance.GetMissions()[i].GetState() == MissionCondition.ConditionState.COMPLETE && MissionManager.instance.GetMissions()[i] != MissionManager.instance.GetCurrentMission())
            {
                totalTurnedIn++;
            }
        }
        
        return totalTurnedIn;
    }

    /// <summary>
    /// Draws the mission cards for the missions.
    /// </summary>
    public void DrawPage(){

        bool thisZoneBossKilled = MissionManager.instance.HasZoneBossDied();

        // update unlock button
        if (!thisZoneBossKilled) {
            unlockButton.GetComponentInChildren<Button>().interactable = false;
            iconLocked.SetActive(true);
            iconUnlocked.SetActive(false);
            unlockButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            unlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Kill Boss First";
        }
        else {
            unlockButton.GetComponentInChildren<Button>().interactable = true;
            iconUnlocked.SetActive(true);
            iconLocked.SetActive(false);
            unlockButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            unlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go to Next Zone";
        }

        // update mission cards
        for (int i = 0; i < lesserMissionCardUIList.Count; i++)
        {
            lesserMissionCardUIList[i].gameObject.SetActive(true);
        }
    }

    public void ButtonPressed(){
        if (MissionManager.instance == null) return;
        if (MissionManager.instance.GetCurrentZone() == null) return;
        if (MissionManager.instance.GetCurrentZone().m_zoneMonsters == null) return;

        bool thisZoneBossKilled = MissionManager.instance.HasZoneBossDied();

        if (thisZoneBossKilled){
            MissionManager.instance.GoToNextZone();
        }

        ZoneNameUI[] zoneNameUIs = FindObjectsOfType<ZoneNameUI>();
        for (int i = 0; i < zoneNameUIs.Length; i++)
        {
            zoneNameUIs[i].UpdateText();
        }
    }
}
