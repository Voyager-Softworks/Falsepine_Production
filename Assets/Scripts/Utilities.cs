using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


 #if UNITY_EDITOR
 using UnityEditor;
 #endif
 
 /// <summary>
 /// Some useful utilities for the game.
 /// </summary>
 namespace Utilities
 {
     [System.Serializable]
     public class SceneField
     {
         [SerializeField] private UnityEngine.Object m_sceneAsset;
         public UnityEngine.Object SceneAsset { 
            get { return m_sceneAsset; }
            #if UNITY_EDITOR
            set { 
                m_sceneAsset = value;
                var fullScenePath = AssetDatabase.GetAssetPath(m_sceneAsset);
                var assetsIndex = fullScenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                var extensionIndex = fullScenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                fullScenePath = fullScenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                m_scenePath = fullScenePath;
            }
            #endif
        }
    

         [SerializeField] private string m_scenePath = "";
         public string scenePath
         {
             get { return m_scenePath; }
         }
 
         // makes it work with the existing Unity methods (LoadLevel/LoadScene)
         public static implicit operator string(SceneField sceneField)
         {
             return sceneField.scenePath;
         }
     }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty sceneAsset = property.FindPropertyRelative("m_sceneAsset");
            SerializedProperty scenePath = property.FindPropertyRelative("m_scenePath");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null)
            {
                //if game is running
                if (Application.isPlaying)
                {
                    // display the path
                    EditorGUI.LabelField(position, scenePath.stringValue);
                }
                else{
                    EditorGUI.BeginChangeCheck();
                    UnityEngine.Object value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

                    // get scene path currently stored in the property
                    string scenePathCurrently = scenePath.stringValue;

                    // get the actual scene path from the sceneAsset
                    string scenePathFromAsset = "";
                    if (value != null)
                    {
                        scenePathFromAsset = AssetDatabase.GetAssetPath(value);

                        int assetsIndex = scenePathFromAsset.IndexOf("Assets", StringComparison.Ordinal) + 7;
                        int extensionIndex = scenePathFromAsset.LastIndexOf(".unity", StringComparison.Ordinal);
                        scenePathFromAsset = scenePathFromAsset.Substring(assetsIndex, extensionIndex - assetsIndex);
                    }
                    // if the scene path from the asset is different from the one stored in the property, update the property
                    if (scenePathFromAsset != scenePathCurrently)
                    {
                        scenePath.stringValue = scenePathFromAsset;
                    }

                    // if any changes, update the asset and the scene path
                    if (EditorGUI.EndChangeCheck())
                    {
                        sceneAsset.objectReferenceValue = value;
                        if (sceneAsset.objectReferenceValue != null)
                        {
                            string fullScenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                            int assetsIndex = fullScenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                            int extensionIndex = fullScenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                            fullScenePath = fullScenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                            scenePath.stringValue = fullScenePath;
                        }
                    }
                }
                
            }
            EditorGUI.EndProperty();
        }
    }
    #endif
}