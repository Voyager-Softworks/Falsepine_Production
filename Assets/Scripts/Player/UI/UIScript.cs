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
    public HotbarUIList hotbarUIList;
    public GameObject infoPannel;
    public TextMeshProUGUI infoText;

    [Header("Boss UI")]
    public GameObject bossUI;
    public Image bossHealthBG;
    public Image bossHealthBar;
    [HideInInspector] public float bossHealthBarMaxWidth;
    public TextMeshProUGUI bossNameText;

    [Header("Pause UI")]
    public GameObject pauseUI;


    public UnityEvent OnStart;

    private void Start() {
        if (cursorScript == null) cursorScript = GetComponent<CursorScript>();
        if (cursorScript == null) cursorScript = FindObjectOfType<CursorScript>();

        if (healthBar != null) healthBarMaxWidth = healthBar.rectTransform.sizeDelta.x;
        if (bossHealthBar != null) bossHealthBarMaxWidth = bossHealthBar.rectTransform.sizeDelta.x;

        OnStart.Invoke();

        // close pause
        ClosePauseMenu();
    }

    private void Update() {
        // if escape is pressed, toggle pause
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseUI == null) return;

        if (pauseUI.activeSelf)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    private void OpenPauseMenu(){
        if (pauseUI == null) return;

        pauseUI.SetActive(true);
    }

    private void ClosePauseMenu(){
        if (pauseUI == null) return;

        pauseUI.SetActive(false);
    }


}
