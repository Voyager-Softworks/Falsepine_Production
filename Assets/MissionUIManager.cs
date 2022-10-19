using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionUIManager : MonoBehaviour
{
    public List<MissionCardUI> missionCardUIList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable() {
    }

    private void OnDisable() {
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
        
        DrawPage();
    }

    /// <summary>
    /// Draws the mission cards for the missions.
    /// </summary>
    public void DrawPage(){

        // update mission cards
        for (int i = 0; i < missionCardUIList.Count; i++)
        {
            missionCardUIList[i].gameObject.SetActive(true);
        }
    }
}
