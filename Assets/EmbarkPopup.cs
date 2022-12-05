using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EmbarkPopup : ToggleableWindow
{
    public GameObject m_panel;
    public Button embarkButton;
    public Button cancelButton;
    public Button exitButton;

    private void Start() {
        CloseWindow();
    }

    public override void Update() {
        base.Update();

        // if gamepad in use, and currently selected object is not a child of this, select button
        if (
            CustomInputManager.LastInputWasGamepad && 
            (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.activeInHierarchy == false || EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform) == false) &&
            IsOpen()
        ) {
            EventSystem.current.SetSelectedGameObject(embarkButton.gameObject);
        }
    }

    protected override void OnEnable() {
        base.OnEnable();

        embarkButton.onClick.AddListener(OnEmbarkButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
        exitButton.onClick.AddListener(OnCancelButtonClicked);
    }

    protected override void OnDisable() {
        base.OnDisable();
        
        embarkButton.onClick.RemoveListener(OnEmbarkButtonClicked);
        cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        exitButton.onClick.RemoveListener(OnCancelButtonClicked);
    }

    private void OnEmbarkButtonClicked() {
        MissionManager.instance.TryEmbark();
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
