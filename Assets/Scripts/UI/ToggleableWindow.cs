using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

/// <summary>
/// Class that manages toggleable windows in game, only allowing one at a time to be open.
/// </summary>
public class ToggleableWindow : MonoBehaviour
{
    static List<ToggleableWindow> windows = new List<ToggleableWindow>();

    public bool m_wasOpenedThisFrame = false;
    public bool m_wasClosedThisFrame = false;

    private static bool m_closedAllThisFrame = false;

    protected virtual void OnEnable() {
        windows.Add(this);
    }

    protected virtual void OnDisable() {
        windows.Remove(this);
    }

    public virtual void Update() {
        // if escape is pressed, toggle pause
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !m_closedAllThisFrame)
        {
            if (ToggleableWindow.AnyWindowOpen()){
                ToggleableWindow.CloseAllWindows();
                m_closedAllThisFrame = true;
            }
        }
    }

    public virtual void LateUpdate() {
        //reset flags
        m_wasOpenedThisFrame = false;
        m_wasClosedThisFrame = false;
        m_closedAllThisFrame = false;
    }

    /// <summary>
    /// Checks if this UI is open
    /// </summary>
    /// <returns></returns>
    public virtual bool IsOpen(){
        return false;
    }

    /// <summary>
    /// Opens this UI
    /// </summary>
    public virtual void OpenWindow(){
        CloseAllWindows();
        m_wasOpenedThisFrame = true;
    }

    /// <summary>
    /// Closes this UI
    /// </summary>
    public virtual void CloseWindow(){
        m_wasClosedThisFrame = true;
    }

    /// <summary>
    /// Toggles this UI between open and closed
    /// </summary>
    public virtual void ToggleWindow(){
        if (IsOpen()){
            CloseWindow();
        } else {
            OpenWindow();
        }
    }

    /// <summary>
    /// Closes all Toggleable windows in scene
    /// </summary>
    public static void CloseAllWindows(){
        // get all toggleable windows
        windows = FindObjectsOfType<ToggleableWindow>().ToList();
        foreach(ToggleableWindow window in windows){
            if (window.m_wasOpenedThisFrame) continue;
            window.CloseWindow();
        }
    }

    /// <summary>
    /// Checks if any toggleable windows are open in the scene
    /// </summary>
    /// <returns></returns>
    public static bool AnyWindowOpen(){
        // get all toggleable windows
        foreach(ToggleableWindow window in windows){
            if (window.IsOpen()){
                return true;
            }
        }
        return false;
    }
}