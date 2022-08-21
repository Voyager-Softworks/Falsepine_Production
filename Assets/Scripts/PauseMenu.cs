using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages opening and closing the "pause" menu.
/// </summary>
public class PauseMenu : ToggleableWindow
{
    public InputAction pauseAction;

    public GameObject PausePanel;

    private void OnEnable() {
        pauseAction.Enable();
    }
    private void OnDisable() {
        pauseAction.Disable();
    }

    public override void Update() {
        if (pauseAction.WasPressedThisFrame()) {
            if (!ToggleableWindow.AnyWindowOpen() || IsOpen()) {
                ToggleWindow();
            }
        }

        base.Update();
    }

    public override bool IsOpen()
    {
        return PausePanel.activeSelf;
    }
    public override void OpenWindow()
    {
        if (m_wasClosedThisFrame) return;

        base.OpenWindow();
        PausePanel.SetActive(true);

        // pause the game
        LevelController.RequestPause(this);
    }
    public override void CloseWindow()
    {
        base.CloseWindow();
        PausePanel.SetActive(false);

        // unpause the game
        LevelController.RequestUnpause(this);
    }

    /// <summary>
    /// When restart button is clicked, reload the scene.
    /// </summary>
    public void RestartClicked(){
        LevelController.ReloadScene();
    }

    /// <summary>
    /// When menu button is clicked, go to the main menu (without saving),
    /// </summary>
    public void MainMenuClicked(){
        LevelController.LoadMenu(false);
    }
}
