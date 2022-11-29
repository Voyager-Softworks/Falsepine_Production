using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// When clicked, toggles the journal if it exists.
/// </summary>
public class JournalButton : MonoBehaviour
{
    Button m_button;

    private void Awake() {
        m_button = GetComponent<Button>();
    }

    private void OnEnable() {
        m_button.onClick.AddListener(ToggleJournal);
    }

    private void OnDisable() {
        m_button.onClick.RemoveListener(ToggleJournal);
    }

    private void ToggleJournal() {
        if (JournalManager.instance != null) {
            JournalManager.instance.ToggleWindow();
        }
    }
}
