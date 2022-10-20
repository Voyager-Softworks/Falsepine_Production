using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This class interfaces with the UIAudioManager to play UI sounds by storing a specific field in the UIAudioManager
/// </summary>
public class UIAudioRequester : MonoBehaviour
{
    // store a specific field in the UIAudioManager
    public Button button;
    [ReadOnly] public string soundName;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
    }

    // play the sound
    public void PlaySound()
    {
        if (UIAudioManager.instance == null) return;

        FieldInfo field = UIAudioManager.instance.GetType().GetField(soundName);
        if (field == null) return;

        UIAudioManager.UISound sound = (UIAudioManager.UISound)field.GetValue(UIAudioManager.instance);
        if (sound == null) return;

        sound.Play();
    }

    private void OnEnable() {
        // bind this to the button's onClick event
        if (button != null) button.onClick.AddListener(PlaySound);
    }

    private void OnDisable() {
        // unbind this from the button's onClick event
        if (button != null) button.onClick.RemoveListener(PlaySound);
    }

    #if UNITY_EDITOR
    private void OnValidate() {
        // get button component if it's not set
        if (button == null)
        {
            button = GetComponent<Button>();

            // set dirty
            EditorUtility.SetDirty(this);
        }

        // if the soundName is not a field in the UIAudioManager, set it to null
        // get all fields in the UIAudioManager
        FieldInfo[] fields = typeof(UIAudioManager).GetFields(BindingFlags.Public | BindingFlags.Instance);
        bool found = false;
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(UIAudioManager.UISound))
            {
                if (field.Name == soundName)
                {
                    found = true;
                    break;
                }
            }
        }
        if (!found) soundName = null;
    }

    // custom inspector
    [CustomEditor(typeof(UIAudioRequester))]
    public class UIAudioRequesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            UIAudioRequester requester = (UIAudioRequester)target;

            // get button component if it's not set
            if (requester.button == null)
            {
                requester.button = requester.GetComponent<Button>();

                // set dirty
                EditorUtility.SetDirty(requester);
            }

            // generic menu to select a sound
            if (string.IsNullOrEmpty(requester.soundName) ? GUILayout.Button("Select Sound") : GUILayout.Button(requester.soundName))
            {
                GenericMenu menu = new GenericMenu();

                // get all fields in the UIAudioManager
                FieldInfo[] fields = typeof(UIAudioManager).GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    // only add fields that are of type UISound
                    if (field.FieldType == typeof(UIAudioManager.UISound))
                    {
                        // add menu item
                        menu.AddItem(new GUIContent(field.Name), false, () =>
                        {
                            requester.soundName = field.Name;
                            EditorUtility.SetDirty(requester);
                        });
                    }
                }

                menu.ShowAsContext();
            }
        }
    }
    #endif
}
