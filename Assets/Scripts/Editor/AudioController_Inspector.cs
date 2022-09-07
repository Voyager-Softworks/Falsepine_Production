using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using UnityEditor.Rendering;

[CustomEditor(typeof(AudioController))]
public class AudioController_Inspector : Editor
{
    AudioController musicManager;
    SerializedProperty audioChannels;
    ReorderableList audioChannelList;

    SerializedProperty transitions;
    ReorderableList transitionList;

    List<float> heights = new List<float>();

    private void OnEnable()
    {
        musicManager = (AudioController)target;
        audioChannels = serializedObject.FindProperty("audioChannels");
        audioChannelList = new ReorderableList(serializedObject, audioChannels, true, true, true, true);
        heights.Clear();
        heights = new List<float>(audioChannels.arraySize);
        audioChannelList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Audio Channels");
        };
        audioChannelList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = audioChannelList.serializedProperty.GetArrayElementAtIndex(index);
            float height = EditorGUIUtility.singleLineHeight * 1.25f;

            EditorGUI.ProgressBar(new Rect(rect.x, rect.y, rect.width * 0.6f, EditorGUIUtility.singleLineHeight), musicManager.audioChannels[index].timeNormalized, musicManager.audioChannels[index].name);

            if (musicManager.audioChannels[index].playing)
            {
                if (GUI.Button(new Rect(rect.x + rect.width * 0.6f, rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight), "Pause"))
                {
                    if (Application.isPlaying)
                        musicManager.Pause(index);
                }
            }
            else
            {
                if (GUI.Button(new Rect(rect.x + rect.width * 0.6f, rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight), "Play"))
                {
                    if (Application.isPlaying)
                        musicManager.Play(index);
                }
            }
            if (GUI.Button(new Rect(rect.x + rect.width * 0.8f, rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight), "Stop"))
            {
                if (Application.isPlaying)
                    musicManager.Stop(index);
            }

            //Make a foldout menu for each channel
            if (EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.isExpanded, ""))
            {
                height += EditorGUIUtility.singleLineHeight * 1.25f * 10;
                element.isExpanded = true;
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("volume"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("pitch"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("clip"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 5, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loop"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 6, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("playOnAwake"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 7, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("SpatialBlend"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 8, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("maxDistance"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 9, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("minDistance"));
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 10, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("distanceCurve"));
                EditorGUI.indentLevel--;
            }
            else
            {
                element.isExpanded = false;
            }
            heights[index] = height;
        };
        audioChannelList.elementHeightCallback = (int index) =>
        {
            Repaint();
            float height = EditorGUIUtility.singleLineHeight;

            try
            {
                height = heights[index];
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Debug.LogWarning(e.Message);
            }
            finally
            {
                float[] floats = heights.ToArray();
                Array.Resize(ref floats, audioChannels.arraySize);
                heights = floats.ToList();
            }

            return height;
        };
        audioChannelList.onReorderCallback = (ReorderableList list) =>
        {
            // Reorder the heights list
            float[] floats = heights.ToArray();
            Array.Resize(ref floats, audioChannels.arraySize);
            heights = floats.ToList();
            for (int i = 0; i < heights.Count; i++)
            {
                if (heights[i] == 0)
                {
                    heights[i] = EditorGUIUtility.singleLineHeight;
                }
            }
            Repaint();
        };
        audioChannelList.onAddCallback = (ReorderableList list) =>
        {
            if (audioChannels.arraySize == 0)
            {
                audioChannels.arraySize++;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue = "Audio Channel 0";
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("volume").floatValue = 1;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("pitch").floatValue = 1;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("clip").objectReferenceValue = null;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("loop").boolValue = false;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("playOnAwake").boolValue = false;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("SpatialBlend").floatValue = 0;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("maxDistance").floatValue = 300;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("minDistance").floatValue = 1;
                audioChannels.GetArrayElementAtIndex(0).FindPropertyRelative("distanceCurve").animationCurveValue = AnimationCurve.Linear(0, 1, 1, 0);
                heights.Add(EditorGUIUtility.singleLineHeight);
            }
            else
            {
                audioChannels.arraySize++;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("name").stringValue = "Audio Channel " + (audioChannels.arraySize - 1);
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("volume").floatValue = 1;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("pitch").floatValue = 1;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("clip").objectReferenceValue = null;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("loop").boolValue = false;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("playOnAwake").boolValue = false;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("SpatialBlend").floatValue = 0;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("maxDistance").floatValue = 300;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("minDistance").floatValue = 1;
                audioChannels.GetArrayElementAtIndex(audioChannels.arraySize - 1).FindPropertyRelative("distanceCurve").animationCurveValue = AnimationCurve.Linear(0, 1, 1, 0);
                heights.Add(EditorGUIUtility.singleLineHeight);
            }
        };



    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        audioChannelList.DoLayoutList();

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("transitions"));

        foreach (var trig in musicManager.triggers)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Trigger: " + trig))
            {
                musicManager.Trigger(trig);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("triggers"));

        EditorGUILayout.Space();



        serializedObject.ApplyModifiedProperties();
    }
}
