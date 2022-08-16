using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// @deprecated not used anymore. <br/>
/// Class to manage old UI for the journal.
/// </summary>
public class JournalUIList : MonoBehaviour  /// @todo comment
{
    [Header("Self")]
    public GameObject journalPanel;

    [Header("Notes")]
    public TextMeshProUGUI notesText;

    [Header("Mission")]
    public GameObject missionCard;
}
