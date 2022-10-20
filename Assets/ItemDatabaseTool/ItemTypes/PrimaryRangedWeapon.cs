using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

//[REQUIRED]
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Primary variant of a ranged weapon.
/// </summary>
//Make sure to set the [Serializable] attribute on this class, so that it can be saved!
[Serializable]
public class PrimaryRangedWeapon : RangedWeapon
{
    // make sure to set variables as serialized fields in the inspector, so that they can be saved!

    public override string GetTypeDisplayName(){
        return "Primary";
    }

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
        PrimaryRangedWeapon newItem = (PrimaryRangedWeapon)base.CreateInstance();

        // Setting unique values here:

        return newItem;
    }

    
    //Custom editor for this class
    #if UNITY_EDITOR
    [CustomEditor(typeof(PrimaryRangedWeapon))]
    public class PrimaryRangedWeaponEditor : RangedWeaponEditor
    {
        public override void OnInspectorGUI()
        {
            // [REQUIRED] draw base editor (Item in this case)
            base.OnInspectorGUI();
            // [REQUIRED] get the editor target
            PrimaryRangedWeapon item = (PrimaryRangedWeapon)target;

            return;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            // bold center text
            GUILayout.Label("PrimaryRangedWeapon Stats", CustomEditorStuff.center_bold_label);

            // Your custom values here:

            //end red box
            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(item);
            }
        }
    }
    #endif
	
	
}
