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
/// An example of a custom item type that can be used in the item database.
/// </summary>
//Make sure to set the [Serializable] attribute on this class, so that it can be saved!
[Serializable]
public class SecondaryRangedWeapon : RangedWeapon
{
    // make sure to set variables as serialized fields in the inspector, so that they can be saved!

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
        SecondaryRangedWeapon newItem = (SecondaryRangedWeapon)base.CreateInstance();

        // Setting unique values here:

        return newItem;
    }

    
    //Custom editor for this class
    #if UNITY_EDITOR
    [CustomEditor(typeof(SecondaryRangedWeapon))]
    public class SecondaryRangedWeaponEditor : RangedWeaponEditor
    {
        public override void OnInspectorGUI()
        {
            // [REQUIRED] draw base editor (Item in this case)
            base.OnInspectorGUI();
            // [REQUIRED] get the editor target
            SecondaryRangedWeapon item = (SecondaryRangedWeapon)target;

            return;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            // bold center text
            GUILayout.Label("SecondaryRangedWeapon Stats", CustomEditorStuff.center_bold_label);

            // Your custom values here:

            //end red box
            GUILayout.EndVertical();
        }
    }
    #endif
	
	
}
