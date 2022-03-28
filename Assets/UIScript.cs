using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public CursorScript _cursorScript;
    public TextMeshProUGUI _ammoText;
    public Image _healthBG;
    public Image _healthBar;

    private void Start() {
        if (_cursorScript == null) _cursorScript = GetComponent<CursorScript>();
        if (_cursorScript == null) _cursorScript = FindObjectOfType<CursorScript>();
    }
}
