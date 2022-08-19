using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Custom editor styles for easier styling.
/// </summary>
public class CustomEditorStuff
{
    public static GUIStyle center_bold_label = new GUIStyle(EditorStyles.label) {
        alignment = TextAnchor.MiddleCenter,
        fontStyle = FontStyle.Bold
    };

    public static GUIStyle center_label = new GUIStyle(EditorStyles.label) {
        alignment = TextAnchor.MiddleCenter
    };

    public static GUIStyle bold_label = new GUIStyle(EditorStyles.label) {
        fontStyle = FontStyle.Bold
    };

    [MenuItem("Dev/Add Required")]
    public static void AddRequired()
    {
        // get all prefabs in Assets/Prefabs/Managers/Required
        string[] prefabs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Prefabs/Managers/Required" });
        // add them to scene if they dont already exist
        foreach (string prefab in prefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefab);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null){
                Debug.LogError("Could not load prefab at path: " + path);
                continue;
            }
            if (GameObject.Find(go.name) == null){
                Debug.Log("Adding prefab: " + go.name);
                PrefabUtility.InstantiatePrefab(go);

                // make scene to be saved
                EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }
    }

}



/// <summary>
/// ReadOnly attribute for serialized fields.
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute
{
}
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
     public override float GetPropertyHeight(SerializedProperty property,
                                             GUIContent label)
     {
         return EditorGUI.GetPropertyHeight(property, label, true);
     }
     public override void OnGUI(Rect position,
                                SerializedProperty property,
                                GUIContent label)
     {
         GUI.enabled = false;
         EditorGUI.PropertyField(position, property, label, true);
         GUI.enabled = true;
     }
}

/// <summary>
/// Custom Input Dialog for Unity Editor <br/>
/// Retrieved From: https://forum.unity.com/threads/is-there-a-way-to-input-text-using-a-unity-editor-utility.473743/#post-7191802 <br/>
/// By User: https://forum.unity.com/members/vedran_m.4124355/
/// </summary>
public class EditorInputDialog : EditorWindow
{
    string  description, inputText;
    string  okButton, cancelButton;
    bool    initializedPosition = false;
    System.Action  onOKButton;
 
    bool    shouldClose = false;
    Vector2 maxScreenPos;
 
    #region OnGUI()
    void OnGUI()
    {
        // Check if Esc/Return have been pressed
        var e = Event.current;
        if( e.type == EventType.KeyDown )
        {
            switch( e.keyCode )
            {
                // Escape pressed
                case KeyCode.Escape:
                    shouldClose = true;
                    e.Use();
                    break;
 
                // Enter pressed
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    onOKButton?.Invoke();
                    shouldClose = true;
                    e.Use();
                    break;
            }
        }
 
        if( shouldClose ) {  // Close this dialog
            Close();
            //return;
        }
 
        // Draw our control
        var rect = EditorGUILayout.BeginVertical();
 
        EditorGUILayout.Space( 12 );
        EditorGUILayout.LabelField( description );
 
        EditorGUILayout.Space( 8 );
        GUI.SetNextControlName( "inText" );
        inputText = EditorGUILayout.TextField( "", inputText );
        GUI.FocusControl( "inText" );   // Focus text field
        EditorGUILayout.Space( 12 );
 
        // Draw OK / Cancel buttons
        var r = EditorGUILayout.GetControlRect();
        r.width /= 2;
        if( GUI.Button( r, okButton ) ) {
            onOKButton?.Invoke();
            shouldClose = true;
        }
 
        r.x += r.width;
        if( GUI.Button( r, cancelButton ) ) {
            inputText = null;   // Cancel - delete inputText
            shouldClose = true;
        }
 
        EditorGUILayout.Space( 8 );
        EditorGUILayout.EndVertical();
 
        // Force change size of the window
        if( rect.width != 0 && minSize != rect.size ) {
            minSize = maxSize = rect.size;
        }
 
        // Set dialog position next to mouse position
        if( !initializedPosition && e.type == EventType.Layout )
        {
            initializedPosition = true;
 
            // Move window to a new position. Make sure we're inside visible window
            var mousePos = GUIUtility.GUIToScreenPoint( Event.current.mousePosition );
            mousePos.x += 32;
            if( mousePos.x + position.width > maxScreenPos.x ) mousePos.x -= position.width + 64; // Display on left side of mouse
            if( mousePos.y + position.height > maxScreenPos.y ) mousePos.y = maxScreenPos.y - position.height;
 
            position = new Rect( mousePos.x, mousePos.y, position.width, position.height );
 
            // Focus current window
            Focus();
        }
    }
    #endregion OnGUI()
 
    #region Show()
    /// <summary>
    /// Returns text player entered, or null if player cancelled the dialog.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="description"></param>
    /// <param name="inputText"></param>
    /// <param name="okButton"></param>
    /// <param name="cancelButton"></param>
    /// <returns></returns>
    public static string Show( string title, string description, string inputText, string okButton = "OK", string cancelButton = "Cancel" )
    {
        // Make sure our popup is always inside parent window, and never offscreen
        // So get caller's window size
        var maxPos = GUIUtility.GUIToScreenPoint( new Vector2( Screen.width, Screen.height ) );
 
        string ret = null;
        //var window = EditorWindow.GetWindow<InputDialog>();
        var window = CreateInstance<EditorInputDialog>();
        window.maxScreenPos = maxPos;
        window.titleContent = new GUIContent( title );
        window.description = description;
        window.inputText = inputText;
        window.okButton = okButton;
        window.cancelButton = cancelButton;
        window.onOKButton += () => ret = window.inputText;
        //window.ShowPopup();
        window.ShowModal();
 
        return ret;
    }
    #endregion Show()
}
#endif
