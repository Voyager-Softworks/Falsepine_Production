using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CampGameInteract : Interactable
{
    [Header("Camp Game Interact")]
    public ToggleableTownWindow building;

    public override void DoInteract()
    {
        base.DoInteract();

        building.OpenWindow();
    }

    #if UNITY_EDITOR
    private void OnValidate() {
        if (building == null)
        {
            building = GetComponentInParent<ToggleableTownWindow>();
            // set dirty
            EditorUtility.SetDirty(this);
        }
    }
    #endif
}