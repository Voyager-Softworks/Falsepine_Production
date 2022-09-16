using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// A clickable building in the town. Used as a base class for specific buildings.
/// </summary>
public class TownBuilding : ClickableObject
{
    public enum BuildingType {
        BANK,
        HOME,
        INN,
        STORE
    }

    public GameObject UI;

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
        // if (Keyboard.current.escapeKey.wasPressedThisFrame)
        // {
        //     CloseUI();
        // }
    }

    /// <summary>
    /// When this building is clicked.
    /// </summary>
    public override void OnClick()
    {
        base.OnClick();

        ToggleableTownWindow ttw = GetComponent<ToggleableTownWindow>();
        if (ttw != null)
        {
            ttw.OpenWindow();
        }
    }

    /// <summary>
    /// Opens the attached UI.
    /// </summary>
    public virtual void OpenUI()
    {
        if (!UI) return;
        UI.SetActive(true);
    }

    /// <summary>
    /// Close the attached UI.
    /// </summary>
    public virtual void CloseUI()
    {
        if (!UI) return;
        UI.SetActive(false);
    }

    /// <summary>
    /// Toggle the attached UI.
    /// </summary>
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
