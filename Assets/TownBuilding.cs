using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class TownBuilding : ClickableObject
{
    public GameObject uiPanel;
    public GameObject objectPanel;

    // Start is called before the first frame update
    void Start()
    {
        CloseUI();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //if escape is pressed, close the panel
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseUI();
        }
    }

    public override void OnClick()
    {
        base.OnClick();

        OpenUI();
    }

    public void OpenUI()
    {
        uiPanel.SetActive(true);
        objectPanel.SetActive(true);
    }

    public void CloseUI()
    {
        uiPanel.SetActive(false);
        objectPanel.SetActive(false);
    }

    public void ToggleUI()
    {
        if (uiPanel.activeSelf)
        {
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }
}
