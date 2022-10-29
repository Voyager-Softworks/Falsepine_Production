using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Toggleable window script for town buildings as they cannot inherit from the base script.
/// </summary>
public class ToggleableTownWindow : ToggleableWindow
{
    public TownBuilding building;

    public override bool IsOpen()
    {
        if (building == null || building.UI == null) return false;
        return building.UI.activeSelf;
    }
    public override void OpenWindow()
    {
        // if tutorial popup is open, return
        if (FindObjectOfType<TutorialPopup>() && FindObjectOfType<TutorialPopup>().gameObject.activeSelf) {
            return;
        }

        base.OpenWindow();
        building.OpenUI();
    }
    public override void CloseWindow()
    {
        base.CloseWindow();
        building.CloseUI();
    }
}
