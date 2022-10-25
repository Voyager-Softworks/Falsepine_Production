using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Responsible for updating a journal page with the latest information about the mission.
/// </summary>
public class JournalUpdater_Mission : JournalContentUpdater
{
    public MissionCardUI m_missionCardUI;
    //public TextMeshProUGUI m_conditionText;

    protected override void Update() {
        UpdateContent();
    }

    public override void UpdateContent()
    {
        base.UpdateContent();

        // add conditions to description
        // if (m_missionCardUI.associatedMission){
        //     m_conditionText.text = "";
        //     foreach (MissionCondition condition in m_missionCardUI.associatedMission.m_conditions)
        //     {
        //         Color conditionCol = Color.white;
        //         switch (condition.GetState())
        //         {
        //             case MissionCondition.ConditionState.COMPLETE:
        //                 conditionCol = Color.green;
        //                 break;
        //             case MissionCondition.ConditionState.INCOMPLETE:
        //                 conditionCol = Color.white;
        //                 break;
        //             case MissionCondition.ConditionState.FAILED:
        //                 conditionCol = Color.red;
        //                 break;
        //         }

        //         // show condition, and coloured sprite to show state
        //         m_conditionText.text += (m_conditionText.text == "" ? "" : "\n") + "<sprite=0 color=#" + ColorUtility.ToHtmlStringRGB(conditionCol) + ">" + condition.GetDescription();
        //     }
        // }
    }
}
