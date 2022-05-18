using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class TownBuilding : ClickableObject
{
    public enum BuildingType {
        BANK,
        HOME,
        INN,
        SHOP
    }

    public GameObject UI;

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
        UI.SetActive(true);
    }

    public void CloseUI()
    {
        UI.SetActive(false);
    }

    public void ToggleUI()
    {
        if (UI.activeSelf)
        {
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }
}
