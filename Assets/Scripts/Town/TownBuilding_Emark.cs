using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (MissionManager.instance.currentMission == null){
            worldText.text = "Contract Required!";
        }
        //if contract already completed, say so
        else if (MissionManager.instance.currentMission.isCompleted){
            worldText.text = "Turn in Contract!";
        }
        else{
            worldText.text = "Embark";
        }

        base.Update();
    }

    public override void OnClick()
    {
        base.OnClick();

        //load level 1 if valid
        if (MissionManager.instance.currentMission != null && !MissionManager.instance.currentMission.isCompleted){
            if (MissionManager.instance.currentMission.size == Mission.MissionSize.LESSER){
                LevelController.LoadSnow();
            }
            else if (MissionManager.instance.currentMission.size == Mission.MissionSize.GREATER){
                LevelController.LoadSnowBoss();
            }
        }
    }
}
