using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Town building for the embark sign.
/// </summary>
public class TownBuilding_Emark : TownBuilding
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (MissionManager.instance == null) return;
        //if no contract, say so
        // if (MissionManager.instance.GetCurrentMission() == null){
        //     worldText.text = "Contract Required!";
        // }
        
        //if contract already completed, say so
        if (MissionManager.instance.GetCurrentMission()?.GetState() == MissionCondition.ConditionState.COMPLETE){
            worldText.text = "Turn in Contract!";
        }
        else{
            worldText.text = "Embark";
        }

        base.Update();
    }

    /// <summary>
    /// Try to emabrk
    /// </summary>
    public override void OnClick()
    {
        base.OnClick();

        //load level 1 if valid
        if (MissionManager.instance != null) {
            MissionManager.instance.TryEmbark();
        }
    }
}
