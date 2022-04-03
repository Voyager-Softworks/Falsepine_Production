using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public InputAction openBagAction;
    public InputAction openJournalAction;
    public InputAction closeAction;

    private UIScript _uiScript;

    // Start is called before the first frame update
    void Start()
    {
        openBagAction.Enable();
        openJournalAction.Enable();
        closeAction.Enable();

        openBagAction.performed += ctx => OpenBag();
        openJournalAction.performed += ctx => ToggleJournal();
        closeAction.performed += ctx => CloseAll();

        if (_uiScript == null) _uiScript = FindObjectOfType<UIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenBag()
    {

    }

    public void OpenJournal()
    {
        if (_uiScript == null) return;

        CloseAll();

        GameObject journalPanel = _uiScript._journalPanel;

        if (journalPanel == null) return;

        journalPanel.SetActive(true);
    }
    public void CloseJournal()
    {
        if (_uiScript == null) return;

        GameObject journalPanel = _uiScript._journalPanel;

        if (journalPanel == null) return;

        journalPanel.SetActive(false);
    }
    public void ToggleJournal()
    {
        if (_uiScript == null) return;

        GameObject journalPanel = _uiScript._journalPanel;

        if (journalPanel == null) return;

        if (journalPanel.activeSelf) CloseJournal();
        else OpenJournal();
    }

    public void CloseAll()
    {
        CloseJournal();
    }
}
