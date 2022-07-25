using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class TownBuilding : ClickableObject  /// @todo Comment
{
    public enum BuildingType {
        BANK,
        HOME,
        INN,
        STORE
    }

    public GameObject UI;

    public Camera uiCamera;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

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

    public virtual void OpenUI()
    {
        if (!UI) return;
        UI.SetActive(true);
        uiCamera.enabled = true;
    }

    public virtual void CloseUI()
    {
        if (!UI) return;
        UI.SetActive(false);
        uiCamera.enabled = false;
    }

    public virtual void ToggleUI()
    {
        if (!UI) return;
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
