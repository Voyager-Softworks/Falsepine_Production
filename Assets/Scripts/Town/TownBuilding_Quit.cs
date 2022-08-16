using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Town building for the quit sign.
/// @todo remove this? replaced by pause menu
/// </summary>
public class TownBuilding_Quit : TownBuilding
{
    public override void OnClick()
    {
        base.OnClick();

        LevelController.LoadMenu();
    }
}
