using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{
    private Button m_button;

    private void OnEnable() {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnSkip);
    }

    private void OnDisable() {
        m_button.onClick.RemoveListener(OnSkip);
    }

    private void OnSkip() {
        ZoneCinematicManager cm = FindObjectOfType<ZoneCinematicManager>();
        if (cm != null) {
            cm.OnCinematicEnd();
        }
    }
}
