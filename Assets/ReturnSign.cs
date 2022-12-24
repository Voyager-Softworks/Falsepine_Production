using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReturnSign : Interactable
{
    [Header("Return Sign")]
    public ReturnPopup m_returnPopup;

    override public void DoInteract()
    {
        base.DoInteract();

        m_returnPopup.OpenWindow();
    }
}