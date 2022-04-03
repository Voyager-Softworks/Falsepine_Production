using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIScript : MonoBehaviour
{
    public CursorScript cursorScript;

    [Header("Game UI")]
    public TextMeshProUGUI ammoText;
    public Image healthBG;
    public Image healthBar;
    [HideInInspector] public float healthBarMaxWidth;

    [Header("Inventory UI")]
    public BagUIList bagUIList;
    public JournalUIList journalUIList;


    public UnityEvent OnStart;

    private void Start() {
        if (cursorScript == null) cursorScript = GetComponent<CursorScript>();
        if (cursorScript == null) cursorScript = FindObjectOfType<CursorScript>();

        if (healthBar != null) healthBarMaxWidth = healthBar.rectTransform.sizeDelta.x;

        OnStart.Invoke();
    }
}
