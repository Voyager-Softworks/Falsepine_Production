using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ReturnPopup : ToggleableWindow
{
    public GameObject m_panel;
    public TextMeshProUGUI m_text;
    public Button returnButton;
    public Button cancelButton;
    public Button exitButton;

    private ExitGate exitGate;

    private void Start() {
        CloseWindow();
    }

    public override void Update() {
        base.Update();

        // use the exit gate to determine if the player can return to town
        if (exitGate != null)
        {
            if (exitGate.m_unlocked == true){
                returnButton.interactable = true;
                m_text.text = "Are you sure you want to return to town safely? \nYou will retain everything you've found";
            }
            else{
                returnButton.interactable = false;
                m_text.text = "Cannot travel with enemies nearby!";
            }
        }
        else{
            // otherwise, check if all Health_Base.allHealths are dead if they are EnemyHealth only
            bool allEnemiesDead = true;
            if (Health_Base.allHealths.Count > 0){
                foreach (Health_Base health in Health_Base.allHealths){
                    if (health is EnemyHealth){
                        if (!health.hasDied){
                            allEnemiesDead = false;
                        }
                    }
                }
            }
            if (allEnemiesDead){
                returnButton.interactable = true;
                m_text.text = "Are you sure you want to return to town safely? \nYou will retain everything you've found";
            }
            else{
                returnButton.interactable = false;
                m_text.text = "Cannot travel with enemies nearby!";
            }
        }

    }

    protected override void OnEnable() {
        base.OnEnable();

        returnButton.onClick.AddListener(OnReturnButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
        exitButton.onClick.AddListener(OnCancelButtonClicked);

        // exit gate
        if (exitGate == null){
            exitGate = FindObjectOfType<ExitGate>();
        }
    }

    protected override void OnDisable() {
        base.OnDisable();
        
        returnButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void OnReturnButtonClicked() {
        LevelController.LoadTown();
    }

    private void OnCancelButtonClicked() {
        CloseWindow();
    }

    public override bool IsOpen(){
        return m_panel.activeSelf;
    }

    public override void OpenWindow(){
        base.OpenWindow();
        m_panel.SetActive(true);
    }

    public override void CloseWindow(){
        base.CloseWindow();
        m_panel.SetActive(false);
    }
}
