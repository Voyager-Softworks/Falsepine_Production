using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages opening and closing the "pause" menu.
/// </summary>
public class PauseMenu : ToggleableWindow
{
    public InputAction pauseAction;

    public GameObject PausePanel;

    public Button TownButton;
    public Button MenuButton;

    [Header("Windows")]
    public GameObject AudioSettingsWindow;
    public GameObject VideoSettingsWindow;

    private ExitGate exitGate;

    protected override void OnEnable() {
        base.OnEnable();

        pauseAction.Enable();

        // exit gate
        if (exitGate == null){
            exitGate = FindObjectOfType<ExitGate>();
        }
    }

    protected override void OnDisable() {
        base.OnDisable();
        
        pauseAction.Disable();
    }

    public override void Update() {
        if (pauseAction.WasPressedThisFrame()) {
            if (!ToggleableWindow.AnyWindowOpen() || IsOpen()) {
                ToggleWindow();
            }
        }

        base.Update();

        // if exitGate is unlocked, allow town and menu buttons
        if (exitGate != null) {
            TownButton.interactable = exitGate.m_unlocked == true;
            MenuButton.interactable = exitGate.m_unlocked == true;

            
            // if mouse is over town or menu button, and exitGate is locked, change text to "Enemies remain"
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (exitGate.m_unlocked == false) {
                if (TownButton.GetComponent<RectTransform>().rect.Contains(TownButton.transform.InverseTransformPoint(mousePos))) {
                    TownButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enemies remain";
                }
                else {
                    TownButton.GetComponentInChildren<TextMeshProUGUI>().text = "Return to Town";
                }
                
                if (MenuButton.GetComponent<RectTransform>().rect.Contains(MenuButton.transform.InverseTransformPoint(mousePos))) {
                    MenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enemies remain";
                } else {
                    
                    MenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Main Menu";
                }
            }
            else{
                TownButton.GetComponentInChildren<TextMeshProUGUI>().text = "Return to Town";
                MenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Main Menu";
            }
        }
    }

    public override bool IsOpen()
    {
        return PausePanel.activeSelf;
    }
    public override void OpenWindow()
    {
        if (m_wasClosedThisFrame) return;

        // if tutorial popup is open, return
        if (FindObjectOfType<TutorialPopup>() && FindObjectOfType<TutorialPopup>().gameObject.activeSelf) {
            return;
        }
        // if toggleable windows are open, return
        if (ToggleableTownWindow.AnyWindowOpen()){
            return;
        }

        base.OpenWindow();
        PausePanel.SetActive(true);

        // pause the game
        LevelController.RequestPause(this);
    }
    public override void CloseWindow()
    {
        base.CloseWindow();
        PausePanel.SetActive(false);

        AudioSettingsWindow.SetActive(false);
        VideoSettingsWindow.SetActive(false);

        // unpause the game
        LevelController.RequestUnpause(this);
    }

    /// <summary>
    /// When restart button is clicked, reload the scene.
    /// </summary>
    public void TownClicked(){
        LevelController.LoadTown();
    }

    /// <summary>
    /// When menu button is clicked, go to the main menu
    /// </summary>
    public void MainMenuClicked(){
        LevelController.LoadMenu();
    }
}
