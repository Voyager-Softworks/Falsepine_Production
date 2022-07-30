using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


 #if UNITY_EDITOR
 using UnityEditor;
 #endif
 
 namespace Utilities
 {
     [System.Serializable]
     public class SceneField
     {
         [SerializeField] private UnityEngine.Object sceneAsset;
         [SerializeField] private string scenePath = "";
 
         public string ScenePath
         {
             get { return scenePath; }
         }
 
         // makes it work with the existing Unity methods (LoadLevel/LoadScene)
         public static implicit operator string(SceneField sceneField)
         {
             return sceneField.ScenePath;
         }
     }
 
    #if UNITY_EDITOR
     [CustomPropertyDrawer(typeof(SceneField))]
     public class SceneFieldPropertyDrawer : PropertyDrawer
     {
         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
         {
             EditorGUI.BeginProperty(position, GUIContent.none, property);
             var sceneAsset = property.FindPropertyRelative("sceneAsset");
             var scenePath = property.FindPropertyRelative("scenePath");
             position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
             if (sceneAsset != null)
             {
                 EditorGUI.BeginChangeCheck();
                 var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                 if (EditorGUI.EndChangeCheck())
                 {
                     sceneAsset.objectReferenceValue = value;
                     if (sceneAsset.objectReferenceValue != null)
                     {
                         var fullScenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                         var assetsIndex = fullScenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                         var extensionIndex = fullScenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                         fullScenePath = fullScenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                         scenePath.stringValue = fullScenePath;
                     }
                 }
             }
             EditorGUI.EndProperty();
         }
     }
    #endif
 }