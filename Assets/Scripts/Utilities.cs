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
    /// <summary>
    /// A field for scene assets that can be used in the inspector.
    /// </summary>
    [System.Serializable]
    public class SceneField
    {
        [SerializeField] private UnityEngine.Object m_sceneAsset;
        public UnityEngine.Object SceneAsset
        {
            get { return m_sceneAsset; }
#if UNITY_EDITOR
            set
            {
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
    /// <summary>
    /// The property drawer for the SceneField class.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        private int fieldAmount = 2;
        private float fieldSize = 20;
        private float padding = 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //divide all field heights by the field amount..then minus the padding
            position.height /= fieldAmount; position.height -= padding;

            SerializedProperty sceneAsset = property.FindPropertyRelative("m_sceneAsset");
            SerializedProperty scenePath = property.FindPropertyRelative("m_scenePath");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null)
            {
                //if game is running
                if (Application.isPlaying)
                {
                    fieldAmount = 1;

                    // display the path
                    EditorGUI.LabelField(position, scenePath.stringValue);
                }
                else
                {
                    fieldAmount = 2;

                    EditorGUI.BeginChangeCheck();

                    UnityEngine.Object value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

                    // if any changes, update the asset and the scene path
                    if (EditorGUI.EndChangeCheck())
                    {
                        sceneAsset.objectReferenceValue = value;
                        if (sceneAsset.objectReferenceValue != null)
                        {
                            string fullScenePath = GetScenePath(sceneAsset);
                            scenePath.stringValue = fullScenePath;
                        }
                    }

                    // get scene path currently stored in the property
                    string scenePathCurrently = scenePath.stringValue;

                    // get the actual scene path from the sceneAsset
                    string scenePathFromAsset = "";
                    if (value != null)
                    {
                        scenePathFromAsset = GetScenePath(sceneAsset);
                    }
                    // if the scene path from the asset is different from the one stored in the property, update the property
                    if (scenePathFromAsset != scenePathCurrently)
                    {
                        scenePath.stringValue = scenePathFromAsset;
                    }

                    // display path below the object field

                    position.y += fieldSize + padding; //offset position.y by field size
                    EditorGUI.LabelField(position, scenePath.stringValue);
                }
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Gets the path of a scene asset.
        /// </summary>
        /// <param name="sceneAsset"></param>
        /// <returns></returns>
        private static string GetScenePath(SerializedProperty sceneAsset)
        {
            string fullScenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
            int assetsIndex = fullScenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
            int extensionIndex = fullScenePath.LastIndexOf(".unity", StringComparison.Ordinal);
            fullScenePath = fullScenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
            return fullScenePath;
        }

        /// <summary>
        /// Calculates the height of the property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //set the height of the drawer by the field size and padding
            return (fieldSize * fieldAmount) + (padding * fieldAmount);
        }
    }
#endif
}