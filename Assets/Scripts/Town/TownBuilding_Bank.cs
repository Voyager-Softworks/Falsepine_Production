using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Town building for the bank.
/// </summary>
public class TownBuilding_Bank : TownBuilding
{
    public Button m_upgradeButton;

    public override void OpenUI()
    {
        base.OpenUI();

        // update slots of bank
        EconomyManager.instance.UpdateBankSlotAmount();
    }
}
