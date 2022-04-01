using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIScript : MonoBehaviour
{
    public CursorScript _cursorScript;

    [Header("Game UI")]
    public TextMeshProUGUI _ammoText;
    public Image _healthBG;
    public Image _healthBar;
    [HideInInspector] public float _healthBarMaxWidth;

    [Header("Inventory UI")]
    public GameObject _journalPanel;
    public TextMeshProUGUI _notesText;


    public UnityEvent OnStart;

    private void Start() {
        if (_cursorScript == null) _cursorScript = GetComponent<CursorScript>();
        if (_cursorScript == null) _cursorScript = FindObjectOfType<CursorScript>();

        if (_healthBar != null) _healthBarMaxWidth = _healthBar.rectTransform.sizeDelta.x;

        OnStart.Invoke();
    }
}
