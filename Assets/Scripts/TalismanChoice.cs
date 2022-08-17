using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Interactable))]
public class TalismanChoice : ToggleableWindow
{
    public Button m_closeButton;
    public GameObject m_choicePanel;

    [Header("Talisman Choice")]
    public Button m_choice1;
    public Button m_choice2;

    //[Header("Choices")]
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable() {
        m_closeButton.onClick.AddListener(CloseWindow);
    }
    private void OnDisable() {
        m_closeButton.onClick.RemoveListener(CloseWindow);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ToggleableWindow overrides
    public override bool IsOpen()
    {
        return m_choicePanel.activeSelf;
    }
    public override void OpenWindow()
    {
        base.OpenWindow();
        m_choicePanel.SetActive(true);
    }
    public override void CloseWindow()
    {
        base.CloseWindow();
        m_choicePanel.SetActive(false);
    }
}
