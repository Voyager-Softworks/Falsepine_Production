using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionUIManager : MonoBehaviour
{
    public List<MissionCardUI> missionCardUIList;

    public GameObject unlockButton;
    public GameObject iconLocked;
    public GameObject iconUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable() {
        // bind button
        Button button = unlockButton.GetComponent<Button>();
        button.onClick.AddListener(ButtonPressed);
    }

    private void OnDisable() {
        // unbind button
        Button button = unlockButton.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
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
        for (int i = 0; i < missionCardUIList.Count; i++)
        {
            missionCardUIList[i].gameObject.SetActive(true);
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
