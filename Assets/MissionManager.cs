using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton donotdestroy script that handles the mission system
/// </summary>
public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

    public List<Mission> missionList;
    public int currentMissionIndex;

    void Awake() {
        if (instance == null) {
            instance = this;
            //do not destroy this object
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ReinstantiateMissions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Replace missions in list with new copies of the missions
    /// </summary>
    public void ReinstantiateMissions(){
        for (int i = 0; i < missionList.Count; i++)
        {
            missionList[i] = Instantiate(missionList[i]);
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

        UpdateAllMissionCards();
    }

    public void UpdateAllMissionCards(){
        MissionCardUI[] missionCardUIList = FindObjectsOfType<MissionCardUI>(true);

        if (missionCardUIList == null) return;

        for (int i = 0; i < missionCardUIList.Length; i++)
        {
            MissionCardUI missionCardUI = missionCardUIList[i];
            if (missionCardUI == null) continue;

            missionCardUI.UpdateUI();
        }
    }
}