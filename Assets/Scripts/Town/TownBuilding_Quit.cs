using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownBuilding_Quit : TownBuilding  /// @todo Comment
{
    public override void OnClick()
    {
        base.OnClick();

        LevelController.LoadMenu();
    }
}
