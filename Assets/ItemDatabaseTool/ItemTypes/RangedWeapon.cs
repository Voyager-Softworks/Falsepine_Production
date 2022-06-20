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
public class RangedWeapon : Item
{
    // make sure to set variables as serialized fields in the inspector, so that they can be saved!

    // Shooting Performance:
    [SerializeField] public float m_damage = 0;
    [SerializeField] public float m_range = 0;
    [SerializeField] public float m_shootTime = 0;
    [SerializeField] private float m_shootTimer = 0;

    // Aiming:
    [SerializeField] public float m_aimedInaccuracy = 0;
    [SerializeField] public float m_unaimedInaccuracy = 0;
    [SerializeField] public float m_aimTime = 0;
    [SerializeField] private float m_aimTimer = 0;

    // Ammunition:
    [SerializeField] public int m_clipAmmo = 0;
    [SerializeField] public int m_clipSize = 0;
    
    // Reloading:
    [SerializeField] public float m_reloadTime = 0;
    [SerializeField] private float m_reloadTimer = 0;
    [SerializeField] private float m_reloadAmount = 0;


    // Other:
    [SerializeField] public float m_equipTime = 0;
    [SerializeField] private float m_equipTimer = 0;
    

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
        RangedWeapon newItem = (RangedWeapon)base.CreateInstance();

        // Setting unique values here:
        // damage
        newItem.m_damage = m_damage;

        return newItem;
    }

    
    //Custom editor for this class
    #if UNITY_EDITOR
    [CustomEditor(typeof(RangedWeapon))]
    public class RangedWeaponEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            // [REQUIRED] draw base editor (Item in this case)
            base.OnInspectorGUI();
            // [REQUIRED] get the editor target
            RangedWeapon rangedWeapon = (RangedWeapon)target;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            // bold center text
            GUILayout.Label("Weapon Item Stats", CustomEditorStuff.center_bold_label);

            // horiz
            GUILayout.BeginHorizontal();

            // vertical
            GUILayout.BeginVertical();
            // performance
            GUILayout.Label("Shooting Performance", CustomEditorStuff.center_bold_label);
            // damage
            rangedWeapon.m_damage = EditorGUILayout.FloatField(new GUIContent("Damage", "The amount of damage this weapon does"), rangedWeapon.m_damage);
            // range
            rangedWeapon.m_range = EditorGUILayout.FloatField(new GUIContent("Range", "The range of this weapon"), rangedWeapon.m_range);
            // shoot time
            rangedWeapon.m_shootTime = EditorGUILayout.FloatField(new GUIContent("Shoot Time", "The time it takes to shoot\nThe delay after one shot"), rangedWeapon.m_shootTime);
            // end vertical
            GUILayout.EndVertical();

            //space
            GUILayout.Space(20);

            // vertical
            GUILayout.BeginVertical();
            // aiming
            GUILayout.Label("Aiming", CustomEditorStuff.center_bold_label);
            // aimed accuracy
            rangedWeapon.m_aimedInaccuracy = EditorGUILayout.FloatField(new GUIContent("Aimed Inaccuracy", "The amount of degrees +/- of inaccuracy when AIMED"), rangedWeapon.m_aimedInaccuracy);
            // unaimed accuracy
            rangedWeapon.m_unaimedInaccuracy = EditorGUILayout.FloatField(new GUIContent("Unaimed Inaccuracy", "The amount of degrees +/- of inaccuracy when UNAIMED"), rangedWeapon.m_unaimedInaccuracy);
            // aim time
            rangedWeapon.m_aimTime = EditorGUILayout.FloatField(new GUIContent("Aim Time", "The time it takes to aim"), rangedWeapon.m_aimTime);
            // end vertical
            GUILayout.EndVertical();

            // end horiz
            GUILayout.EndHorizontal();

            // ammunition
            GUILayout.Label("Ammunition", CustomEditorStuff.center_bold_label);
            // clip ammo
            rangedWeapon.m_clipAmmo = EditorGUILayout.IntField(new GUIContent("Clip Ammo", "The amount of ammo currently in the clip"), rangedWeapon.m_clipAmmo);
            // reserve ammo
            rangedWeapon.m_clipSize = EditorGUILayout.IntField(new GUIContent("Clip Size", "The amount of ammo the clip can carry"), rangedWeapon.m_clipSize);

            // reloading
            GUILayout.Label("Reloading", CustomEditorStuff.center_bold_label);
            // reload time
            rangedWeapon.m_reloadTime = EditorGUILayout.FloatField(new GUIContent("Reload Time", "The time it takes to reload"), rangedWeapon.m_reloadTime);
            // reload amount
            rangedWeapon.m_reloadAmount = EditorGUILayout.FloatField(new GUIContent("Reload Amount", "The amount of ammo to reload"), rangedWeapon.m_reloadAmount);

            // other
            GUILayout.Label("Other", CustomEditorStuff.center_bold_label);
            // equip time
            rangedWeapon.m_equipTime = EditorGUILayout.FloatField(new GUIContent("Equip Time", "The time it takes to equip"), rangedWeapon.m_equipTime);


            //end red box
            GUILayout.EndVertical();

            // on change, save the changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(rangedWeapon);
            }
        }
    }
    #endif


}