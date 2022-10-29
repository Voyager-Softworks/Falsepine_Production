using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalMissionEmbark : MonoBehaviour
{
    public Button embarkButton;
    public Utilities.SceneField finalMissionScene;

    // Start is called before the first frame update
    void Start()
    {
        CheckForReplacement();
    }

    private void CheckForReplacement()
    {
        MissionManager mm = MissionManager.instance;
        if (mm == null)
        {
            Debug.LogError("FinalMissionEmbark: MissionManager not found");
            return;
        }

        // if this is almost the final zone, replace button
        if (mm.GetZoneIndex(mm.GetCurrentZone()) >= mm.m_missionZones.Count - 2)
        {
            ReplaceButton();
        }
    }

    public void ReplaceButton()
    {
        embarkButton.onClick.RemoveAllListeners();
        embarkButton.onClick.AddListener(() => {
            // go to next zone
            MissionManager.instance.GoToNextZone();
            // save missions
            MissionManager.instance.SaveMissions(SaveManager.currentSaveSlot);
            MissionManager.instance.TryEmbark();
        });
    }
}
