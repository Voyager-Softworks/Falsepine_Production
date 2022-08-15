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
public class Equipment : Item
{
    // make sure to set variables as serialized fields in the inspector, so that they can be saved!
    [SerializeField] public GameObject m_equipmentPrefab;

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
        Equipment newItem = (Equipment)base.CreateInstance();

        // Setting unique values here:
        // example valye
        newItem.m_equipmentPrefab = m_equipmentPrefab;

        return newItem;
    }

    // update
    public override void ManualUpdate(GameObject _owner)
    {
        base.ManualUpdate(_owner);
    }

    public void TossPrefab(Transform _throwTransform, Vector3 _velocity, GameObject _owner)
    {
        GameObject newEquipment = Instantiate(m_equipmentPrefab, _throwTransform.position, Quaternion.identity);
        newEquipment.GetComponent<ItemThrow>()?.TossPrefab(_throwTransform, _velocity, _owner);
    }

    
    //Custom editor for this class
    #if UNITY_EDITOR
    [CustomEditor(typeof(Equipment))]
    public class EquipmentEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            // [REQUIRED] draw base editor (Item in this case)
            base.OnInspectorGUI();
            // [REQUIRED] get the editor target
            Equipment item = (Equipment)target;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            // bold center text
            GUILayout.Label("Equipment Stats", CustomEditorStuff.center_bold_label);

            // Your custom values here
            item.m_equipmentPrefab = (GameObject)EditorGUILayout.ObjectField("Equipment Prefab", item.m_equipmentPrefab, typeof(GameObject), false);

            //end red box
            GUILayout.EndVertical();

            // on change, save the changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(item);
            }
        }
    }
    #endif
	
	
}
